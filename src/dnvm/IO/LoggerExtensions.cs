using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager
{
    public static class LoggerExtensions
    {
        public static ILogger Output(this ILogger log, string text)
        {
            log.LogInformation(text);
            return log;
        }

        public static ILogger Output(this ILogger log, EventId eventId, string text)
        {
            log.LogInformation(eventId, text);
            return log;
        }

        public static ILogger Verbose(this ILogger log, string text)
        {
            log.LogDebug(text);
            return log;
        }

        public static ILogger Error(this ILogger log, string text)
        {
            log.LogCritical(text);
            return log;
        }

        public static ILogger Warn(this ILogger log, string text)
        {
            log.LogWarning(text);
            return log;
        }

        [Conditional("DEBUG")]
        public static void Trace(this ILogger log, string text)
            => log.LogTrace(text);
    }
}
