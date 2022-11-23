const double distance = 15d;

Console.WriteLine("The travelDistance to work is 15 miles, you can (W)alk, (B)ike, or Drive a (C)ar");
Console.WriteLine("How do you want to go to work today?");
var key = Console.ReadKey(true).KeyChar;

Context context = new(new WalkingStrategy(), new BikingStrategy(), new DrivingStrategy());
context.SelectStrategy(key);
Console.WriteLine($"{context.Name} will take {context.CalculateTravelTime(distance).TotalMinutes:F2} minutes.");

internal interface ITravelStrategy
{
    string Name { get; }
    char Selector { get; }
    TimeSpan CalculateTravelTime(double travelDistance);
}

internal class Context : ITravelStrategy
{
    private readonly ITravelStrategy[] _strategies;
    private ITravelStrategy _selectedTravelStrategy = FailOverStrategy.Instance;

    public Context(params ITravelStrategy[] strategies)
    {
        _strategies = strategies;
    }

    public void SelectStrategy(char selector)
    {
        foreach (var strategy in _strategies)
        {
            if (!strategy.Selector.Equals(selector)) continue;
            _selectedTravelStrategy = strategy;
            break;
        }
    }

    public string Name => _selectedTravelStrategy.Name;
    public char Selector => '\0';
    public TimeSpan CalculateTravelTime(double travelDistance)
    {
        return _selectedTravelStrategy.CalculateTravelTime(travelDistance);
    }
}

internal class FailOverStrategy : ITravelStrategy
{
    public static ITravelStrategy Instance = new FailOverStrategy();

    private FailOverStrategy()
    {

    }

    public string Name => "Invalid Selection";
    public char Selector => '\0';
    public TimeSpan CalculateTravelTime(double travelDistance)
    {
        return TimeSpan.Zero;
    }
}

internal class WalkingStrategy : ITravelStrategy
{
    private const double Speed = 3.5d;
    public string Name => "Walking";
    public char Selector => 'W';
    public TimeSpan CalculateTravelTime(double travelDistance)
    {
        return TimeSpan.FromHours(travelDistance / Speed);
    }
}

internal class BikingStrategy : ITravelStrategy
{
    private const double Speed = 15d;
    public string Name => "Biking";
    public char Selector => 'B';
    public TimeSpan CalculateTravelTime(double travelDistance)
    {
        return TimeSpan.FromHours(travelDistance / Speed);
    }
}

internal class DrivingStrategy : ITravelStrategy
{
    private const double Speed = 60d;
    public string Name => "Driving";
    public char Selector => 'C';
    public TimeSpan CalculateTravelTime(double travelDistance)
    {
        return TimeSpan.FromHours(travelDistance / Speed);
    }
}