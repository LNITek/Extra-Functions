using System;
using System.IO;
using System.Linq;
using System.Net;

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
            string[] arrDir = ExistingFile.Split("\\");
            string Dr = string.Join("\\", arrDir[..(arrDir.Length - 1)]);
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
            DirectoryInfo dir = new(SourceDir);

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
                using var response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch { return false; }
        }
    }
}
