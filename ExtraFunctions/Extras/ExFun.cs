using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Extra Functions To Fill Every Need!
    /// </summary>
    public static class ExFun
    {
        #region Extra Dates And Time Fun
        /// <summary>
        /// Gets The Name Of The Week
        /// </summary>
        /// <param name="Week">The Week</param>
        /// <param name="Long">
        /// True To Return The Full Name Of The Week.
        /// False To Return Shorted Name Of The Week
        /// </param>
        /// <returns>The Name Of The Week</returns>
        public static string ToWeek(this DateTime Week, bool Long = false) =>
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
            Dictionary<string, string> DayesOfWeek = new()
            {
                {"Sun","Sunday" },
                {"Mon","Monday" },
                {"Tue","Tuesday" },
                {"Wen","Wednesday" },
                {"Thu","Thursday" },
                {"Fry","Friday" },
                {"Sat","Saturday" },
            };
            if (Week < 0 || Week >= DayesOfWeek.Count) throw new ArgumentOutOfRangeException(nameof(Week), "Value Is Out Of Range : " + Week);
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
        public static string ToMonth(this DateTime Month, bool Long = false) =>
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
            Dictionary<string, string> DatesOfMonth = new()
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
            if (Month < 0 || Month >= DatesOfMonth.Count) throw new ArgumentOutOfRangeException(nameof(Month), "Value Is Out Of Range : " + Month);
            return Long ? DatesOfMonth.ElementAt(Month).Value.Trim() : DatesOfMonth.ElementAt(Month).Key;
        }
        #endregion
        #region Other Fun
        /// <summary>
        /// Find And Copy Files.
        /// </summary>
        /// <param name="ExistingFile">Where The File Is Ment To Be.</param>
        /// <param name="ReplacementFile">Where The Replacement File Is.</param>
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
        /// Test If A Connection To A Spesific Services Or Server Is Active.
        /// </summary>
        /// <param name="URL">The URL To Use When Testing A Connection</param>
        /// <param name="Timeout">Will Return False After A Specified Time (default Is 1 Min)</param>
        /// <returns>True If Connection Was Successful, Otherwise False.</returns>
        public static bool ConnectionChecker(string URL = "www.google.com", TimeSpan Timeout = default)
        {
            if (Timeout == default) Timeout = new TimeSpan(0, 1, 0);
            try
            {
                return new Ping().Send(URL, (int)Timeout.TotalMilliseconds)?.Status == IPStatus.Success;
            }
            catch { return false; }
        }

        /// <summary>
        /// Clears The Current Line In The Console.
        /// </summary> 
        [Obsolete("ClearCurrentConsoleLine is deprecated, please do not use this method.")]
        public static void ClearCurrentConsoleLine(int Offset = 0)
        {
            if (Offset < 0) throw new ArgumentOutOfRangeException(nameof(Offset), "Offset Can Not Be Less Than 0");
            Console.SetCursorPosition(0, Console.CursorTop -  Offset);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        #endregion
        #region Extra List Fun
#if NET6_0_OR_GREATER
        /// <summary>
        /// Converts A Int Range To A List Of Integers.
        /// </summary>
        /// <param name="range">The Range To Convert.</param>
        /// <returns>The List Of Integers As IEnumerable.</returns>
        public static IEnumerable<int> AsEnumerable(this Range range)
        {
            for(int I = range.Start.Value; I <= range.End.Value; I++)
                yield return I;
        }
#endif
        /// <summary>
        /// Converts An Ienumerble List To A ObservableCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <returns>An ObservabelCollection Containging Your Elements.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> Collection) =>
            new(Collection);

        /// <summary>
        /// Add a collection of values to an <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to add items to</param>
        /// <param name="values">The values to add to the collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            foreach (var item in values)
                collection.Add(item);
        }

        /// <summary>
        /// Gets The Index Of A Element In The Dictionary By Its Key.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Dictionary"></param>
        /// <param name="Key">The Key To Use For Finding Its Index.</param>
        /// <returns>The A 0 Zero-Based Index Of The Element, Otherwise -1.</returns>
        public static int IndexOf<T1,T2>(this Dictionary<T1, T2> Dictionary, T1 Key) =>
            Dictionary.Keys.ToList().IndexOf(Key);

        /// <summary>
        /// Gets The Index Of A Element In The Dictionary By Its Value.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Dictionary"></param>
        /// <param name="Value">The Value To Use For Finding Its Index.</param>
        /// <returns>The A 0 Zero-Based Index Of The Element, Otherwise -1.</returns>
        public static int IndexOf<T1, T2>(this Dictionary<T1, T2> Dictionary, T2 Value) =>
            Dictionary.Values.ToList().IndexOf(Value);

        /// <summary>
        /// A Clone From List.ForEach(). 
        /// Now Avalabel For All IEnumerable!
        /// </summary>
        /// <typeparam name="T">The Type Of Element In The List.</typeparam>
        /// <param name="List">The List To Preform The Looped Action On.</param>
        /// <param name="Action">The Action To Preform For Each Element.</param>
        public static void ForEach<T>( this IEnumerable<T> List, Action<T> Action)
        {
            if (Action == null)
                throw new ArgumentNullException(nameof(Action));

            for (int i = 0; i < List.Count(); i++)
                Action(List.ElementAt(i));
        }

        /// <summary>
        /// Converts A IEnumerble To ExList Object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ExList<T> ToExList<T>(this IEnumerable<T> list) => new(list);

        /// <summary>
        /// Gets The Index Of An Element In The IEnumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="Item">The Element To Locate.</param>
        /// <returns>The Index Of An Element, Otherwise -1</returns>
        public static int IndexOf<T>(this IEnumerable<T> list, T Item)
        {
            for (int I = 0; I < list.Count(); I++)
                if (list.ElementAt(I).Equals(Item)) return I;
            return -1;
        }
        #endregion
    }
}
