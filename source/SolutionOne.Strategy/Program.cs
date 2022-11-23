const double distance = 15d;
const double walkMph = 3.5d;
const double bikeMph = 15d;
const double carMph = 60d;

double CalculateTravelTime(double travelDistance, double travelSpeed) => travelDistance / travelSpeed;

Console.WriteLine("The travelDistance to work is 15 miles, you can (W)alk, (B)ike, or Drive a (C)ar");
Console.WriteLine("How do you want to go to work today?");
var key = Console.ReadKey().KeyChar;

switch (char.ToUpperInvariant(key))
{
    case 'W':
        Console.WriteLine($"Walking will take {TimeSpan.FromHours(CalculateTravelTime(distance, walkMph)).TotalMinutes:F2} minutes.");
        break;
    case 'B':
        Console.WriteLine($"Biking will take {TimeSpan.FromHours(CalculateTravelTime(distance, bikeMph)).TotalMinutes:F2} minutes.");
        break;
    case 'C':
        Console.WriteLine($"Driving will take {TimeSpan.FromHours(CalculateTravelTime(distance, carMph)).TotalMinutes:F2} minutes.");
        break;

    default:
        Console.WriteLine($"Unsupported Selection {key}");
        return;
}
