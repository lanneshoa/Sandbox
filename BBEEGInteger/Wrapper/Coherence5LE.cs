using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using BBEEGInteger.Log;
//using BBEEGInteger.EEG;
//using System.Runtime.ExceptionServices;

namespace BBEEGInteger.Wrapper
{
    public class Coherence5LE
    {
        // Use DllImport to import the Win32 MessageBox function.
        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode)]
        public static extern int _Eeg3_Initialisation();

        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_Unlock")]
        public static extern int _Eeg3_Unlock(TUnlock3LE Unlock3LE);


        //public static extern int Eeg3_Initialisation ();	// 1
        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_Termination")]
        public static extern int _Eeg3_Termination();	// 2
        //public static extern int Eeg3_Version (TVersion version);	// 3

        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_OpenFile")]
        public static extern int _Eeg3_OpenFile([MarshalAs(UnmanagedType.LPStr)] string FileName, IntPtr aCoh3);	// 4

        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_CloseFile")]
        public static extern int _Eeg3_CloseFile();	// 5

        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_GetEeg2", CallingConvention=CallingConvention.Cdecl)]
        public static extern int _Eeg3_GetEeg([MarshalAs(UnmanagedType.I4)] int begin, [MarshalAs(UnmanagedType.I4)] int duration, IntPtr Hbuf);	// 6
        //public static extern int Eeg3_PutMarker (TMarker evtle);	// 7


        //[DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_GetMarkers")]
        //public static extern int _Eeg3_GetMarkers (int begin, int end, IntPtr Hbuf);	// 8

        //[DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_GetImpedances")]
        //public static extern int _Eeg3_GetImpedances(ref int pos, IntPtr imped);	// 9
        //public static extern int Eeg3_Unlock(TUnlock3LE Unlock3LE);	// 10
        //public static extern int Eeg3_DebugFileSwitch(bool CreateDbgFile);                            	// 11

        [DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_NextFile")]
        public static extern int _Eeg3_NextFile(int direction, [MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr acoh3le);	// 12

        //[DllImport("Coherence5LE.dll", CharSet = CharSet.Unicode, EntryPoint = "_Eeg3_GetEeg2")]
        //[return: MarshalAs(UnmanagedType.I4)]
        //public static extern int _Eeg3_GetEeg2(int debut, int duree,  IntPtr PBuf);	// 13

        //public static extern int Eeg3_GetMarkers2(int begin, int end, Tmarker *Pevt);	// 14
        //public static extern int Eeg3_GetMarkersNumber(int begin, int end);   	// 15
        //public static extern int Eeg3_GetPatientInfo(TPatientInfo3LE *infopat);                        	// 16
        //public static extern int Eeg3_GetNumberOfBlocFiles(char *FileName);                            	// 17   
        //public static extern int Eeg3_GetAvailableBlocFiles(char* FileName, String* BlocFiles);	// 18
        //public static extern int Eeg3_ModeDoubleEventFile(bool mode);                                 	 // 19  
        //public static extern int Eeg3_GetNextMarker(int debut,TMarker *evtle);                 	 // 20  
        //public static extern int Eeg3_TempFolderSwitch(bool CreateTmpFolder);                          	 // 21  
        //public static extern int Eeg3_GetMarkersLong(int debut, int fin, HGLOBAL *Hbuf);  // 22       
        //public static extern int Eeg3_GetMarkersLong2(int debut, int fin, TMarkerLong *evtle); //23  
        //public static extern int Eeg3_GetNextMarkerLong(int debut,TMarkerLong *evtle);  	// 24    
        //public static extern int Eeg3_GetRealTimeMarkers(HGLOBAL *HrealTimeMrk);                      	// 25    
        //public static extern int Eeg3_GetNumberOfRealTimeMarkers();                                      	// 26    
        //public static extern int Eeg3_GetRealTimeMarkers2(TRealTimeMarker *realTimeMrk);               


        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        public const Int32 GMEM_MOVEABLE = 0x0002;
        public const Int32 GMEM_ZEROINIT = 0x0040;
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);


        // Construct the binary file representing EEG data
        public static void SaveFile(FileData fileData)
        {
            DateTime now = DateTime.Now;
            Uri uri = new Uri(fileData.path);
            string name = Path.GetFileName(fileData.path);
            name = name.Substring(0, name.IndexOf('.'));

            string filename = string.Format("{0}{1}{2}_{3}{4}{5}_{6}_{7}", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond, name );
            
            string filenameEEG = string.Format("{0}.{1}", filename, "eeg");
            string filenameJSON = string.Format("{0}.{1}", filename, "txt");

            string savedFileFolder = System.Configuration.ConfigurationSettings.AppSettings["SavedFileFolder"];


            //Create EEG file
            if (File.Exists(filenameEEG))
                File.Delete(filenameEEG);

            //StreamWriter sw = new StreamWriter(string.Format("{0}/{1}", savedFileFolder, filenameEEG));//création du fichier

            //for (int i = 0; i < fileData.eegMatrice.GetLength(0); i++)
            //{
            //    for (int j = 0; j < fileData.eegMatrice.GetLength(1); j++)
            //    {

            //        sw.Write(string.Format("{0} ", fileData.eegMatrice[i, j].ToString()));

            //    }
            //}


            using (FileStream fs = new FileStream(string.Format("{0}/{1}", savedFileFolder, filenameEEG), FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    for (int i = 0; i < fileData.eegMatrice.GetLength(0); i++)
                    {
                        for (int j = 0; j < fileData.eegMatrice.GetLength(1); j++)
                        {
                            bw.Write(fileData.eegMatrice[i, j]);
                        }
                    }
                }
            }
    


            //sw.Close();

            //Create JSON file
            if (File.Exists(filenameJSON))
                File.Delete(filenameJSON);

            StreamWriter sw = new StreamWriter(string.Format("{0}/{1}", savedFileFolder, filenameJSON));//création du fichier
            sw.Write(string.Format("{0} ", fileData.metaDataJSON.ToString()));
            sw.Close();
        }

        public static FileData OpenFile(string path)
        {
            //Set Unlock Code
            TUnlock3LE unlockCode = new TUnlock3LE();
            unlockCode.int1 = 0;
            unlockCode.int2 = 0;
            unlockCode.int3 = 0;
            unlockCode.int4 = 0;

            // Create the result
            FileData fileData = new FileData();

            try
            {
                // Init DLL
                if (_Eeg3_Initialisation() == 0)
                {
                    // Unlock DLL
                    if (_Eeg3_Unlock(unlockCode) == 0)
                    {
                        // Open the File
                        fileData = Eeg3_OpenFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError.Write(ex);
            }
            finally
            {
                // Close File
                _Eeg3_CloseFile();
                // Stop Using DLL
                _Eeg3_Termination();
            }

            return fileData;
        }

        public static FileData Eeg3_OpenFile(String path)
        {
            TCoh3 metaInfo = ReadMetaInfo(path);
            int nbValues = metaInfo.duration * metaInfo.frequency;

            short[,] eegMatrice = ReadEEG(nbValues, metaInfo.electrodes);

                
            //eegMatrice = null;

            FileNext();


            FileData fileData = new FileData { metaData = metaInfo, eegMatrice = eegMatrice, path=path };
            return fileData;
        }

        public static TCoh3 ReadMetaInfo(string path)
        {
            // Create a new empty struct TCoh3
            // and set a ptr on it
            TCoh3 tcoh = new TCoh3();
            IntPtr unmanagedAddr2 = Marshal.AllocHGlobal(Marshal.SizeOf(tcoh));
            Marshal.StructureToPtr(tcoh, unmanagedAddr2, false);

            int resp = _Eeg3_OpenFile(path, unmanagedAddr2);

            TCoh3 tcohres = (TCoh3)Marshal.PtrToStructure(unmanagedAddr2, typeof(TCoh3));

            // Unallocate memory
            Marshal.FreeHGlobal(unmanagedAddr2);
            unmanagedAddr2 = IntPtr.Zero;

            return tcohres;
        }

        public static string FileNext()
        {
            string path = "";

            // Create a new empty struct TCoh3
            // and set a ptr on it
            TCoh3 tcoh = new TCoh3();
            IntPtr unmanagedAddr2 = Marshal.AllocHGlobal(Marshal.SizeOf(tcoh));
            Marshal.StructureToPtr(tcoh, unmanagedAddr2, false);

            int resp = _Eeg3_NextFile(1, path, unmanagedAddr2);

            TCoh3 tcohres = (TCoh3)Marshal.PtrToStructure(unmanagedAddr2, typeof(TCoh3));

            // Unallocate memory
            Marshal.FreeHGlobal(unmanagedAddr2);
            unmanagedAddr2 = IntPtr.Zero;

            return path;
        }

        //[HandleProcessCorruptedStateExceptions]
        public static short[,] ReadEEG(int nbValues, int nbElectrodes)
        {
            int resultRequest=0;
            int start = 0;
            int bufferSize = nbElectrodes;
            short[,] result = new short[nbElectrodes,nbValues];

            StringBuilder res = new StringBuilder();
            //IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            IntPtr hbuffer = GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, bufferSize);
            IntPtr buffer = GlobalLock(hbuffer);

            bool finalLoop = false;
            int j = 0;

            do
            {

                if (start == (nbValues-1)  )
                {
                    //start = nbValues;
                    finalLoop = true;
                }
                
                resultRequest = _Eeg3_GetEeg(start, bufferSize, buffer);
                
                short[] resultTemp = new short[bufferSize];
                Marshal.Copy(buffer, resultTemp, 0, bufferSize);


                int i = 0;
                foreach (short data in resultTemp)
                {
                    result[i, j] = data;

                    if (i == nbElectrodes - 1)
                    {

                        //res.AppendLine(data.ToString() + ";");
                        i = 0;
                        j++;
                    }
                    else
                    {
                        //if (i == 0)
                        //    res.AppendLine(data.ToString() + ";");
                        //res.Append(data.ToString() + ";");
                        i++;
                    }

                }

                //if (!finalLoop)
                start = start +1;
                //else
                //{
                //    //If this is the final loop, take the rest
                //    start += resultRequest;
                //    //Init the right buffer size
                //    bufferSize = resultRequest;
                //}

            } while (!finalLoop);

            bool response = GlobalUnlock(hbuffer);
            // Unallocate memory
            //Marshal.FreeHGlobal(buffer);
            //buffer = IntPtr.Zero;

            return result;
        }

    }
}

