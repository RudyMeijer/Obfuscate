//*****************************************************************************
//
//      Class:          Library
//      Namespace:      Kerr.Resources
//      Date created:   23 November 2001
//      Copyright:      © Kenny Kerr 2001 (mailto:kennykerr@hotmail.com)
//
//*****************************************************************************

using System;
using System.Diagnostics;
using System.Drawing;

namespace Kerr.Resources
{
    /// <summary>
    /// The Library class allows you to load unmanaged libraries, such as 
    /// executables and dynamic-link libraries.
    /// </summary>
	internal class Library : IDisposable
	{
        #region Construction/destruction

        /// <summary>
        /// Maps the specified module into the address space of the calling 
        /// process. The module will be freed when the Library object is 
        /// disposed.
        /// </summary>
		public Library(string strFileName,
                       ulong ulFlags)
		{
            m_bOwn = true;

			m_hModule = WindowsAPI.LoadLibraryEx(strFileName,
                                                 IntPtr.Zero,
                                                 ulFlags);

            if (IntPtr.Zero == m_hModule)
            {
                WindowsAPI.ThrowLastError();
            }
		}

        /// <summary>
        /// Creates a Library object attached to the given module. If 
        /// bTakeOwnership is true the module will be freed when the Library 
        /// object is disposed.
        /// </summary>
        public Library(IntPtr hModule,
                       bool bTakeOwnership)
        {
            Debug.Assert(IntPtr.Zero != hModule);

            m_hModule = hModule;
            m_bOwn = bTakeOwnership;
        }

        /// <summary>
        /// Frees the underlying module.
        /// </summary>
        ~Library()
        {
            Free();
        }

        /// <summary>
        /// Frees the underlying module.
        /// </summary>
        public void Dispose()
        {
            Free();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a handle to the underlying module.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return m_hModule;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Decrements the reference count of the loaded module if the Library 
        /// object owns it.
        /// </summary>
        public void Free()
        {
            if (m_bOwn && IntPtr.Zero != m_hModule)
            {
                WindowsAPI.FreeLibrary(m_hModule);
            }

            m_hModule = IntPtr.Zero;
        }

        /// <summary>
        /// Searches the library for each resource of the specified type and 
        /// passes either the name or the ID of each resource it locates to the
        /// delegate.
        /// </summary>
        public int EnumResourceNames(IntPtr pType,
                                     WindowsAPI.EnumResNameProc enumFunc,
                                     IntPtr param)
        {
            Debug.Assert(IntPtr.Zero != m_hModule);

            return WindowsAPI.EnumResourceNames(m_hModule,
                                                pType,
                                                enumFunc,
                                                param);
        }

        /// <summary>
        /// Determines the location of a resource with the specified type and 
        /// name.
        /// </summary>
        public IntPtr FindResource(IntPtr pName,
                                   IntPtr pType)
        {
            Debug.Assert(IntPtr.Zero != m_hModule);

            IntPtr hResource = WindowsAPI.FindResource(m_hModule,
                                                       pName,
                                                       pType);

            if (IntPtr.Zero == hResource)
            {
                WindowsAPI.ThrowLastError();
            }

            return hResource;
        }

        /// <summary>
        /// Loads the specified resource into memory.
        /// </summary>
        public IntPtr LoadResource(IntPtr hResource)
        {
            Debug.Assert(IntPtr.Zero != m_hModule);

            IntPtr hGlobal = WindowsAPI.LoadResource(m_hModule,
                                                     hResource);

            if (IntPtr.Zero == hGlobal)
            {
                WindowsAPI.ThrowLastError();
            }

            return hGlobal;
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified resource.
        /// </summary>
        public uint SizeofResource(IntPtr hResource)
        {
            Debug.Assert(IntPtr.Zero != m_hModule);

            uint dwByteCount = WindowsAPI.SizeofResource(m_hModule,
                                                         hResource);

            if (0 == dwByteCount)
            {
                WindowsAPI.ThrowLastError();
            }

            return dwByteCount;
        }

        /// <summary>
        /// Locks the specified resource in memory.
        /// </summary>
        public static IntPtr LockResource(IntPtr hGlobal)
        {
            IntPtr pResource = WindowsAPI.LockResource(hGlobal);

            if (IntPtr.Zero == pResource)
            {
                WindowsAPI.ThrowLastError();
            }

            return pResource;
        }

        /// <summary>
        /// Loads the specified icon resource from the library.
        /// </summary>
        public Icon LoadIcon(IntPtr pName)
        {
            IntPtr hIcon = WindowsAPI.LoadIcon(m_hModule,
                                               pName);

            if (IntPtr.Zero == hIcon)
            {
                WindowsAPI.ThrowLastError();
            }

            Icon n = Icon.FromHandle(hIcon);
			return n;
        }

        #endregion

        #region Implementation

        private IntPtr m_hModule;
        private bool m_bOwn;

        #endregion
	}
}
