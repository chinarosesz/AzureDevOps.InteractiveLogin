using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ManagedClientConsoleAppSample
{
    public class Program
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD
        // Replace with your own if you already have one
        private const string clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";

        // The Authority is the sign-in URL of the tenant
        private const string authority = "https://login.microsoftonline.com/microsoft.com/v2.0";

        // Constant value to target Azure DevOps. Do not change  
        private static readonly string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; 
        
        // MSAL Public client app
        private static IPublicClientApplication application;

        public static async Task Main(string[] args)
        {
            // Retrieve organization value from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string organization = configuration["organization"];

            try
            {
                AuthenticationResult authResult = await SignInUserAndGetTokenUsingMSAL(scopes);

                // Create authorization header of the form "Bearer {AccessToken}"
                string authHeader = authResult.CreateAuthorizationHeader();

                // Finally call Azure DevOps to list all projects from organization
                await ListProjectsAsync(authHeader, organization);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong.");
                Console.WriteLine("Message: " + ex + "\n");
            }
            Console.ReadLine();
        }
       
        /// <summary>
        /// Sign-in user using MSAL and obtain an access token for Azure DevOps
        /// </summary>
        private static async Task<AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId).
                WithAuthority(authority).
                WithDefaultRedirectUri().
                Build();

            AuthenticationResult result;

            try
            {
                IEnumerable<IAccount> accounts = await application.GetAccountsAsync();
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // If the token has expired, prompt the user with a login prompt
                result = await application.AcquireTokenInteractive(scopes)
                        .WithClaims(ex.Claims)
                        .ExecuteAsync();
            }

            return result;
        }

        /// <summary>
        /// Get all projects in the organization that the authenticated user has access to and print the results.
        /// </summary>
        private static async Task ListProjectsAsync(string authHeader, string organization)
        {
            // use the httpclient
            using HttpClient client = new();
            client.BaseAddress = new Uri($"https://dev.azure.com/{organization}/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
            client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            // connect to the REST endpoint            
            HttpResponseMessage response = await client.GetAsync("_apis/projects");

            // check to see if we have a succesfull respond
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Retrieved projects successfully for {client.BaseAddress}:");
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                Console.WriteLine($"Failed to get response because it return a {response.StatusCode} with reason {response.ReasonPhrase}");
            }
        }
    }
}
