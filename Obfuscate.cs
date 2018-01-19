//V115
//don't use My class in this file!! (an obfucated exe may not have a reference to this class)
using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Resources;
using System.Drawing;
using System.Windows.Forms;

namespace ល
{
    class ល
    {
        static Assembly resourceAssembly;                       //sdfwfvwevcbvef;
        static Hashtable dynamicAssemblyHash = new Hashtable(); //sdfwfvwevcdvef;
        static string[] resources;                              //sdfwfvvevcdvef; 
        static MethodInfo entryPoint;                           //sdfwfvwcvcbvef;
        static byte[] exeBytes;                                 //sdfwfvwevdbvef;
        public static int EncriptionOffset= 0x3F;               //sdfwvfwevcbvef;
        static string[] litstr = new string[] { "xxxx" };
        static bool blnLog = Environment.CommandLine.Contains("/log");
        static string ResourceNamex;
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (blnLog) ConsoleWriteToFile("Rudy.log");
                Console.WriteLine("start");
                if (args.Length >= 2 && args[0]=="test")
                    resourceAssembly = Assembly.LoadFile(args[1]);
                else
                    resourceAssembly = Assembly.GetEntryAssembly();
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveInternalAssembly);//sdfwfvwfvcbvef
                Assembly target = AppDomain.CurrentDomain.Load("Initial");
                entryPoint = target.EntryPoint;
                if (entryPoint != null)
                {
                    ParameterInfo[] p = entryPoint.GetParameters();
                    if (p.Length == 1) entryPoint.Invoke(null, new object[] { args });
                    else               entryPoint.Invoke(null, new object[] { });

                }
            }
            catch (Exception e)
            {
                if (!blnLog) ConsoleWriteToFile("error.log");
                Console.WriteLine(e.ToString());
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException.Message);
            }
        }

        static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args)
        {
            ResourceNamex = args.Name.Split(',')[0];
            Console.WriteLine("R {0}", ResourceNamex); //Resolve
            if (dynamicAssemblyHash[ResourceNamex] == null)
            {
                resources = resourceAssembly.GetManifestResourceNames();
                if (ResourceNamex == "Initial")
                {
                    foreach (string item in resources) Console.WriteLine(":{0}",item);
                    ResourceNamex = Path.GetFileNameWithoutExtension(resources[0])+"";
                }
                foreach (string name in resources)
                {
                        
                        using (Stream asmStream = resourceAssembly.GetManifestResourceStream(name))
                        {
                            exeBytes = new byte[asmStream.Length];
                            asmStream.Read(exeBytes, 0, exeBytes.Length);
                            asmStream.Close();
                            string name1 = Path.GetFileNameWithoutExtension(name)+"";
                            Console.WriteLine("{0}={1}",name1,exeBytes.Length);
                            if (name.Contains(".ico"))
                                dynamicAssemblyHash[name1] = Assembly.Load(From(exeBytes));
                            else
                                WriteFile(Path.GetDirectoryName(resourceAssembly.Location)+"\\"+ name, exeBytes);
                        }
                }
            }
            return dynamicAssemblyHash[ResourceNamex] as Assembly;
        }

        private static void WriteFile(string filename,byte[] bytes) //V116
        {
            Console.WriteLine("w file: "+filename );
            if (!File.Exists(filename)) File.WriteAllBytes(filename,exeBytes);
        }
        private static byte[] From(byte[] b)
        {
            for (int i = EncriptionOffset; i < EncriptionOffset + 1; i++) b[i] ^= 1;
            return b;
        }
        public static void ConsoleWriteToFile(string outputFile)
        {
            StreamWriter sw = new StreamWriter(outputFile);
            sw.AutoFlush = true;
            Console.SetOut(sw);
        }
    }
}
