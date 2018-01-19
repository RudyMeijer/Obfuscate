//*****************************************************************************
//
//      Class:          WindowsAPI
//      Namespace:      Kerr.Resources
//      Date created:   23 November 2001
//      Copyright:      © Kenny Kerr 2001 (mailto:kennykerr@hotmail.com)
//
//*****************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Kerr.Resources
{
    /// <summary>
    /// Contains definitions of common unmanaged Win32 API functions, 
    /// constants, and structures. For more information see the Platform SDK
    /// documentation.
    /// </summary>
    internal class WindowsAPI
    {
        #region Functions

        /// <summary>
        /// ThrowLastError is a helper function to translate Win32 error codes
        /// into exceptions that can be handled by managed code. It is 
        /// conceptually the same as the following C++ code:
        /// 
        ///     throw HRESULT_FROM_WIN32(::GetLastError());
        /// 
        /// </summary>
        public static void ThrowLastError()
        {
            throw new System.ComponentModel.Win32Exception();
        }

        [DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern IntPtr LoadLibraryEx(string strFileName,
                                                  IntPtr hFile,
                                                  ulong ulFlags);

        [DllImport("Kernel32.dll", SetLastError=true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern IntPtr FindResource(IntPtr hModule,
                                                 IntPtr pName,
                                                 IntPtr pType);

        [DllImport("Kernel32.dll", SetLastError=true)]
        public static extern IntPtr LoadResource(IntPtr hModule,
                                                 IntPtr hResource);

        [DllImport("Kernel32.dll", SetLastError=true)]
        public static extern uint SizeofResource(IntPtr hModule,
                                                 IntPtr hResource);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hGlobal);

        [DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern int EnumResourceNames(IntPtr hModule,
                                                   IntPtr pType,
                                                   EnumResNameProc enumFunc,
                                                   IntPtr param);

        [DllImport("User32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern IntPtr LoadIcon(IntPtr hInstance,
                                             IntPtr pName);

        [DllImport("Shell32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr ExtractIcon(IntPtr hInstance,
                                                string strFileName,
                                                uint uiIconIndex);

        unsafe public static bool IS_INTRESOURCE(IntPtr wInteger)
        {
            uint uiInteger = (uint) wInteger.ToPointer();
            return ((uiInteger >> 16) == 0);
        }

        #endregion

        #region Definitions

        public static readonly IntPtr RT_ICON = new IntPtr(3);
        public static readonly IntPtr RT_GROUP_ICON = new IntPtr(14);

        public static readonly ulong LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        public delegate bool EnumResNameProc(IntPtr hModule,
                                             IntPtr pType,
                                             IntPtr pName,
                                             IntPtr param);

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors; // inline array
        }

        /// <summary>
        /// MEMICONDIRENTRY helps to defines the memory layout of a 
        /// RT_GROUP_ICON resource. In particular its wId member indicates the
        /// RT_ICON resource for the directory entry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack=2)]
        public struct MEMICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public ushort wId;
        }

        /// <summary>
        /// MEMICONDIR defines the memory layout of a RT_GROUP_ICON Win32
        /// resource.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack=2)]
        public struct MEMICONDIR
        {
            public ushort wReserved;
            public ushort wType;
            public ushort wCount;
            public MEMICONDIRENTRY arEntries; // inline array
        }

        /// <summary>
        /// FILEICONDIRENTRY defines the peristent format of an icon directory 
        /// entry in a .ICO file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FILEICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public uint dwImageOffset;
        }

        #endregion
    }
}
