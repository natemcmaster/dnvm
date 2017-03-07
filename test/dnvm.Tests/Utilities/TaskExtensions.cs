using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotNet.VersionManager.Tests
{
    public static class TaskExtensions
    {
        public static async Task OrTimeout(this Task task, int timeout,
            [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            if (task != await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(timeout))).ConfigureAwait(false))
            {
                throw new TimeoutException($"Task timed out after {timeout} seconds at {path}:{line}");
            }
        }

        public static async Task<T> OrTimeout<T>(this Task<T> task, int timeout,
            [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            await OrTimeout(task, timeout, path, line);
            return task.Result;
        }
    }
}
