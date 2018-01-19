//*****************************************************************************
//
//      Class:          IconResource
//      Namespace:      Kerr.Resources
//      Date created:   23 November 2001
//      Copyright:      © Kenny Kerr 2001 (mailto:kennykerr@hotmail.com)
//
//*****************************************************************************

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Kerr.Resources
{
    /// <summary>
    /// The IconResource class represents an icon resource. Normally when you 
    /// load an icon the operating system returns an icon image targeted to 
    /// the current display. The IconResource class contains all the images in
    /// the icon resource.
    /// </summary>
	internal class IconResource
	{
        #region Constructors

        /// <summary>
        /// Create an IconResource object to represent the icon resource in the
        /// given library.
        /// </summary>
        unsafe public IconResource(Library library,
                                   IntPtr pResourceName)
        {
            m_pName = pResourceName;

            // Get the RT_GROUP_ICON resource data.

            IntPtr hGroupInfo = library.FindResource(m_pName, 
                                                     WindowsAPI.RT_GROUP_ICON);

            IntPtr hGroupRes = library.LoadResource(hGroupInfo);
            uint dwGroupResSize = library.SizeofResource(hGroupInfo);

            WindowsAPI.MEMICONDIR* pDirectory = (WindowsAPI.MEMICONDIR*) Library.LockResource(hGroupRes);

            // Get the RT_ICON resource data for each icon in the RT_GROUP_ICON.

            m_images = new IconImage[pDirectory->wCount];

            for (ushort i = 0; i < pDirectory->wCount; ++i)
            {
                WindowsAPI.MEMICONDIRENTRY* pEntry = GetDirectoryEntry(pDirectory, i);

                m_images[i] = new IconImage(library, 
                                            new IntPtr(pEntry->wId));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the resource as a string.
        /// </summary>
        public string Text
        {
            get
            {
                if (0 == m_strText.Length)
                {
                    if (WindowsAPI.IS_INTRESOURCE(m_pName))
                    {
                        m_strText = m_pName.ToString();
                    }
                    else
                    {
                        m_strText = Marshal.PtrToStringAuto(m_pName);
                    }
                }

                return m_strText;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the icon resource to a .ICO file.
        /// </summary>
        public void Save(string strFileName)
        {
            Stream stream = new FileStream(strFileName, 
                                           FileMode.Create, 
                                           FileAccess.Write, 
                                           FileShare.None);

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                WriteHeader(writer);
                WriteDirectoryEntries(writer);
                WriteImages(writer);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Helper function get the icon directory entry at the given index.
        /// </summary>
        unsafe private static WindowsAPI.MEMICONDIRENTRY* GetDirectoryEntry(WindowsAPI.MEMICONDIR* pDirectory,
                                                                            ushort index)
        {
            WindowsAPI.MEMICONDIRENTRY* pEntry = &pDirectory->arEntries;
            return pEntry += index;
        }

        /// <summary>
        /// Writes the .ICO file header data to the stream.
        /// </summary>
        private void WriteHeader(BinaryWriter writer)
        {
            // Write the reserved WORD (0).
            ushort word = 0;
            writer.Write(word);

            // Write the resource type (1 for icons).
            word = 1;
            writer.Write(word);

            // Write the image count.
            word = (ushort) m_images.Length;
            writer.Write(word);
        }

        /// <summary>
        /// Writes the icon directory entries to the stream.
        /// </summary>
        private void WriteDirectoryEntries(BinaryWriter writer)
        {
            for (int i = 0; i < m_images.Length; ++i)
            {
                IconImage image = m_images[i];
                image.WriteDirectoryEntry(writer, (uint) GetIcoImageOffset(i));
            }
        }

        /// <summary>
        /// Writes the icon image data to the stream.
        /// </summary>
        private void WriteImages(BinaryWriter writer)
        {
            for (int i = 0; i < m_images.Length; ++i)
            {
                IconImage image = m_images[i];
                image.WriteImage(writer);
            }
        }

        /// <summary>
        /// Gets the offset of the image data from the beginning of the .ICO 
        /// file. This is used by WriteDirectoryEntries to write the icon 
        /// directory entries.
        /// </summary>
        private int GetIcoImageOffset(int index)
        {
            // Space for the ICO header.
            int iOffset = 3 * Marshal.SizeOf(typeof (ushort));

            // Space for the FILEICONDIRENTRY structures.
            iOffset += m_images.Length * Marshal.SizeOf(typeof (WindowsAPI.FILEICONDIRENTRY));

            // Space for any preceding images.
            for (int i = 0; i < index; ++i)
            {
                IconImage image = m_images[i];
                iOffset += image.Size;
            }

            return iOffset;
        }

        private IntPtr m_pName;
        private IconImage[] m_images;

        private string m_strText = string.Empty;

        #endregion
	}
}
