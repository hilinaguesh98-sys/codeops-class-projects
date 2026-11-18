

decimal[] topUps = new decimal[5];

for (int i = 0; i < topUps.Length; i++)
{
    Console.Write($"Enter top-up amount {i + 1}: ");

    topUps[i] = decimal.Parse(Console.ReadLine()!);
}

Console.WriteLine("\n Raw Top-Up Amounts:");

foreach (decimal amount in topUps)
{
    Console.WriteLine(amount);
}
