using System.ComponentModel;

namespace Raccoon.Stack.Data.Constants;

public static class ExceptionErrorCode
{
    public const string FRAMEWORK_PREFIX = "Raccoon";

    private const string ARGUMENT = $"{FRAMEWORK_PREFIX}ARG";

    /// <summary>
    /// '{PropertyName}' cannot be Null or empty collection.
    /// </summary>
    [Description("'{0}' cannot be Null or empty collection.")]
    public const string NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR = $"{ARGUMENT}0029";
    
    [Description("'{0}' must not be empty.")]
    public const string NOT_NULL_VALIDATOR = $"{ARGUMENT}0011";
    
    
    [Description("'{0}' cannot be null and empty.")]
    public const string NOT_NULL_AND_EMPTY_VALIDATOR = $"{ARGUMENT}0019";
    
    
    [Description("'{0}' cannot be Null or whitespace.")]
    public const string NOT_NULL_AND_WHITESPACE_VALIDATOR = $"{ARGUMENT}0030";
    
    
    [Description("'{0}' must be less than or equal to '{1}'.")]
    public const string LESS_THAN_OR_EQUAL_VALIDATOR = $"{ARGUMENT}0007";
    
    [Description("'{0}' must be less than '{1}'.")]
    public const string LESS_THAN_VALIDATOR = $"{ARGUMENT}0008";
    
    
    [Description("'{0}' must be greater than or equal to '{1}'.")]
    public const string GREATER_THAN_OR_EQUAL_VALIDATOR = $"{ARGUMENT}0002";
    
    
    [Description("'{0}' must be greater than '{1}'.")]
    public const string GREATER_THAN_VALIDATOR = $"{ARGUMENT}0003";
    
    [Description("'{0}' must be greater than or equal to '{1}' and less than or equal to '{2}'.")]
    public const string OUT_OF_RANGE_VALIDATOR = $"{ARGUMENT}0032";
    
    
    [Description("'{0}' cannot contain {1}.")]
    public const string NOT_CONTAIN_VALIDATOR = $"{ARGUMENT}0031";
    
    private static readonly Dictionary<string, string?> ErrorCodeMessageDictionary = new();
    
    public static string? GetErrorMessage(string errorCode)
    {
        return ErrorCodeMessageDictionary.GetValueOrDefault(errorCode);
    }
}