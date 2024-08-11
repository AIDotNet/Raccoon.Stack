using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Raccoon.Stack.Data.Exceptions;

[Serializable]
public class RaccoonArgumentException : RaccoonException
{
    private string? _paramName;
    protected string? ParamName => _paramName;


    public RaccoonArgumentException(
        string message,
        LogLevel? logLevel = null)
        : base(message, logLevel)
    {
    }

    public RaccoonArgumentException(
        string message,
        string paramName,
        LogLevel? logLevel = null)
        : base(message, logLevel)
    {
        _paramName = paramName;
    }

    public RaccoonArgumentException(
        string? paramName,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : this((Exception?)null, errorCode, logLevel, parameters)
    {
        _paramName = paramName;
    }

    public RaccoonArgumentException(
        Exception? innerException,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : base(innerException, errorCode, logLevel, parameters)
    {
    }

    public RaccoonArgumentException(string message, Exception? innerException, LogLevel? logLevel = null)
        : base(message, innerException, logLevel)
    {
    }

    protected RaccoonArgumentException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    public static void ThrowIfNullOrEmptyCollection<T>([NotNull] IEnumerable<T>? arguments,
        [CallerArgumentExpression("arguments")]
        string? paramName = null)
    {
        ThrowIf(arguments is null || !arguments.Any(),
            paramName,
            Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR);
    }

    public static void ThrowIfNull([NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Constants.ExceptionErrorCode.NOT_NULL_VALIDATOR);
    }

    public static void ThrowIfNullOrEmpty([NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR);
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrWhiteSpace(argument?.ToString()),
            paramName,
            Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR);
    }

    public static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Constants.ExceptionErrorCode.LESS_THAN_OR_EQUAL_VALIDATOR,
            null,
            maxValue);
    }

    public static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Constants.ExceptionErrorCode.LESS_THAN_VALIDATOR,
            null,
            maxValue);
    }

    public static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Constants.ExceptionErrorCode.GREATER_THAN_OR_EQUAL_VALIDATOR,
            null,
            minValue);
    }

    public static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Constants.ExceptionErrorCode.GREATER_THAN_VALIDATOR,
            null,
            minValue);
    }

    public static void ThrowIfOutOfRange<T>(T argument,
        T minValue,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0,
            paramName,
            Constants.ExceptionErrorCode.OUT_OF_RANGE_VALIDATOR,
            null,
            minValue,
            maxValue);
    }

    public static void ThrowIfContain(string? argument,
        string parameter,
        [CallerArgumentExpression("argument")] string? paramName = null)
        => ThrowIfContain(argument, parameter, StringComparison.OrdinalIgnoreCase, paramName);

    public static void ThrowIfContain(string? argument,
        string parameter,
        StringComparison stringComparison,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument != null)
            ThrowIf(argument.Contains(parameter, stringComparison),
                paramName,
                Constants.ExceptionErrorCode.NOT_CONTAIN_VALIDATOR
            );
    }

    public static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        string? paramName,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
    {
        if (condition)
            Throw(paramName, errorCode, Constants.ExceptionErrorCode.GetErrorMessage(errorCode), logLevel, parameters);
    }

    public static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        string? paramName,
        string errorCode,
        string? errorMessage,
        LogLevel? logLevel = null,
        params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, errorMessage, logLevel, parameters);
    }

    [DoesNotReturn]
    private static void Throw(
        string? paramName,
        string errorCode,
        string? errorMessage,
        LogLevel? logLevel,
        params object[] parameters) =>
        throw new RaccoonArgumentException(paramName, errorCode, logLevel, parameters)
        {
            ErrorMessage = errorMessage
        };

    protected override object[] GetParameters()
    {
        var parameters = new List<object>()
        {
            ParamName!
        };
        parameters.AddRange(Parameters);
        return parameters.ToArray();
    }
}