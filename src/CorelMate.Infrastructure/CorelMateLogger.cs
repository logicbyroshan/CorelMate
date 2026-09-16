using System;

namespace CorelMate.Infrastructure;

public interface ICorelMateLogger
{
    void Info(string message);
    void Error(string message, Exception exception);
}

public sealed class NullCorelMateLogger : ICorelMateLogger
{
    public void Info(string message) { }
    public void Error(string message, Exception exception) { }
}