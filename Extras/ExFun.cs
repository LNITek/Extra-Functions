using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Documents;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Extra Functions To Fill Every Need
    /// </summary>
    public static class ExFun
    {
        /// <summary>
        /// Find And Copy Files.
        /// </summary>
        /// <param name="ExistingFile">Where The File Is Ment To B.</param>
        /// <param name="ReplacementFile">Where The Replacement File Is</param>
        /// <param name="Overwrite">True: Will Rewrite The File If It Exists | False: Won't</param>
        /// <returns>True Whether The File Exist Or Copied, False If Copy Was Unsuccessful</returns>
        public static bool LoadFile(string ExistingFile, string ReplacementFile, bool Overwrite = false)
        {
            //var
            bool bExist = !Overwrite;
            string[] arrDir = ExistingFile.Split('\\');
            string Dr = string.Join("\\", arrDir.Where(x => x != arrDir.Last()));
            //Code
            if (!Directory.Exists(Dr))
                Directory.CreateDirectory(Dr);

            if (!Overwrite)
                bExist = File.Exists(ExistingFile);

            if (!bExist)
                try
                {
                    File.Copy(ReplacementFile, ExistingFile, true);
                }
                catch { return false; }

            return true;
        }

        /// <summary>
        /// Find And Copy Files.
        /// </summary>
        /// <param name="SourceDir">The Source Directory To Be Copied</param>
        /// <param name="DestDir">Where The Directory Sould Be Pasted At</param>
        /// <param name="CopySubDirs">Wheter To Copy All Its SubDirs And There Content</param>
        public static void CopyDir(string SourceDir, string DestDir, bool CopySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(SourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(SourceDir + " : Does Not Exist");

            DirectoryInfo[] dirs = dir.GetDirectories();    
            Directory.CreateDirectory(DestDir);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(DestDir, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (CopySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(DestDir, subdir.Name);
                    CopyDir(subdir.FullName, tempPath, CopySubDirs);
                }
            }
        }

        /// <summary>
        /// Test If Your Connected To The Internet.
        /// Test If A Connection To A Spesific Services Or Server Is Active
        /// </summary>
        /// <param name="Timeout">Will Return False After A Specified Time (default Is 1 Min)</param>
        /// <param name="URL">The URL To Use When Testing A Connection</param>
        /// <returns>True If Connection Was Successful. False If Not</returns>
        public static bool ConnectionChecker(TimeSpan Timeout = default, string URL = "www.google.com")
        {
            if (Timeout == default)
                Timeout = new TimeSpan(0, 1, 0);
            try
            {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                var request = (HttpWebRequest)WebRequest.Create(URL);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                request.KeepAlive = false;
                request.Timeout = (int)Timeout.TotalMilliseconds;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Converts A List To ObservableCollection
        /// </summary>
        /// <param name="list">The List To Convert</param>
        /// <returns>The List As A ObservableCollection</returns>
        public static ObservableCollection<T> ListToObservableCollection<T>(List<T> list)
        {
            var ConList = new ObservableCollection<T>();
            list.ForEach(x => ConList.Add(x));
            return ConList;
        }

        /// <summary>
        /// Converts A List To ObservableCollection
        /// </summary>
        /// <param name="list">The List To Convert</param>
        /// <param name="ConList">The ObservableCollection To Convert The List To</param>
        public static void ListToObservableCollection<T>(List<T> list, ref ObservableCollection<T> ConList)
        {
            ConList.Clear();
            foreach (var Item in list)
                ConList.Add(Item);
        }

        /// <summary>
        /// Gets The Index Of The Dictionary By Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dictionary">The Dictionary To Find Index</param>
        /// <param name="Key">The Key To Get Index</param>
        /// <returns>The Dictionary Index</returns>
        public static int IndexOfDictionary<T>(Dictionary<T, object> Dictionary, T Key) =>
            Dictionary.Keys.ToList().IndexOf(Key);

        /// <summary>
        /// Gets The Index Of The Dictionary By Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dictionary">The Dictionary To Find Index</param>
        /// <param name="Value">The Value To Get Index</param>
        /// <returns>The Dictionary Index</returns>
        public static int IndexOfDictionary<T>(Dictionary<object, T> Dictionary, T Value) =>
            Dictionary.Values.ToList().IndexOf(Value);

        /// <summary>
        /// Gets The Name Of The Week
        /// </summary>
        /// <param name="Week">The Week</param>
        /// <param name="Long">
        /// True To Return The Full Name Of The Week.
        /// False To Return Shorted Name Of The Week
        /// </param>
        /// <returns>The Name Of The Week</returns>
        public static string ToWeek(DateTime Week, bool Long = false) =>
            ToWeek((int)Week.DayOfWeek, Long);

        /// <summary>
        /// Gets The Name Of The Week
        /// </summary>
        /// <param name="Week">The Week</param>
        /// <param name="Long">
        /// True To Return The Full Name Of The Week.
        /// False To Return Shorted Name Of The Week
        /// </param>
        /// <returns>The Name Of The Week</returns>
        public static string ToWeek(int Week, bool Long = false)
        {
            Dictionary<string, string> DayesOfWeek = new Dictionary<string, string>()
            {
                {"Sun","Sunday" },
                {"Mon","Monday" },
                {"Tue","Tuesday" },
                {"Wen","Wednesday" },
                {"Thu","Thursday" },
                {"Fry","Friday" },
                {"Sat","Saturday" },
            };
            if (Week < 0 || Week >= DayesOfWeek.Count) throw new IndexOutOfRangeException(Week + " : Value Is Out Of Range");
            return Long ? DayesOfWeek.ElementAt(Week).Value : DayesOfWeek.ElementAt(Week).Key;
        }

        /// <summary>
        /// Gets The Name Of The Month
        /// </summary>
        /// <param name="Month">The Month</param>
        /// <param name="Long">
        /// True To Return The Full Name Of The Month.
        /// False To Return Shorted Name Of The Month
        /// </param>
        /// <returns>The Name Of The Month</returns>
        public static string ToMonth(DateTime Month, bool Long = false) =>
            ToMonth(Month.Month, Long);

        /// <summary>
        /// Gets The Name Of The Month
        /// </summary>
        /// <param name="Month">The Month</param>
        /// <param name="Long">
        /// True To Return The Full Name Of The Month.
        /// False To Return Shorted Name Of The Month
        /// </param>
        /// <returns>The Name Of The Month</returns>
        public static string ToMonth(int Month, bool Long = false)
        {
            Dictionary<string, string> DatesOfMonth = new Dictionary<string, string>()
            {
                {"Dec ","December" },
                {"Jan","January" },
                {"Feb","February" },
                {"Mar","March" },
                {"Apr","April" },
                {"May","May" },
                {"Jun","June" },
                {"Jul","July" },
                {"Aug","August" },
                {"Set","Setember" },
                {"Oct","October" },
                {"Nov","November" },
                {"Dec","December" },
            };
            if (Month < 0 || Month >= DatesOfMonth.Count) throw new IndexOutOfRangeException(Month + " : Value Is Out Of Range");
            return Long ? DatesOfMonth.ElementAt(Month).Value.Trim() : DatesOfMonth.ElementAt(Month).Key;
        }
    }
}
