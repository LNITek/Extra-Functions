using System;
using System.IO;
using System.Net;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Extra Functions To Fill Every Need.
    /// </summary>
    public static class ExFun
    {
        /// <summary>
        /// Find And Copy Files.
        /// </summary>
        /// <param name="Existingfile">Where The File Is Ment To Be.</param>
        /// <param name="Replacementfile">Where The Replacement File Is.</param>
        /// <param name="Overwrite">True: Will Rewrite The File If It Exists | False: Won't.</param>
        /// <returns></returns>
        public static bool LoadFile(string Existingfile, string Replacementfile, bool Overwrite = false)
        {
            //var
            bool bExist = !Overwrite, bFlag = true;
            //Code
            if (Overwrite == false)
                bExist = File.Exists(Existingfile);

            if (bExist == false)
                try
                {
                    File.Copy(Replacementfile, Existingfile, true);

                }
                catch { bFlag = false; }

            return bFlag;
        }

        /// <summary>
        /// Test If Your Connected To The Internet.
        /// Test If A Connection To A Spesific Services Or Server Is Active
        /// </summary>
        /// <param name="Timeout">Will Return False After A Specified Time (default Is 1 Min)</param>
        /// <param name="URL">The URL To Use When Testing A Connection</param>
        /// <returns></returns>
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
