internal class HumidityAlert : IHumidityAlert
{
    private readonly TimeAndTemperature _thresholdTimeAndTemperature;

    public HumidityAlert(TimeAndTemperature thresholdTimeAndTemperature)
    {
        _thresholdTimeAndTemperature = thresholdTimeAndTemperature;
    }

    public bool ExceedThreshold(TimeAndTemperature timeAndTemperature)
    {
        return timeAndTemperature.Humidity > _thresholdTimeAndTemperature.Humidity;
    }

    public Task Alert(TimeAndTemperature timeAndTemperature)
    {
        Console.WriteLine($"At {timeAndTemperature.TemperatureReading} {timeAndTemperature.Humidity}% exceeds humidity limit of {_thresholdTimeAndTemperature.Humidity}%");

        return Task.CompletedTask;
    }
}