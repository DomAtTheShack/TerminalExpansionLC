// Define the TerminalCommand attribute

using System;

[AttributeUsage(AttributeTargets.Method)]
public class TerminalCommandAttribute : Attribute
{
    public string CommandName { get; }

    public TerminalCommandAttribute(string commandName)
    {
        CommandName = commandName;
    }
}

// Define the AllowedCaller attribute
[AttributeUsage(AttributeTargets.Method)]
public class AllowedCallerAttribute : Attribute
{
    public AllowedCaller Caller { get; }

    public AllowedCallerAttribute(AllowedCaller caller)
    {
        Caller = caller;
    }
}

// Define the AllowedCaller enum
public enum AllowedCaller
{
    Host,
    Client,
    // Add other caller types as needed
}