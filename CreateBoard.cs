using GetAllBoards;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BOP3000
{
    public class CreateBoard
    {
        private readonly ILogger<CreateBoard> _logger;

        public CreateBoard(ILogger<CreateBoard> logger)
        {
            _logger = logger;
        }

        [Function("CreateBoard")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            String boardID = req.Query["id"];
            String name = req.Query["name"];
            
            if (boardID != null && name != null)
            {
                Boolean success = InsertBoard(boardID, name);
                if (success)
                {
                    _logger.LogInformation("The query was successfully executed");
                    return new OkObjectResult("The query was successfully executed!");
                }
                else
                {
                    _logger.LogInformation("Query failed");
                    return new OkObjectResult("Query failed");
                }
            }
            else
            {
                _logger.LogInformation("Failed to get input");
                return new OkObjectResult("Failed to get input, make sure the url format is as follows: https://laprofunctions.azurewebsites.net/api/CreateBoard?id={id}&name={name}\n" +
                    "example: https://laprofunctions.azurewebsites.net/api/CreateBoard?id=123&name=Prosjekt A");
            }



        }
        public Boolean InsertBoard(String boardID, String name)
        {
            Boolean querySuccess = false;
            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var query = new SqlCommand("INSERT INTO [board] (board_id, name) VALUES (@boardID, @name);", conn);
            query.Parameters.AddWithValue("@boardID", boardID);
            query.Parameters.AddWithValue("@name", name);

            var result = query.ExecuteNonQuery();
            if (result != 0)
            {
                querySuccess = true;
            }
            return querySuccess;
        }
    }
}
