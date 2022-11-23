using IdentityServer4.Models;
using Microsoft.VisualBasic;
using FlintstonesUtils;

namespace FlintstonesAuthApi
{
    public class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> {
            new ApiScope("spike", "Test API Scope")
        };

        public static IEnumerable<Client> Clients => new List<Client> {
            new Client {
                ClientId = "testclient-bmm",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("1532e76dbe9d43d0dea98c331ca5ae8a65c5e8e8b99d3e2a42ae989356f6242a".Sha256()) },
                AllowedScopes = { "spike" },
                Claims = { new ClientClaim("kafka-topic", Hahahaha()) }
            },
            //new Client {
            //    ClientId = "testclient2",
            //    AllowedGrantTypes = GrantTypes.ClientCredentials,
            //    ClientSecrets = { new Secret("testsecret2".Sha256()) },
            //    AllowedScopes = { "spike" },
            //    Claims = { new ClientClaim("kafka-topic", Hahahaha()) }
            //},
        };

        public static string Hahahaha()
        {
            return SecurityOperation.EncryptString(DateTime.Now.ToString("d"));
        }
    }
}
