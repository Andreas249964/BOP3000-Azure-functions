using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;

namespace GetAllBoards
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("GetAllBoards")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            List<Board> boards = SQLQuery();

            var jsonToReturn = JsonConvert.SerializeObject(boards);

            //return new OkObjectResult(jsonToReturn);
            return new JsonResult(jsonToReturn);
        }

        public List<Board> SQLQuery()
        {
            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            List<Board> boardList = new List<Board>();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand("SELECT TOP (1000) [board_id],[name] FROM [dbo].[board];", conn);
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Board board = new Board();

                    //board.KortID = reader["KortID"].ToString();
                    board.id = reader.GetString(0);

                    //board.Beskrivelse = reader["Beskrivelse"].ToString();
                    board.name = reader.GetString(1);

                    boardList.Add(board);

                }
            }

            return boardList;

        }
    }
    public class Board
    {
        public String id;
        public String name;
    }
}
