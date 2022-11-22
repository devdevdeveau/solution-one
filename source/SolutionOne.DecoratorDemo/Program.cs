Brew brew = new(Coffee.LargeCoffee, Temperature.Hot, AddOn.Cream, AddOn.Cream, AddOn.Hazelnut, AddOn.Hazelnut,
    AddOn.Hazelnut, AddOn.Sugar);

Console.WriteLine(brew);


internal class Coffee
{
    private Coffee()
    {
        // locking construction to static initializer
    }

    public required string Name { get; init; }

    public required double Calories { get; init; }
    
    public required decimal Cost { get; init; }

    public static Coffee SmallCoffee = new()
        { Name = "Small Black Coffee", Calories = 5, Cost = 1.00m };

    public static Coffee RegularCoffee = new()
        { Name = "Black Regular Coffee", Calories = 10, Cost = 2.00m };

    public static Coffee LargeCoffee = new()
        { Name = "Large Black Coffee", Calories = 20, Cost = 3.00m };
}

internal enum Temperature
{
    Hot,
    Warm,
    Cold,
    Iced
}

internal class AddOn
{
    private AddOn()
    {
        // Locking construction to static initializer
    }

    public required string Name { get; init; }
    public required double Calories { get; init; }
    public required decimal Cost { get; init; }

    public static AddOn Sugar = new() { Calories = 20, Cost = 0m, Name = nameof(Sugar) };
    public static AddOn Cream = new() { Calories = 30, Cost = 0m, Name = nameof(Cream) };
    public static AddOn Hazelnut = new() { Calories = 20, Cost = 0.50m, Name = nameof(Hazelnut) };
}

internal class Brew
{
    private readonly Coffee _coffee;
    private readonly Temperature _temperature;
    private readonly AddOn[] _addOns;

    public Brew(Coffee coffee, Temperature temperature, params AddOn[] addOns)
    {
        _coffee = coffee;
        _temperature = temperature;
        _addOns = addOns;
    }

    public string Name => $"{_temperature} {_coffee.Name} {_addOns.Aggregate("with ", AddOnAggregator)}";
    public decimal Cost => _coffee.Cost + _addOns.Sum(addOn => addOn.Cost);
    public double Calories => _coffee.Calories + _addOns.Sum(addOn => addOn.Calories);

    private string AddOnAggregator(string current, AddOn addOn)
    {
        return $"{current}{addOn.Name} ";
    }

    public override string ToString()
    {
         return $"""
                Name: {Name}
                Calories: {Calories}
                Cost: {Cost:C}
                """;
    }
}

// Adding raspberry is easy now
// It's a bit ... wordy though