using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Kerr.Resources
{
	public static class IconHelper
	{
		static IconResource iconResource; 

		public static bool ExtractIconFromFile(string exeFile, string iconFile)
		{
			using (Library library = new Library(exeFile,WindowsAPI.LOAD_LIBRARY_AS_DATAFILE))
			{
				library.EnumResourceNames(WindowsAPI.RT_GROUP_ICON,
										  new WindowsAPI.EnumResNameProc(AddIconToList),
										  IntPtr.Zero);
			}
			if (iconResource == null) return false;
			iconResource.Save(iconFile);

			return true;
		}

		static private bool AddIconToList(IntPtr hModule,IntPtr pType,IntPtr pName,IntPtr param)
		{
				Library library = new Library(hModule, false);
				//if (iconResource == null) //V117
					iconResource = new IconResource(library, pName);

			return true;
		}
	}
}
