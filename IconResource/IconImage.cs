//*****************************************************************************
//
//      Class:          IconImage
//      Namespace:      Kerr.Resources
//      Date created:   23 November 2001
//      Copyright:      © Kenny Kerr 2001 (mailto:kennykerr@hotmail.com)
//
//*****************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Kerr.Resources
{
    /// <summary>
    /// The IconImage class represents a single image in an IconResource.
    /// </summary>
	internal class IconImage
	{
        #region Constructors

        /// <summary>
        /// Creates a new IconImage object to represent a RT_ICON resource.
        /// </summary>
        /// <param name="library">The library that contains the resource.</param>
        /// <param name="pResourceName">The name of the resource.</param>
        unsafe public IconImage(Library library,
                                IntPtr pResourceName)
        {
            // Find the resource with the given name.

            IntPtr hIconInfo = library.FindResource(pResourceName,
                                                    WindowsAPI.RT_ICON);

            // Load the resource and get the resource bits.

            IntPtr hIconRes = library.LoadResource(hIconInfo);
            IntPtr pDibBits = Library.LockResource(hIconRes);

            // We make a local copy of the DIB bits as pDibBits may be 
            // freed when the module is unloaded.
			m_arDibBits = new byte[library.SizeofResource(hIconInfo)];
            Marshal.Copy(pDibBits, m_arDibBits, 0, m_arDibBits.Length);

            fixed (byte* pBytes = &m_arDibBits[0])
            {
                WindowsAPI.BITMAPINFO* pBitmapInfo = (WindowsAPI.BITMAPINFO*) pBytes;

                // The header height value is the combined height of XOR and AND masks.
                // So we simply divide by two to get the image height.
                m_iHeight = pBitmapInfo->bmiHeader.biHeight / 2;
                
                m_iWidth = pBitmapInfo->bmiHeader.biWidth;
                m_iColors = pBitmapInfo->bmiHeader.biPlanes * pBitmapInfo->bmiHeader.biBitCount;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of the icon image.
        /// </summary>
        public int Size
        {
            get
            {
                return m_arDibBits.Length;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the directory entry to the stream.
        /// </summary>
        unsafe public void WriteDirectoryEntry(BinaryWriter writer, 
                                               uint uiImageOffset)
        {
            WindowsAPI.FILEICONDIRENTRY data = new WindowsAPI.FILEICONDIRENTRY();
            data.dwImageOffset = uiImageOffset;

            data.bWidth = (byte) m_iWidth;
            data.bHeight = (byte) m_iHeight;

            fixed (byte* pBytes = &m_arDibBits[0])
            {
                WindowsAPI.BITMAPINFO* pBitmapInfo = (WindowsAPI.BITMAPINFO*) pBytes;

                data.wPlanes = pBitmapInfo->bmiHeader.biPlanes;
                data.wBitCount = pBitmapInfo->bmiHeader.biBitCount;
            }

            byte bColorCount = (byte) (data.wPlanes * data.wBitCount);

            if (8 <= bColorCount)
            {
                data.bColorCount = 0;
            }
            else
            {
                data.bColorCount = (byte) (1 << bColorCount);
            }

            data.dwBytesInRes = (uint) Size;

            writer.Write(data.bWidth);
            writer.Write(data.bHeight);
            writer.Write(data.bColorCount);
            writer.Write(data.bReserved);
            writer.Write(data.wPlanes);
            writer.Write(data.wBitCount);
            writer.Write(data.dwBytesInRes);
            writer.Write(data.dwImageOffset);
        }

        /// <summary>
        /// Writes the image data to the stream.
        /// </summary>
        unsafe public void WriteImage(BinaryWriter writer)
        {
            fixed (byte* pBytes = &m_arDibBits[0])
            {
                // When we write the BITMAPINFO the image size must be set to zero.
                // So we cache it here and restore it after we're done writing.

                WindowsAPI.BITMAPINFO* pBitmapInfo = (WindowsAPI.BITMAPINFO*) pBytes;
                uint biSizeImage = pBitmapInfo->bmiHeader.biSizeImage;
                pBitmapInfo->bmiHeader.biSizeImage = 0;

                writer.Write(m_arDibBits);

                pBitmapInfo->bmiHeader.biSizeImage = biSizeImage;
            }
        }

        #endregion

        #region Implementation

        private byte[] m_arDibBits;

        private int m_iHeight;
        private int m_iWidth;
        private int m_iColors;

        #endregion
	}
}
