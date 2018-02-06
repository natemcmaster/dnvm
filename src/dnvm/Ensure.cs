using System;

internal static class Ensure
{
    public static string NotNullOrEmpty(string str, string paramName)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException("Value cannot be null or an empty string.", paramName);
        }

        return str;
    }
}
