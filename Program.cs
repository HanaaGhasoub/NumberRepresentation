namespace CurrencyStringConverter;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter number: ");
        var input = "Start";
        while (!string.IsNullOrEmpty(input))
        {
            input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                return;
            var output = CurrencyStringRepresentation.GetStringRepresentation(input);
            Console.WriteLine(output);
        }
    }
}