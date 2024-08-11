using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Raccoon.Stack.Data.Constants;

namespace Raccoon.Stack.Data.Exceptions;

[Serializable]
public class RaccoonException : Exception
{
    private LogLevel? _logLevel;

    public LogLevel? LogLevel => _logLevel;

    private string? _errorCode;

    public string? ErrorCode => _errorCode;

    /// <summary>
    /// Provides error message that I18n is not used
    /// </summary>
    protected string? ErrorMessage { get; set; }

    private object[] _parameters;

    public object[] Parameters => _parameters;

    private bool _initialize;


    private bool _supportI18n;

    internal bool SupportI18n
    {
        get
        {
            TryInitialize();
            return _supportI18n;
        }
    }

    public override string Message => GetLocalizedMessage();


    private void TryInitialize()
    {
        if (_initialize)
            return;

        Initialize();
    }

    private void Initialize()
    {
        _supportI18n = false;
        _initialize = true;
    }

    public RaccoonException()
    {
    }

    public RaccoonException(string message, LogLevel? logLevel = null)
        : base(message)
    {
        _logLevel = logLevel;
    }

    public RaccoonException(string errorCode, LogLevel? logLevel, params object[] parameters)
        : this(null, errorCode, logLevel, parameters)
    {
    }

    public RaccoonException(Exception? innerException, string errorCode, LogLevel? logLevel = null,
        params object[] parameters)
        : base(null, innerException)
    {
        _errorCode = errorCode;
        _parameters = parameters;
        _logLevel = logLevel;
    }

    public RaccoonException(string message, Exception? innerException, string errorCode, LogLevel? logLevel = null,
        params object[] parameters)
        : base(message, innerException)
    {
        _errorCode = errorCode;
        _parameters = parameters;
        _logLevel = logLevel;
    }

    public RaccoonException(string message, Exception? innerException, LogLevel? logLevel = null)
        : base(message, innerException)
    {
        _logLevel = logLevel;
    }

    protected RaccoonException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    private string GetLocalizedMessage()
    {
        if (string.IsNullOrWhiteSpace(ErrorCode))
            return base.Message;

        return GetLocalizedMessageExecuting();
    }

    protected virtual string GetLocalizedMessageExecuting()
    {
        if (!SupportI18n)
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                return base.Message;

            var parameters = GetParameters();
            if (parameters != null! && parameters.Length != 0)
                return string.Format(ErrorMessage, GetParameters());

            return ErrorMessage;
        }

        if (ErrorCode!.StartsWith(ExceptionErrorCode.FRAMEWORK_PREFIX))
        {
            //The current framework frame exception
            // return FrameworkI18n!.T(ErrorCode!, false, GetParameters()) ?? base.Message;
        }

        return base.Message;
    }

    protected virtual object[] GetParameters() => Parameters;

    public string? GetErrorMessage() => ErrorMessage;
}