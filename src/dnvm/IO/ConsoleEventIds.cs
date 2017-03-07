using System;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.IO
{
    public static class ConsoleEventIds
    {
        public const int ColorFlag = 1 << 4;

        public static EventId Green { get; } = new EventId(ColorFlag | (int)ConsoleColor.Green);
        public static EventId DarkGray { get; } = new EventId(ColorFlag | (int)ConsoleColor.DarkGray);
    }
}
