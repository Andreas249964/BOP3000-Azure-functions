using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BOP3000_functions
{
    public class CreateTask
    {
        private readonly ILogger<CreateTask> _logger;

        public CreateTask(ILogger<CreateTask> logger)
        {
            _logger = logger;
        }

        [Function("CreateTask")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            /*
            Task task = new Task();
            task.kort = req.Query["KortID"];
            task.beskrivelse = req.Query["beskrivelse"];
            */

            return new OkObjectResult("Under Construction");
        }
        
    }
    public class Task
    {
        public String kort { get; set; }
        public String beskrivelse { get; set; }
    }
}
