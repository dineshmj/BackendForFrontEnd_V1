using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace FW.IDP;

public static class TestUsers
{
    public static List<TestUser> Users =>
        new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "alice",
                Claims = new List<Claim>
                {
                    new Claim("name", "Alice Smith"),
                    new Claim("given_name", "Alice"),
                    new Claim("family_name", "Smith"),
                    new Claim("email", "alice@example.com"),
                    new Claim("email_verified", "true"),
                    new Claim("role", "admin")
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "bob",
                Claims = new List<Claim>
                {
                    new Claim("name", "Bob Johnson"),
                    new Claim("given_name", "Bob"),
                    new Claim("family_name", "Johnson"),
                    new Claim("email", "bob@example.com"),
                    new Claim("email_verified", "true"),
                    new Claim("role", "user")
                }
            }
        };
}
