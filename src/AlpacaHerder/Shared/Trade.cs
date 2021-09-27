using System;

namespace AlpacaHerder.Shared {
    public record Trade(string Symbol, decimal Price, ulong Size, ulong TradeId, DateTime TimestampUtc);
}