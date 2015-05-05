using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Synth.Logging
{
  public class Logger : ILogger
  {
    NLog.Logger nLogger;

    public Logger(string typeName)
    {
      nLogger = NLog.LogManager.GetLogger(typeName);
    }

    public void Debug(string message)
    {
      nLogger.Debug(message);
    }

    public void Debug(string message, Exception ex)
    {
      nLogger.DebugException(message, ex);
    }

    public void Debug(string message, params object[] args)
    {
      nLogger.Debug(message, args);
    }

    public void Info(string message)
    {
      nLogger.Info(message);
    }

    public void Info(string message, Exception ex)
    {
      nLogger.InfoException(message, ex);
    }

    public void Info(string message, params object[] args)
    {
      nLogger.Info(message, args);
    }

    public void Warn(string message)
    {
      nLogger.Warn(message);
    }

    public void Warn(string message, Exception ex)
    {
      nLogger.WarnException(message, ex);
    }

    public void Warn(string message, params object[] args)
    {
      nLogger.Warn(message, args);
    }

    public void Error(string message)
    {
      nLogger.Error(message);
    }

    public void Error(string message, Exception ex)
    {
      nLogger.ErrorException(message, ex);
    }

    public void Error(string message, params object[] args)
    {
      nLogger.Error(message, args);
    }

    public void Fatal(string message)
    {
      nLogger.Fatal(message);
    }

    public void Fatal(string message, Exception ex)
    {
      nLogger.FatalException(message, ex);
    }

    public void Fatal(string message, params object[] args)
    {
      nLogger.Fatal(message, args);
    }

  }
}
