using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

var channel = Channel.CreateUnbounded<ICommand>();

IDictionary<string, Stock> pool = new ConcurrentDictionary<string, Stock>(
    new KeyValuePair<string, Stock>[]
    {
        new("MS", new Stock { Cost = 100m, ShareCount = 10000m, Symbol = "MS" }),
        new("AMZN", new Stock {Cost = 123m, ShareCount = 200000m, Symbol = "AMZN"})
    });

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) =>
    {
        builder.AddConsole();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(pool);
        services.AddSingleton<StockTerminal>();
        services.AddSingleton<StockLedger>();
        services.AddSingleton(channel.Writer);
        services.AddSingleton(channel.Reader);
        services.AddHostedService<Worker>();
    })
    .Build();

var terminal = host.Services.GetRequiredService<StockTerminal>();


terminal.PushCommand(new BuyStockCommand("MS", 1000m));
terminal.PushCommand(new BuyStockCommand("MS", 1000m));
terminal.PushCommand(new BuyStockCommand("MS", 1000m));
terminal.PushCommand(new BuyStockCommand("MS", 1000m));

terminal.PushCommand(new SellStockCommand("AMZN", 199999m));

host.Run();

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ChannelReader<ICommand> _reader;
    private readonly StockLedger _ledger;

    public Worker(ILogger<Worker> logger, ChannelReader<ICommand> reader, StockLedger ledger)
    {
        _logger = logger;
        _reader = reader;
        _ledger = ledger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (_reader.TryRead(out ICommand command))
            {
                command.Execute(_ledger);
            }
            await Task.Delay(1000, stoppingToken);

            _logger.LogInformation("{ledger}", _ledger);
        }
    }
}

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

internal class SellStockCommand : ICommand
{
    private readonly string _symbol;
    private readonly decimal _shares;

    public SellStockCommand(string symbol, decimal shares)
    {
        _symbol = symbol;
        _shares = shares;
    }
    
    public bool CanExecute(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(_symbol) && _shares > 0 && parameter is StockLedger;
    }

    public void Execute(object? parameter)
    {
        if (parameter is StockLedger ledger)
        {
            ledger.SellStock(_symbol, _shares);
        }
    }

    public event EventHandler? CanExecuteChanged;
}

internal class BuyStockCommand : ICommand
{
    private readonly string _symbol;
    private readonly decimal _amount;

    public BuyStockCommand(string symbol, decimal amount)
    {
        _symbol = symbol;
        _amount = amount;
    }

    public bool CanExecute(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(_symbol) && _amount >=0 && parameter is StockLedger;
    }

    public void Execute(object? parameter)
    {
        if (parameter is StockLedger ledger)
        {
            ledger.BuyStock(_symbol, _amount);
        }
    }

    public event EventHandler? CanExecuteChanged;
}

internal class StockTerminal
{
    private readonly ChannelWriter<ICommand> _writer;

    public StockTerminal(ChannelWriter<ICommand> writer)
    {
        _writer = writer;
    }

    public bool PushCommand(ICommand command)
    {
        return _writer.TryWrite(command);
    }
}