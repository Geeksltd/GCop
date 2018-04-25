using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

var folder = new DirectoryInfo(@"./Rules");
var filez = Reader.RemoveSuffix(Reader.RemovePrefix(Reader.ReadFileNames(folder,"_Template.md"),"GCop"),".md");
var lines = MDEngine.GetLines(new FileInfo(@"Rules\GCop101.md"));
Writer.WriteTable(new FileInfo(@"docs\Rules.md"), "/Rules/", filez);
var files = Reader.RemoveSuffix(Reader.ReadFileNames(folder,"_Template.md"),".md");
Writer.WriteList(new FileInfo(@"_sidebar.md"), "/Rules/", files);

        

    public static class Reader
    {
        public static List<FileItem> ReadFileNames(DirectoryInfo dir, params string[] ignore)
        {
            var filez = ReadFileNames(dir);
            foreach (var item in ignore)
                filez.RemoveAll(x => x.Name == item);

            return filez;
        }

        public static List<FileItem> ReadFileNames(DirectoryInfo dir)
        {
            var files = new List<FileItem>();
            foreach (var file in dir.GetFiles())
                files.Add(new FileItem(file.Name, file));

            return files;
        }

        public static List<FileItem> RemovePrefix(List<FileItem> strings, string prefix)
        {
            var result = new List<FileItem>();
            foreach (var item in strings)
            {
                item.Name = item.Name.Substring(prefix.Length);
                result.Add(item);
            }
            return result;
        }

        public static List<FileItem> RemoveSuffix(List<FileItem> strings, string suffix)
        {
            var result = new List<FileItem>();
            foreach (var item in strings)
            {
                item.Name = item.Name.Substring(0, item.Name.Length - suffix.Length);
                result.Add(item);
            }
            return result;
        }
    }
    public static class Writer
    {
        public static void WriteList(FileInfo dest, string relativePath, List<FileItem> contents)
        {
            string initString =
 @"* [Rules List](docs/Rules.md)
* [Template](Rules/_Template.md)
* Rules
";

            var str = BuildListString(relativePath, contents, initString);
            using (var writer = new StreamWriter(dest.FullName))
            {
                writer.Write(str);
            }
        }

        private static string BuildListString(string relativePath, List<FileItem> contents, string initString)
        {
            var builder = new StringBuilder();
            builder.Append(initString);
            foreach (var content in contents)
            {
                builder.AppendLine($"   * [{content.Name}]({relativePath + content.File.Name})");
            }
            return builder.ToString();
        }

        public static void WriteTable(FileInfo dest, string relativePath, List<FileItem> contents)
        {
            string initString =
 @"# Error list
GCop code | Description
--- | ---
";
            var str = BuildTableString(relativePath, contents, initString);
            using (var writer = new StreamWriter(dest.FullName))
            {
                writer.Write(str);
            }
        }
        static string BuildTableString(string relativePath, List<FileItem> contents, string initString)
        {
            var builder = new StringBuilder();
            builder.Append(initString);
            foreach (var content in contents)
            {
                builder.AppendLine($"{content.Name} | [{MDEngine.ParseTitle(content.File)}]({relativePath + content.File.Name})");
            }
            return builder.ToString();

        }

    }
    public static class MDEngine
    {
        public static string GetLines(FileInfo path)
        {
            using (StreamReader srt = new StreamReader(path.FullName))
            {
                return srt.ReadToEnd();
            }
        }

        public static string ParseTitle(FileInfo path) // =>GetLine(text, 3).Substring(4).Substring(GetLine(text, 3).Substring(4).Length - 2);
        {

            var x = GetLine(MDEngine.GetLines(path), 3);
            var z = x.Substring(4);
            var y = z.Substring(0, z.Length - 2);
            return y;
        }


        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }
    }
    public class FileItem
    {
        public FileItem(string name, FileInfo file)
        {
            Name = name;
            File = file;
        }
        public string Name { get; set; }
        public FileInfo File { get; set; }
    }
