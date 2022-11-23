using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IHumidityAlert, HumidityAlert>();
        services.AddTransient<ITemperatureAlert, TemperatureAlert>();
        services.AddTransient<ITemperatureRepository, TemperatureRepository>();
        services.AddTransient<TimeAndTemperatureHandler>();
        services.AddTransient(_=> new TimeAndTemperature(DateTimeOffset.MinValue, 70d, 40d));
    })
    .Build();

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;
var timeAndTemperatureHandler = provider.GetRequiredService<TimeAndTemperatureHandler>();
await timeAndTemperatureHandler.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 100d, 80d));
await timeAndTemperatureHandler.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 20d, 20d));
await timeAndTemperatureHandler.HandleReading(new TimeAndTemperature(DateTimeOffset.Now, 30d, 70d));

internal record TimeAndTemperature(DateTimeOffset TemperatureReading, double Temperature, double Humidity);

internal class TimeAndTemperatureHandler
{
    private readonly IHumidityAlert _humidityAlert;
    private readonly ITemperatureAlert _temperatureAlert;
    private readonly ITemperatureRepository _temperatureRepository;

    public TimeAndTemperatureHandler(IHumidityAlert humidityAlert, ITemperatureAlert temperatureAlert, ITemperatureRepository temperatureRepository)
    {
        _humidityAlert = humidityAlert;
        _temperatureAlert = temperatureAlert;
        _temperatureRepository = temperatureRepository;
    }

    public async Task HandleReading(TimeAndTemperature timeAndTemperature)
    {
        List<Task> tasks = new(3);

        if (_humidityAlert.ExceedThreshold(timeAndTemperature))
        {
            tasks.Add(_humidityAlert.Alert(timeAndTemperature));
        }

        if (_temperatureAlert.ExceedThreshold(timeAndTemperature))
        {
            tasks.Add(_temperatureAlert.Alert(timeAndTemperature));
        }

        tasks.Add(_temperatureRepository.Add(timeAndTemperature));

        await Task.WhenAll(tasks);
    }
}