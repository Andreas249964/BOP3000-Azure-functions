using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BOP3000_functions
{
    public class CreateCard
    {
        private readonly ILogger<CreateCard> _logger;

        public CreateCard(ILogger<CreateCard> logger)
        {
            _logger = logger;
        }

        [Function("CreateCard")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            Card card = new Card();
            /*card.KortID = req.Query["KortID"];
            card.Beskrivelse = req.Query["beskrivelse"];
            card.Prosjekt = req.Query["prosjektID"];*/

            card.KortID = "1007";
            card.Beskrivelse = "Test av function Insert";
            card.Prosjekt = "1";

            String result = InsertCard(card);
            return new OkObjectResult("testing");
        }
        public String InsertCard(Card card)
        {
            string connectionString = "Server=tcp:laprodb.database.windows.net,1433;Initial Catalog=lapro;Persist Security Info=False;User ID=lapro;Password=bopdbadmin5%;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var Query = new SqlCommand($"INSERT INTO [dbo].[Kort] (KortID, Beskrive, Prosjekt) VALUES {card.KortID},{card.Beskrivelse},{card.Prosjekt}");
            var result = Query.ExecuteNonQuery();
            if (result !=0)
            {
                return "Card created succesfully";
                //Console.WriteLine("Card Created");
            }
            else
            {
                return "Failed to create card";
                //Console.WriteLine("Failed to create card");
            }
        }

        public class Card
        {
            public String KortID { get; set; }
            public String Beskrivelse { get; set; }
            public String Prosjekt { get; set; }
        }
    }
}
