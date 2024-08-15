using System;

namespace AlpacaHerder.Shared {
    public record Trade(string Symbol, decimal Price, decimal Size, ulong TradeId, DateTime TimestampUtc);
}