using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BBEEGInteger.EEG;

namespace BBEEGInteger.Wrapper
{
    


        public struct TUnlock3LE
        {
            [MarshalAs(UnmanagedType.I4)]
            public int int1;		// first integer	
            [MarshalAs(UnmanagedType.I4)]
            public int int2; 		// second integer	
            [MarshalAs(UnmanagedType.I4)]
            public int int3; 		// third integer		
            [MarshalAs(UnmanagedType.I4)]
            public int int4; 		// fourth integer	
        } ;

        public struct TVersion
        {
            public short major;	    // major
            public short minor;	    // minor version
            public short compile;	// compilation
            public short number;	// number
        } ;

        public struct TMarker
        {
            public int evttype;	    // type of the marker
            public int pos;	        // position
            public int duration;	// duration
            public char[] text;	    // comment
        } ;

        public struct TImpedances
        {
            public char[] text;	    // text associated to the impedance test
            public int pos;	        // position of the impedance test
            public int nbchannel;	// number of channels used in the impedance test
            public int[] imped;	    // impedances values (in Kohm, range : 0 to 250)
        } ;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TBuffer
        {
            [MarshalAs(UnmanagedType.I2)]
            public int value;	    
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TCoh3
        {
            public Int32 duration;	// duration of the record in seconds
            public Int32 frequency;	// sampling rate
            public Int32 timebase;	// sampling time base in seconds
            public Int32 electrodes;	// number of electrodes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public Char[] date;	    // date and time of the record (‘hh:mm:ss dd/mm/yyyy’)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public NameArray[] name;	// electrode names
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Char[] type;	    // electrode types
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] theta;	    // theta angular coordinate
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] phi;	    // phi angular coordinate
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] r;	        // radius
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] minanal;	// min analog value
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] maxanal;	// max analog value
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] minconv;	// min value of the converter
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] maxconv;	// max value of the converter
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public UnitArray[] unit;	// display unit (‘µV’, ‘mmHg’…)
        } ;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NameArray
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UnitArray
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] unit;
        }

        public struct TPatientInfo3LE
        {
            public char[] name;         // patient name
            public char[] firstname;    // patient first name
            public char[] date;         // patient date of birth
            public char sex;         	// patient first name
            public char[] file;        	// file number of recording
            public char[] center;       // origin of the recording
            public char[] comments;   	// commentary
        };

        public struct TMarkerLong
        {
            public int evttype;	    // type of the marker
            public int pos;	        // position
            public int duration;	// duration
            public char text;	    // comment
        } ;

        public struct TRealTimeMarker
        {
            public int pos; 		//Position of the real time marker in sample
            public int realtime; 	//Absolute real time at the specified position, in second
        };


        public struct FileData
        {
            public string path;
            public TCoh3 metaData;
            public short[,] eegMatrice;

            public string metaDataJSON 
            {
                get { 
                    EEGMetadata eegMetadata = new EEGMetadata();
                    eegMetadata.Parse(this.metaData);
                    return Serialize.ToJson(eegMetadata); 
                }
            }
        }
}
