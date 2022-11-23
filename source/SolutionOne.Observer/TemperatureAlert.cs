internal class TemperatureAlert : ITemperatureAlert
{
    private readonly TimeAndTemperature _thresholdTimeAndTemperature;

    public TemperatureAlert(TimeAndTemperature thresholdTimeAndTemperature)
    {
        _thresholdTimeAndTemperature = thresholdTimeAndTemperature;
    }

    public bool ExceedThreshold(TimeAndTemperature timeAndTemperature)
    {
        return timeAndTemperature.Temperature > _thresholdTimeAndTemperature.Temperature;
    }

    public Task Alert(TimeAndTemperature timeAndTemperature)
    {
        Console.WriteLine($"At {timeAndTemperature.TemperatureReading} {timeAndTemperature.Temperature}c exceeds temperature limit of {_thresholdTimeAndTemperature.Temperature}c");

        return Task.CompletedTask;
    }
}