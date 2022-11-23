internal interface ITemperatureRepository
{
    Task Add(TimeAndTemperature timeAndTemperature);
}