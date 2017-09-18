using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using static IdentityServer4.IdentityServerConstants;

namespace AD.AuthService
{
    class IdentityServerConfig
    {
        private readonly IConfiguration _config;
        public IdentityServerConfig(IConfiguration config)
        {
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        public IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("SolarMonitorApi", "Solar Monitor API")
                {
                    UserClaims = {"role"}
                }
            };
        }

        public IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "swagger",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // .ClientCredentials, // GrantTypes.Implicit, // GrantTypes.ClientCredentials,
                    

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(_config["AuthServiceSwaggerClientSecret"].Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "SolarMonitorApi" },

                    RedirectUris = new List<string>() {
                        "http://localhost:5001/swagger/o2c.html"
                    }
                },

                // Testing purposes only: used to generate long duration tokens for the integration tests

                // new Client
                // {
                //     ClientId = "integration-tests",

                //     // no interactive user, use the clientid/secret for authentication
                //     AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // .ClientCredentials, // GrantTypes.Implicit, // GrantTypes.ClientCredentials,
                    

                //     // secret for authentication
                //     ClientSecrets =
                //     {
                //         new Secret("secret".Sha256())
                //     },

                //     // scopes that client has access to
                //     AllowedScopes = new List<string>
                //     {
                //         "SolarMonitorApi",
                //         StandardScopes.OfflineAccess,
                //         //StandardScopes.OpenId,
                //         //StandardScopes.Profile,
                //         //StandardScopes.Email,
                //         //StandardScopes.Phone
                //     },

                //     RedirectUris = new List<string>() {
                //         "http://localhost:5001/swagger/o2c.html"
                //     },

                //     //This feature refresh token
                //     AllowOfflineAccess = true,
                //     //Access token life time is 7200 seconds (2 hour)
                //     AccessTokenLifetime = 1, // 300_000_000, // approx 10 years
                //     //Identity token life time is 7200 seconds (2 hour)
                //     //IdentityTokenLifetime = 7200
                // },
            };
        }
    }
}