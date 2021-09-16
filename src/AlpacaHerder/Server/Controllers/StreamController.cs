using AlpacaHerder.Server.Controllers.Abstractions;
using AlpacaHerder.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Controllers {
    public class StreamController : BaseApiController {

        private readonly ILogger<StreamController> _logger;
        private readonly IStreamingDataService _streamingDataService;

        public StreamController(ILogger<StreamController> logger, IStreamingDataService streamingDataService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _streamingDataService = streamingDataService ?? throw new ArgumentNullException(nameof(streamingDataService));
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> Get(string symbol, CancellationToken cancellationToken) {
            try {
                var subscription = await _streamingDataService.GetSubscriptionAsync(symbol, cancellationToken);
                return Ok(subscription);
            } catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return Problem(ex.Message);
            }
        }

        [HttpPost("{symbol}")]
        public async Task<IActionResult> Subscribe(string symbol, CancellationToken cancellationToken) {
            try {
                var subscription = await _streamingDataService.SubscribeAsync(symbol, cancellationToken);

                return Ok(subscription);
            } catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{symbol}")]
        public async Task<IActionResult> Unsubscribe(string symbol, CancellationToken cancellationToken) {
            try {
                var subscription = await _streamingDataService.UnsubscribeAsync(symbol, cancellationToken);

                return Ok(subscription);
            } catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return Problem(ex.Message);
            }
        }
    }
}