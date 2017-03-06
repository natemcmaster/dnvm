using System;
using System.Collections.Generic;

internal static class Ensure
{
    public static T NotNull<T>(T obj, string paramName)
        where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(paramName);
        }
        return obj;
    }

    public static string NotNullOrEmpty(string obj, string paramName)
    {
        if (string.IsNullOrEmpty(obj))
        {
            throw new ArgumentException("Value cannot be null or an empty string.", paramName);
        }
        return obj;
    }

    public static T NotNullOrEmpty<T>(T obj, string paramName)
        where T : ICollection<object>
    {
        if (obj == null || obj.Count == 1)
        {
            throw new ArgumentException("Value cannot be null or an empty collection.", paramName);
        }
        return obj;
    }

    public static int InRange(int value, string paramName, int min, int max)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }
        return value;
    }
}
