using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BOP3000
{
    public class TokenTest
    {
        private readonly ILogger<TokenTest> _logger;

        public TokenTest(ILogger<TokenTest> logger)
        {
            _logger = logger;
        }

        [Function("TokenTest")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route =null)] HttpRequest req/*, String userid, String password*/)
        {
            String userid = req.Query["user"];
            String password = req.Query["password"];
            if (userid != null && password != null)
            {
                Boolean foundUser = FetchToken(userid, password);
                if (foundUser)
                {
                    return new OkObjectResult("User found!");
                    Console.WriteLine("user found!");
                }
                else
                {
                    return new OkObjectResult("Couldnt find user");
                    Console.WriteLine("Couldnt find user");
                }
            }
            else
            {
                return new OkObjectResult("Failed to get input");
                Console.WriteLine("Failed to get input");
            }

        }
        public Boolean FetchToken(String userid, String password)
        {
            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var userQuery = new SqlCommand("SELECT [e-mail], [password], [name] FROM [user] WHERE [e-mail] = @userid AND [password] = @password;", conn);
            userQuery.Parameters.AddWithValue("@userid", userid);
            userQuery.Parameters.AddWithValue("@password", password);
            using SqlDataReader reader = userQuery.ExecuteReader();
            Boolean foundUser = false;

            if (reader.HasRows)
            {
                foundUser = true;
            }

            return foundUser;

        }
    }
}
