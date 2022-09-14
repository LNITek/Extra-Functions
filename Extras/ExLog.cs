using System.Collections.Generic;
using System.IO;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Extra Logs. Helps You Manige The Process Of Loging
    /// </summary>
    public class ExLog
    {
        List<FileType> SupportedTypes = new() { FileType.log, FileType.txt, FileType.dat };

        /// <summary>
        /// The Name Of The Log File
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Directory Of The Log File
        /// </summary>
        public string OutputDir { get; set; }
        /// <summary>
        /// Full Path Of The Log File
        /// </summary>
        public string OutputPath { get { return OutputDir + "\\" + Name + "." + FileType.ToString() ; } }
        FileType FileType { get; set; }
        /// <summary>
        /// Creates A Object To Manige Loging
        /// </summary>
        /// <param name="LogName">File Name</param>
        /// <param name="OutputDir">Output Directory</param>
        /// <param name="FileExtension">The File Extension To Use</param>
        public ExLog(string LogName, string OutputDir, FileType FileExtension = FileType.log)
        {
            if (!SupportedTypes.Contains(FileExtension)) 
                throw new FileFormatException(FileExtension.ToString() + " : Format Is Not Supported");
            Name = LogName;
            FileType = FileExtension;
            this.OutputDir = OutputDir;
        }

        /// <summary>
        /// Writes To The Log File
        /// </summary>
        /// <param name="Line">The Text To Write</param>
        public void Log(string Line)
        {
            StreamWriter sw = new(OutputPath, true);
            sw.WriteLine(Line);
            sw.Close();
        }

        /// <summary>
        /// Reads A Log File For You
        /// </summary>
        /// <param name="Path">Full Path To The Log File</param>
        /// <returns>Array Of Logs</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string[] ReadLog(string Path)
        {
            List<string> lines = new();
            if (!File.Exists(Path))
                throw new FileNotFoundException(Path + " : Does Not Exist.");
            StreamReader Reader = new(Path);
            while (!Reader.EndOfStream)
                lines.Add(Reader.ReadLine() ?? "");
            return lines.ToArray();
        }
    }
}
