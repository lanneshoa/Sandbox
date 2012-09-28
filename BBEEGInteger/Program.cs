using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBEEGInteger.Wrapper;
using BBEEGInteger.EEG;
using System.IO;
using BBEEGInteger.Log;
using BBEEGInteger.Upload;
using Ionic.Zip;

namespace BBEEGInteger
{
    class Program
    {
        static void Main(string[] args)
        {
            //Convert files from DELTAMED to BBEEG
            ConvertFiles();
            //Send files via http
            //SendFiles();
            //Compressed all files 
            BackupFiles();
            
        }

        private static void ConvertFiles()
        {
            try
            {
                Console.WriteLine("LOOKING FOR DELTA EEG FILES TO CONVERT...");
                string[] files = System.IO.Directory.GetFiles(System.Configuration.ConfigurationSettings.AppSettings["DeltaEEGFileFolder"]);
                Console.WriteLine(string.Format("{0} {1}", files.Length, "FILES"));

                foreach (string path in files)
                {
                    Uri uri = new Uri(path);

                    Console.WriteLine(string.Format("{0} {1}", path, "processed..."));

                    FileData fileData = Coherence5LE.OpenFile(path);
                    Coherence5LE.SaveFile(fileData);

                    string destPath = string.Format(@"{0}{1}", System.Configuration.ConfigurationSettings.AppSettings["BACKUPDeltaEEGFileFolder"], uri.Segments[uri.Segments.Length - 1]);

                    if (File.Exists(destPath))
                        File.Delete(destPath);

                    File.Move(path, destPath);
                }

                if (files.Length > 0)
                    LogError.Write(string.Format("{0} : {1} {2}", DateTime.Now.ToShortTimeString(), files.Length, "files processed"));

            }
            catch (Exception ex)
            {
                LogError.Write(ex);
            }
        }

        private static void SendFiles()
        {
            try
            {
                Console.WriteLine("LOOKING FOR BBEEG FILES TO SEND VIA HTTP...");
                string[] files = System.IO.Directory.GetFiles(System.Configuration.ConfigurationSettings.AppSettings["SavedFileFolder"]);
                Console.WriteLine(string.Format("{0} {1}", files.Length, "FILES"));

                foreach (string path in files)
                {
                    Uri uri = new Uri(path);

                    Network.Send(path);
                }

                if (files.Length > 0)
                    LogError.Write(string.Format("{0} : {1} {2}", DateTime.Now.ToShortTimeString(), files.Length, "files sent"));

            }
            catch (Exception ex)
            {
                LogError.Write(ex);
            }
        }

        private static void BackupFiles()
        {
            //If it's the first day of a month
            //Compress old files
            if (DateTime.Today.Day == 1)
            {
                using (ZipFile zip = new ZipFile())
                {
                    foreach (string file in Directory.GetFiles(System.Configuration.ConfigurationSettings.AppSettings["BACKUPDeltaEEGFileFolder"]))
                    {
                        if (Path.GetExtension(file) != ".zip")
                            // add the report into a different directory in the archive
                            zip.AddFile(file);
                        else
                        {

                        }
                    }
                    string dest = string.Format("{0}{1}.zip", System.Configuration.ConfigurationSettings.AppSettings["BACKUPDeltaEEGFileFolder"], DateTime.Today.AddDays(-1).ToShortDateString().Replace("/", string.Empty));
                    zip.Save(dest);
                }
            }

        }
    }
}
