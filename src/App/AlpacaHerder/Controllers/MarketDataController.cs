﻿using AlpacaHerder.Controllers.Abstractions;
using AlpacaHerder.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpacaHerder.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketDataController : BaseApiController {

    private readonly ILogger<MarketDataController> _logger;
    private readonly IMediator _mediator;

    public MarketDataController(ILogger<MarketDataController> logger, IMediator mediator) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> Get(string symbol) {
        try {
            var snapshot = await _mediator.Send(new GetPrice(symbol));

            var data = new MarketData(snapshot.Symbol, snapshot.Trade.Price);

            return Ok(data);
        } catch (Exception ex) {
            _logger.LogError(ex.ToString());
            return Problem(ex.Message);
        }
    }
}