namespace test.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "_")]
public class TestSetupException(string message) : Exception(message)
{
}