using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;

namespace Synth.Logging
{
  public static class LoggingConfigurator
  {
    public static void Setup(string fileName, string level)
    {
      LogLevel logLevel = LogLevel.Fatal;
      switch (level.ToLower())
      {
        case "trace":
          logLevel = LogLevel.Trace;
          break;
        case "debug":
          logLevel = LogLevel.Debug;
          break;
        case "info":
          logLevel = LogLevel.Info;
          break;
        case "warn":
          logLevel = LogLevel.Info;
          break;
        case "error":
          logLevel = LogLevel.Error;
          break;
        case "fatal":
          logLevel = LogLevel.Fatal;
          break;
      }

      var config = new NLog.Config.LoggingConfiguration();

      string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "City of Portland", "BES-ASM", "Synth");
      if (!Directory.Exists(logDir))
      {
        Directory.CreateDirectory(
          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          "City of Portland", "BES-ASM", "Synth"));
      }

      var fileTarget = new NLog.Targets.FileTarget { FileName = Path.Combine(logDir, fileName) };
      config.AddTarget("logFile", fileTarget);
      var fileRule = new NLog.Config.LoggingRule("*", logLevel, fileTarget);
      config.LoggingRules.Add(fileRule);

      LogManager.Configuration = config;
    }
  }
}
