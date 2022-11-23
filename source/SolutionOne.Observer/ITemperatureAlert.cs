internal interface ITemperatureAlert
{
    bool ExceedThreshold(TimeAndTemperature timeAndTemperature);
    Task Alert(TimeAndTemperature timeAndTemperature);
}