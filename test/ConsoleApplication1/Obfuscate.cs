#define DEBUG
using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Resources;
//using TestObfuscate.Properties;

namespace ល
{
    class ល
    {
        static Assembly resourceAssembly;//resourceAssembly;
        static Hashtable dynamicAssemblyHash = new Hashtable();//dynamicAssemblyHash
        static string[] resources; //resources
        static MethodInfo entryPoint; //entryPoint
        static byte[] exeBytes;//exeBytes
        static int EncriptionOffset;//Encription offset
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                EncriptionOffset = 0x3F; //V112 0x200;
                resourceAssembly = Assembly.GetEntryAssembly();
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveInternalAssembly);
                Assembly target = AppDomain.CurrentDomain.Load("Initial");//Beautifier");
                entryPoint = target.EntryPoint;
                Debug.WriteLine(string.Format("entry: {0}", target.EntryPoint));
                if (entryPoint != null)
                {
                    ParameterInfo[] p = entryPoint.GetParameters();
                    if (p.Length == 1)
                        entryPoint.Invoke(null, new object[] { args });
                    else
                        entryPoint.Invoke(null, new object[] { });

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException.Message);
            }
        }

        static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args) //ResolveInternalAssembly
        {
            string ResourceName = args.Name.Split(',')[0]; //ResourceName
            Debug.Write(string.Format("\n{0} Resolve: {1}",DateTime.Now,ResourceName));
            if (dynamicAssemblyHash[ResourceName] == null)
            {
                resources = resourceAssembly.GetManifestResourceNames();
                if (ResourceName == "Initial")
                {
                    if (resources.Length > 0)
                    {
                        ResourceName = resources[0]; // Load first resource.
                        Debug.Write(" "+ ResourceName);
                    }
                    else Console.WriteLine("The are no resources in this assembly.");
                }
                //
                // add following lines for debug only.
                //
                //if (((System._AppDomain)sender).FriendlyName.StartsWith("TestOb"))
                //{
                //    ResourceManager rm = new ResourceManager("TestObfuscate.Properties.Resources", typeof(Resources).Assembly);
                //    if (ResourceName.StartsWith("Test"))
                //        ResourceName = "Q1";// "Beautifier";
                //    exeBytes = (Byte[]) rm.GetObject(ResourceName.Replace('.','_'));
                //    Debug.Write(string.Format(" {0} {1} bytes",ResourceName, exeBytes.Length));
                //    dynamicAssemblyHash[ResourceName] = Assembly.Load(exeBytes);
                //    Debug.WriteLine("Loaded!");
                //}
                //else 
                foreach (string name in resources)
                {
                    if (name.Contains(ResourceName))
                    {

                        using (Stream asmStream = resourceAssembly.GetManifestResourceStream(name))
                        {
                            exeBytes = new byte[asmStream.Length];
                            asmStream.Read(exeBytes, 0, exeBytes.Length);
                            asmStream.Close();

                            Debug.Write(string.Format(" {0} bytes", exeBytes.Length));
                            dynamicAssemblyHash[ResourceName] = Assembly.Load((exeBytes));
                            Debug.WriteLine(" loaded!");
                        }
                    }
                }
            }

            return dynamicAssemblyHash[ResourceName] as Assembly;
        }
        private static byte[] From(byte[] b)
        {
            // update Convert.Encript also!!
            // int offset = 0x3F 200;
            // V112 for (int i = EncriptionOffset; i < EncriptionOffset + 200; i++) b[i] ^= 1;
            for (int i = EncriptionOffset; i < EncriptionOffset + 1; i++) b[i] ^= 1;
            return b;
        }
    }
}
