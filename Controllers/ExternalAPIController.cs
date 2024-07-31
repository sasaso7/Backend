using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BankBackend.Database.Models;
using BankBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankBackend.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ExternalAPIController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IActivityService _activityService;
        private const string KanyeApiUrl = "https://api.kanye.rest/";

        public ExternalAPIController(IHttpClientFactory httpClientFactory, IActivityService activityService)
        {
            _httpClientFactory = httpClientFactory;
            _activityService = activityService;
        }

        [HttpPost("quote")]
        public async Task<IActionResult> GetKanyeQuote([FromBody] KanyeQuoteRequest request)
        {
            try
            {
                // Call the Kanye.rest API
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(KanyeApiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response
                var quoteResponse = JsonSerializer.Deserialize<KanyeQuoteResponse>(content);

                // Create a new activity
                var createActivity = new CreateActivity
                {
                    AccountID = request.AccountID,
                    Name = "Kanye Quote",
                    Description = quoteResponse.quote
                };

                var newActivity = await _activityService.CreateActivityAsync(createActivity, request.AccountID);

                return Ok(quoteResponse.quote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // You can add more methods here for other endpoints in the future
    }

    public class KanyeQuoteRequest
    {
        public required string AccountID { get; set; }
    }

    public class KanyeQuoteResponse
    {
        public string quote { get; set; }
    }
}
