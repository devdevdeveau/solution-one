using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IHumidityAlert, HumidityAlert>();
        services.AddTransient<ITemperatureAlert, TemperatureAlert>();
        services.AddTransient<ITemperatureRepository, TemperatureRepository>();
        services.AddTransient(_ => new TimeAndTemperature(DateTimeOffset.MinValue, 70d, 40d));
        
        // Observable
        services.AddTransient<TimeAndTemperatureObservable>();
        
        // Observer
        services.AddTransient<ITimeAndTemperatureObserver, TemperatureAlertObserver>();
        services.AddTransient<ITimeAndTemperatureObserver, HumidityAlertObserver>();
        services.AddTransient<ITimeAndTemperatureObserver, RepositoryObserver>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;
var timeAndTemperatureObservable = provider.GetRequiredService<TimeAndTemperatureObservable>();
await timeAndTemperatureObservable.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 100d, 80d));
await timeAndTemperatureObservable.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 20d, 20d));
await timeAndTemperatureObservable.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 30d, 70d));

internal class TimeAndTemperatureObservable
{
    private readonly IEnumerable<ITimeAndTemperatureObserver> _observers;

    public TimeAndTemperatureObservable(IEnumerable<ITimeAndTemperatureObserver> observers)
    {
        _observers = observers;
    }

    public async Task HandleReading(TimeAndTemperature timeAndTemperature)
    {
        await Task.WhenAll(_observers.Select(observer => observer.Handle(timeAndTemperature)));
    }
}

internal interface ITimeAndTemperatureObserver
{
    Task Handle(TimeAndTemperature timeAndTemperature);
}

internal class TemperatureAlertObserver : ITimeAndTemperatureObserver
{
    private readonly ITemperatureAlert _temperatureAlert;

    public TemperatureAlertObserver(ITemperatureAlert temperatureAlert)
    {
        _temperatureAlert = temperatureAlert;
    }

    public Task Handle(TimeAndTemperature timeAndTemperature)
    {
        return _temperatureAlert.ExceedThreshold(timeAndTemperature) ? _temperatureAlert.Alert(timeAndTemperature) : Task.CompletedTask;
    }
}

internal class HumidityAlertObserver : ITimeAndTemperatureObserver
{
    private readonly IHumidityAlert _humidityAlert;

    public HumidityAlertObserver(IHumidityAlert humidityAlert)
    {
        _humidityAlert = humidityAlert;
    }

    public Task Handle(TimeAndTemperature timeAndTemperature)
    {
        return _humidityAlert.ExceedThreshold(timeAndTemperature) ? _humidityAlert.Alert(timeAndTemperature) : Task.CompletedTask;
    }
}

internal class RepositoryObserver : ITimeAndTemperatureObserver
{
    private readonly ITemperatureRepository _temperatureRepository;

    public RepositoryObserver(ITemperatureRepository temperatureRepository)
    {
        _temperatureRepository = temperatureRepository;
    }

    public Task Handle(TimeAndTemperature timeAndTemperature)
    {
        return _temperatureRepository.Add(timeAndTemperature);
    }
}

internal record TimeAndTemperature(DateTimeOffset TemperatureReading, double Temperature, double Humidity);