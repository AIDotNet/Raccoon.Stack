namespace Raccoon.Stack.Extensions.DotNet.Utils;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class CheckUtil
{
    internal static T CheckArgumentNull<T>(T value, string parameterName) where T : class
    {
        if (null == value)
            throw new ArgumentNullException(parameterName);

        return value;
    }
}
