//My.Version...........:  V1.9.2
//My.Drive.............: C:\
//My.ExeFile...........: C:\Projects\My\Test\bin\Debug\Test
//My.ExePath...........: C:\Projects\My\Test\bin\Debug\
//My.VolumeSerialNumber: C841E00A
//My.IPAddress.........: 82.169.9.134
//My.Pin drive C:\.....: 1F8E
//My.GenerateLicenseKey: C5C2AA13
//My.UserConfigFile....: C:\Documents and Settings\Rudy Meijer\Local Settings\Application Data\Test\Test.exe_Url_x4jo3ebrlgthn0bfxhajbtt4jvtvj5so\1.9.1.0\user.config //assemblyVersion is used here.
//20-08-2008 22:04:05 My.Log(Hello version  V1.9.2)
//Druk op een toets om door te gaan. . .

//My.Version...........:  V1.0.0
//My.Drive.............: C:\
//My.ExeFile...........: Test
//My.ExePath...........: C:\Projects\My\Test\bin\Debug\
//My.VolumeSerialNumber: C841E00A
//My.IPAddress.........: 82.169.9.134
//My.Pin drive C:\.....: 1F8E
//My.GenerateLicenseKey: C5C2AA13
//03-05-2008 20:35:46 Hello version 1.0.0
//Druk op een toets om door te gaan. . .
using System;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.Configuration;

public static partial class My //V114
{
    public static string Version = " V" + System.Windows.Forms.Application.ProductVersion.Substring(0, 5);//V226
    private static string ExeFullFile = System.Windows.Forms.Application.ExecutablePath;
    public static string ExePath = Path.GetDirectoryName(ExeFullFile) + "\\";
    public static string ExeFile = ExePath + Path.GetFileNameWithoutExtension(ExeFullFile);

    public static void Log(string format, params object[] arg)
    {
        string msg = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " " + (arg.Length > 0 ? String.Format(format, arg) : format); // V218 V230
        Console.WriteLine(msg);
        AppendToFile(My.ExeFile + ".log", msg);
    }
    
    public static void AppendToFile(string fileName, string msg)
    {
        const bool APPEND = true;
        using (StreamWriter sw = new StreamWriter(fileName, APPEND)) sw.WriteLine(msg);
    }
 }