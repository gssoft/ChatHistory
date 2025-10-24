using System;

// Интерфейс для распределений
public interface IDistribution<TValue>
{
    TValue Sample(Random random);
}

// Реализация нормального распределения
public class NormalDistribution : IDistribution<double>
{
    private readonly double _mean;
    private readonly double _stdDev;

    public NormalDistribution(double mean = 0.0, double stdDev = 1.0)
    {
        if (stdDev <= 0)
            throw new ArgumentException("Standard deviation must be positive.", nameof(stdDev));

        _mean = mean;
        _stdDev = stdDev;
    }

    public double Sample(Random random)
    {
        // Используем метод полярных координат Бокса-Мюллера
        double u1, u2, w;
        do
        {
            u1 = 2.0 * random.NextDouble() - 1.0; // [-1, 1)
            u2 = 2.0 * random.NextDouble() - 1.0; // [-1, 1)
            w = u1 * u1 + u2 * u2;
        } while (w >= 1.0 || w == 0);

        w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
        return _mean + _stdDev * u1 * w; // Возвращаем одно из значений
    }
}

// Основной класс генератора
public class RandomGenerator<TValue, TDistribution>
    where TDistribution : IDistribution<TValue>, new()
{
    private readonly Random _random;
    private readonly TDistribution _distribution;

    public RandomGenerator()
    {
        _random = new Random();
        _distribution = new TDistribution();
    }

    public RandomGenerator(int seed)
    {
        _random = new Random(seed);
        _distribution = new TDistribution();
    }

    public RandomGenerator(Random random, TDistribution distribution)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
        _distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    public TValue Next() => _distribution.Sample(_random);
}

// Пример использования
public class Program
{
    public static void Main()
    {
        var generator = new RandomGenerator<double, NormalDistribution>();

        Console.WriteLine("Sample values from Normal Distribution (mean=0, stdDev=1):");
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(generator.Next());
        }
    }
}