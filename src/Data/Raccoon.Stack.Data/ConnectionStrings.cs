﻿using System.Runtime.Serialization;

namespace Raccoon.Stack.Data;

[Serializable]
public class ConnectionStrings : Dictionary<string, string>
{
    public const string DEFAULT_SECTION = "ConnectionStrings";

    public const string DEFAULT_CONNECTION_STRING_NAME = "DefaultConnection";

    public string DefaultConnection
    {
        get => GetConnectionString(DEFAULT_CONNECTION_STRING_NAME);
        set => this[DEFAULT_CONNECTION_STRING_NAME] = value;
    }

    public ConnectionStrings() { }

    public ConnectionStrings(IEnumerable<KeyValuePair<string, string>> connectionStrings)
        : base(connectionStrings) { }

    protected ConnectionStrings(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {

    }

    public string GetConnectionString(string name)
    {
        if (base.TryGetValue(name, out var connectionString))
            return connectionString;

        return string.Empty;
    }
}
