using System;
using System.Collections;
using System.Reflection;
using System.IO;

namespace O
{
    class O
    {
        static Assembly resourceAssembly;
        static Hashtable dynamicAssemblyHash = new Hashtable();
        [STAThread]
        public static void Main(string[] args)
        {

            resourceAssembly = Assembly.GetEntryAssembly();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveInternalAssembly);
            Assembly target = AppDomain.CurrentDomain.Load("Initial");//Beautifier");
            MethodInfo entryPoint = target.EntryPoint;
            if (entryPoint != null)
            {
                entryPoint.Invoke(null, new object[] { args });
            }
        }

        static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args)
        {
            string shortName = args.Name.Split(',')[0];
            Console.Write("resolve: "+shortName);
            if (dynamicAssemblyHash[shortName] == null)
            {
                string[] resources = resourceAssembly.GetManifestResourceNames();
                if (shortName == "Initial")
                {
                    if (resources.Length > 0)
                    {
                        shortName = resources[0]; // Load first resource.
                    }
                    else Console.WriteLine("The are no resources in this assembly.");
                }
                foreach (string name in resources)
                {
                    if (name.Contains(shortName))
                    {
                        using (Stream asmStream = resourceAssembly.GetManifestResourceStream(name))
                        {
                            byte[] exeBytes = new byte[asmStream.Length];
                            asmStream.Read(exeBytes, 0, exeBytes.Length);
                            asmStream.Close();
                            Console.WriteLine(" {0} bytes.",exeBytes.Length );
                            dynamicAssemblyHash[shortName] = Assembly.Load(exeBytes);
                        }
                    }
                }
            }

            return dynamicAssemblyHash[shortName] as Assembly;
        }
    }
}
