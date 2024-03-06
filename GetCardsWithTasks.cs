using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using static GetCardsWithTasks.GetCardsWithTasks;

namespace GetCardsWithTasks
{
    public class GetCardsWithTasks
    {
        private readonly ILogger<GetCardsWithTasks> _logger;

        public GetCardsWithTasks(ILogger<GetCardsWithTasks> logger)
        {
            _logger = logger;
        }

        [Function("GetCardsWithTasks")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            String BoardID = req.Query["BoardID"];
            //String BoardID = "1";
            List<Card> CardsWithTasks = FetchCards(BoardID);

            var jsonToReturn = JsonConvert.SerializeObject(CardsWithTasks);
            return new JsonResult(jsonToReturn);

        }
        public List<Card> FetchCards(String BoardID)
        {
            List<Card> cardList = new List<Card>();

            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cardQuery = new SqlCommand($"SELECT [card_id],[name] FROM [dbo].[card] WHERE board = {BoardID};", conn);
            using SqlDataReader readerCard = cardQuery.ExecuteReader();

            

            if (readerCard.HasRows) 
            {
                while (readerCard.Read()) 
                {
                    Card card = new Card();
                    card.CardID = readerCard.GetString(0);
                    card.Name = readerCard.GetString(1);
                    card.Tasks = FetchTasks(card.CardID);

                    cardList.Add(card);
                }
            } return cardList;
        }
        public List<Task> FetchTasks(String CardID)
        {
            List<Task> taskList = new List<Task>();

            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var taskQuery = new SqlCommand($"SELECT [task_id],[name],[description] FROM [dbo].[task] WHERE card = {CardID};", conn);
            using SqlDataReader readerTask = taskQuery.ExecuteReader();

            if (readerTask.HasRows)
            {
                while (readerTask.Read())
                {
                    Task task = new Task();
                    task.Name = readerTask.GetString(0);


                    taskList.Add(task);
                }
            }
            return taskList;
        }
        public class Card
        {
            public String CardID { get; set; }
            public String Name { get; set; }
            public List<Task> Tasks { get; set; }
        }
        public class Task
        {
            public String TaskID { get; set; }
            public String Name { get; set; }
            public String Description { get; set; }
        }
    }
}
