internal class TemperatureRepository : ITemperatureRepository
{
    public Task Add(TimeAndTemperature timeAndTemperature)
    {
        return Task.CompletedTask;
    }
}