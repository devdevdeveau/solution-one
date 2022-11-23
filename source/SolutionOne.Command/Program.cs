using System.Collections.Concurrent;

IDictionary<string, Stock> pool = new ConcurrentDictionary<string, Stock>(
    new KeyValuePair<string, Stock>[]
    {
        new("MS", new Stock() { Cost = 100m, ShareCount = 10000m, Symbol = "MS" }),
        new("AMZN", new Stock {Cost = 123m, ShareCount = 200000m, Symbol = "AMZN"})
    });

StockLedger ledger = new(pool);

Console.WriteLine($"Bought {ledger.BuyStock("MS", 1000):F2} Shares of MS");
Console.WriteLine($"Bought {ledger.BuyStock("AMZN", 50):F2} Shares of AMZN");
Console.WriteLine(ledger);

Console.WriteLine($"Sold {ledger.SellStock("MS", 150m)}");
Console.WriteLine(ledger);

internal class StockLedger
{
    private readonly IDictionary<string, Stock> _pool;

    public StockLedger(IDictionary<string, Stock> pool)
    {
        _pool = pool;
    }

    public decimal BuyStock(string symbol, decimal amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        if (!_pool.ContainsKey(symbol))
        {
            throw new InvalidOperationException($"Cannot Find Symbol {symbol} for purchase");
        }

        var stock = _pool[symbol];

        var shares = amount / stock.Cost;

        if (shares > 0 && stock.ShareCount > shares)
        {
            stock.ShareCount -= shares;
        }

        return shares;
    }

    public decimal SellStock(string symbol, decimal shares)
    {
        if (shares <= 0)
        {
            return 0;
        }

        if (!_pool.ContainsKey(symbol))
        {
            throw new InvalidOperationException($"Cannot Find Symbol {symbol} for sale");
        }

        var stock = _pool[symbol];

        var proceeds = stock.Cost * shares;

        stock.ShareCount += shares;

        return proceeds;
    }

    public override string ToString()
    {
        return _pool.Values.Aggregate(string.Empty,
            (s, stock) => $"{s}{Environment.NewLine}Stock: {stock.Symbol} Shares: {stock.ShareCount:F2}");
    }
}

internal class Stock
{
    public required string Symbol { get; init; }
    public required decimal Cost { get; init; }
    public required decimal ShareCount { get; set; }
}