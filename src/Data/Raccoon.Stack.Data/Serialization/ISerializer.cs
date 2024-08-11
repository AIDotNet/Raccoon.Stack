namespace Raccoon.Stack.Data;

public interface ISerializer
{
    string Serialize<TValue>(TValue value);
}