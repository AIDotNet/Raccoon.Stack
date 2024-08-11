namespace Raccoon.Stack.Configuration;

public class VariableInfo
{
    public string Key { get; set; }

    public string Variable { get; set; }

    public string DefaultValue { get; set; }

    public VariableInfo(string key, string variable, string defaultValue)
    {
        Key = key;
        Variable = variable;
        DefaultValue = defaultValue;
    }
}
