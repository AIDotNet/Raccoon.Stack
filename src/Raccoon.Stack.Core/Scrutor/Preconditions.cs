using System.Diagnostics;

namespace Raccoon.Stack.Core.Scrutor;

[DebuggerStepThrough]
internal static class Preconditions
{
    public static T NotNull<T>(T value, string parameterName)
        where T : class
    {
        if (ReferenceEquals(value, null))
        {
            NotEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    public static string NotEmpty(string value, string parameterName)
    {
        if (ReferenceEquals(value, null))
        {
            NotEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        if (value.Length == 0)
        {
            NotEmpty(parameterName, nameof(parameterName));

            throw new ArgumentException("String value cannot be null.", parameterName);
        }

        return value;
    }

    public static TEnum IsDefined<TEnum>(TEnum value, string parameterName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            NotEmpty(parameterName, nameof(parameterName));

            throw new ArgumentOutOfRangeException(parameterName);
        }

        return value;
    }
}