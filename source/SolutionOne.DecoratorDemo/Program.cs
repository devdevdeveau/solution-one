CoffeeDecorator coffee = new Coffee();
coffee = new Cream(coffee);
coffee = new Cream(coffee);
coffee = new Cream(coffee);
coffee = new Sugar(coffee);
coffee = new Sugar(coffee);
coffee = new Sugar(coffee);
coffee = new Hazelnut(coffee);
coffee = new ManagerDiscount(coffee);

Console.WriteLine(coffee.Name);
Console.WriteLine($"{coffee.CalculateCalories()} Calories");
Console.WriteLine($"{coffee.CalculateCost():C}");


internal abstract class CoffeeDecorator
{
    public abstract string Name { get; }
    public abstract decimal CalculateCost();
    public abstract decimal CalculateCalories();
}

internal class Coffee : CoffeeDecorator
{
    public override string Name => "Black Coffee";

    public override decimal CalculateCost()
    {
        return 1.00m;
    }

    public override decimal CalculateCalories()
    {
        return 5m;
    }
}

internal class LargeCoffee : CoffeeDecorator
{
    private readonly CoffeeDecorator _decorator;

    public LargeCoffee(CoffeeDecorator decorator)
    {
        _decorator = decorator;
    }

    public override string Name => $"Large {_decorator.Name}";

    public override decimal CalculateCost()
    {
        return _decorator.CalculateCost() * 1.25m;
    }

    public override decimal CalculateCalories()
    {
        return _decorator.CalculateCalories() * 1.15m;
    }
}

internal class SmallCoffee : CoffeeDecorator
{
    private readonly CoffeeDecorator _decorator;

    public SmallCoffee(CoffeeDecorator decorator)
    {
        _decorator = decorator;
    }

    public override string Name => $"Small {_decorator.Name}";

    public override decimal CalculateCost()
    {
        return _decorator.CalculateCost() * 0.75m;
    }

    public override decimal CalculateCalories()
    {
        return _decorator.CalculateCalories() * 0.85m;
    }
}

internal class Cream : CoffeeDecorator
{
    private readonly CoffeeDecorator _decorator;

    public Cream(CoffeeDecorator decorator)
    {
        _decorator = decorator;
    }

    public override string Name => $"{_decorator.Name} Cream";

    public override decimal CalculateCost()
    {
        return _decorator.CalculateCost() + 0.10m;
    }

    public override decimal CalculateCalories()
    {
        return _decorator.CalculateCalories() + 20m;
    }
}

internal class Sugar : CoffeeDecorator
{
    private readonly CoffeeDecorator _coffeeDecorator;

    public Sugar(CoffeeDecorator coffeeDecorator)
    {
        _coffeeDecorator = coffeeDecorator;
    }

    public override string Name => $"{_coffeeDecorator.Name} Sugar";

    public override decimal CalculateCost()
    {
        return _coffeeDecorator.CalculateCost() + 0m;
    }

    public override decimal CalculateCalories()
    {
        return _coffeeDecorator.CalculateCalories() + 50m;
    }
}

internal class Hazelnut : CoffeeDecorator
{
    private readonly CoffeeDecorator _coffeeDecorator;

    public Hazelnut(CoffeeDecorator coffeeDecorator)
    {
        _coffeeDecorator = coffeeDecorator;
    }

    public override string Name => $"{_coffeeDecorator.Name} Hazel Nut";

    public override decimal CalculateCost()
    {
        return _coffeeDecorator.CalculateCost() + 0.50m;
    }

    public override decimal CalculateCalories()
    {
        return _coffeeDecorator.CalculateCalories() + 10m;
    }
}

internal class ManagerDiscount : CoffeeDecorator
{
    private readonly CoffeeDecorator _coffeeDecorator;
    private readonly decimal _percentDiscount;

    public ManagerDiscount(CoffeeDecorator coffeeDecorator, decimal percentDiscount = 0.25m)
    {
        _coffeeDecorator = coffeeDecorator;
        _percentDiscount = percentDiscount;
    }

    public override string Name => $"Discounted {_coffeeDecorator.Name}";

    public override decimal CalculateCost()
    {
        return _coffeeDecorator.CalculateCost() * (100m - _percentDiscount);
    }

    public override decimal CalculateCalories()
    {
        return _coffeeDecorator.CalculateCalories();
    }
}

// Adding raspberry is really easy now