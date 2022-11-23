internal interface IHumidityAlert
{
    bool ExceedThreshold(TimeAndTemperature timeAndTemperature);
    Task Alert(TimeAndTemperature timeAndTemperature);
}