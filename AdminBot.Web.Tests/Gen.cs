namespace AdminBot.Web.Tests;

public static class Gen
{
    public static long RandomLong()
    {
        return new Random().Next();
    }
    
    public static int RandomInt()
    {
        return new Random().Next();
    }
    
    public static int RandomInt(int maxValue)
    {
        return new Random().Next(maxValue);
    }
    
    public static int RandomInt(int minValue, int maxValue)
    {
        return new Random().Next(minValue, maxValue);
    }

    public static string RandomString()
    {
        return Guid.NewGuid().ToString();
    }

    public static bool RandomBool()
    {
        return new Random().Next(1, 10) > 5;
    }

    public static List<T> ListOfValues<T>(Func<T> nextItem)
    {
        var count = Gen.RandomInt(1, 5);
        var result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            result.Add(nextItem());
        }

        return result;
    }

    public static TimeSpan RandomTimeSpan()
    {
        return TimeSpan.FromSeconds(Gen.RandomInt(0, 9999));
    }
}