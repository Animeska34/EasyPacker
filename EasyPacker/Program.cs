using System;
using System.Collections.Generic;
using System.IO;

namespace EasyPacker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Boolean unpack = true;
                List<String> files = new List<string>();

                foreach (var item in args)
                {
                    var a = File.GetAttributes(item);
                    if (a.HasFlag(FileAttributes.Directory))
                    {
                        Path.GetFileName(item);

                        string[] entries = Directory.GetFileSystemEntries(item, "*", SearchOption.AllDirectories);
                        foreach (var entry in entries)
                        {
                            var aa = File.GetAttributes(entry);
                            if (!aa.HasFlag(FileAttributes.Directory))
                                files.Add(entry.Replace(item, Path.GetFileName(item)));
                        }
                        unpack = false;
                    }
                    else
                    {
                        if (!item.EndsWith(".pack"))
                        {
                            unpack = false;
                        }
                        if (File.Exists(item))
                            files.Add(Path.GetFileName(item));
                    }
                }

                if (files.Count > 0)
                {
                    if (unpack)
                    {
                        ulong fileCount = 0;
                        foreach (var item in files)
                        {
                            Pack.PackReader.UnpackFiles(item, ref fileCount, true);
                        }
                    }
                    else
                    {
                        String[] fileString = files.ToArray();
                        Pack.PackWriter.PackFiles(args[0] + ".pack", fileString, true);
                    }
                }

            }
            else
            {
                Console.WriteLine("Usage:\nDrag'n drop .pack archives on executable to unpack,\nor drop other files and folders to pack");

            }
            Console.ReadKey();
        }
    }
}
