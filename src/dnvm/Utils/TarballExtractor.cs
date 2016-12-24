using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DotNet.Utils
{
    public class TarballExtractor
    {
        public static bool Extract(string file, string destination, bool gzipped)
        {
            var args = new List<string>();
            args.Add("-x");
            if (gzipped)
            {
                args.Add("-z");
            }
            args.Add("-f");
            args.Add(file);

            using (var tmp = new TempDir())
            {
                args.Add("-C");
                args.Add(tmp.Path);

                var psi = new ProcessStartInfo()
                {
                    FileName = "tar",
                    Arguments = ArgumentEscaper.EscapeAndConcatenate(args),
                    UseShellExecute = false,
                };

                var process = Process.Start(psi);
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    return false;
                }

                foreach (var item in new DirectoryInfo(tmp.Path).EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    var newPath = Path.Combine(
                            destination,
                            item.FullName
                                .Replace(tmp.Path, "")
                                .TrimStart(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }));

                    var parentDir = Path.GetDirectoryName(newPath);
                    Directory.CreateDirectory(parentDir);
                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                    }
                    File.Move(item.FullName, newPath);
                }

                return true;
            }
        }
    }
}