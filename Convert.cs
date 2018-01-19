using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Collections.Generic; //V120

namespace Obfuscate
{
    class Convert: IConvert
    {
        CompilerParameters cp = new CompilerParameters(); //V120
        string iconFile = Path.GetTempPath() + "LastObfuscate.ico"; //V113 V115

        #region IConvert Members

        public void Init(string outputFile)
        {
            string outputPath = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            cp.GenerateExecutable = true;
            cp.OutputAssembly = outputFile; 
            cp.IncludeDebugInformation = false;
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.GenerateInMemory = false;
            cp.WarningLevel = 3;
            cp.TreatWarningsAsErrors = false;
            cp.CompilerOptions = Properties.Settings.Default.CompilerOptions + string.Format(" /Win32Icon:\"{0}\"", iconFile); //"/optimize /target:winexe"
            cp.TempFiles = new TempFileCollection(".", false);
        }

        private string getObfuscateFilename() //V115
        {
            string obfuscateFile = Path.GetTempPath() + "obfuscate.cs";
            File.WriteAllText(obfuscateFile, Replace(Properties.Resources.Obfuscate));
            return obfuscateFile;
        }

        private string getAssemblyFilename(Form1 form1)
        {
            string path = form1.txtPath.Text;
            int p = path.IndexOf("\\bin");
            if (p == -1) p = path.Length;
            string assemblyInfoFileName = path.Substring(0, p) + "\\Properties\\AssemblyInfo.cs";
        retry:
            if (File.Exists(assemblyInfoFileName))
            {
                return assemblyInfoFileName;
            }
            if (assemblyInfoFileName.Contains("\\Properties")) //V109
            {
                assemblyInfoFileName = assemblyInfoFileName.Replace("\\Properties", "");
                goto retry;
            }
            return "";//V122

        }

        private string Replace(string p) //V115
        {
            string result = p;
            result = result.Replace("resourceAssembly", "sdfwfvwevcbvef");
            result = result.Replace("dynamicAssemblyHash", "sdfwfvwevcdvef");
            result = result.Replace("resources", "sdfwfvvevcdvef");
            result = result.Replace("entryPoint", "sdfwfvwcvcbvef");
            result = result.Replace("exeBytes", "sdfwfvwevdbvef");
            result = result.Replace("EncriptionOffset", "sdfwvfwevcbvef");
            result = result.Replace("ResolveInternalAssembly","sdfwfvwfvcbvef");
            result = result.Replace("ConsoleWriteToFile", "sdfwvfewvcbvef");
            result = result.Replace("WriteFile", "sdfwvfewvcvbef");
            result = result.Replace("blnLog", "sdfwvfevwcvbef");
            result = result.Replace("From", "sdfwvfvewcvbef");
            result = result.Replace("ResourceNamex", "sdfwfvevwcvbef");
            result = ReplaceLiteralStrings(result);
            return result;
        }

        private string ReplaceLiteralStrings(string result)
        {
            string litStringList = "";
            int p=0,i=0;
            do
            {
                string lit = getLiteral(ref p, result);
                if (p>0 )
                {
                    result = result.Replace(lit, "litstr[" + i + "]");
                    litStringList += "," + lit;
                    i++;
                }
            } while (p>0);
            result = result.Replace("litstr[0]", litStringList.Substring(1));
            result = result.Replace("litstr", "sdfwfvvewcvbef");
            return result;
        }

        private string getLiteral(ref int p, string result)
        {
            int length=0;
            bool litmode = false;
            for (int i = p; i < result.Length; i++)
            {
                if (result[i] == '"')
                {
                    litmode = !litmode;
                    if (litmode) p = i;
                    else
                    {
                        length = i - p+1;
                        string s = result.Substring(p, length);
                        p = i+1;
                        return s;
                    }
                }
            }
            p = 0;
            return "";
        }

        public void ConvertFile(string file)
        {
			string ext = Path.GetExtension(file);
			if (ext == ".exe" || ext == ".dll") //V105
			{
				string tempFile = Path.GetTempPath()+Path.GetFileNameWithoutExtension(file)+".ico";
				Encript(file, tempFile);
                cp.EmbeddedResources.Add(tempFile); 
                if (ext == ".exe") 
                    Kerr.Resources.IconHelper.ExtractIconFromFile(file,iconFile); //V108
			}
            else //V116 copy
            {
                cp.EmbeddedResources.Add(file);
            }
        }

        private void Encript(string inFile, string outputFile)
        {
            
            using (FileStream fin = new FileStream(inFile,FileMode.Open))
            {
                using (FileStream fout = new FileStream(outputFile,FileMode.Create))
                {
                    int length = (int)fin.Length;
                    byte[] b = new byte[length];
                    fin.Read(b, 0, length);
                    //int offset = 0x3f; //V112 0x200; // update Obfuscate.From() also!!
                    int offset = ល.ល.EncriptionOffset;//V120+
                    for (int i = offset; i < offset+1; i++) b[i] ^= 1;
                    fout.Write(b, 0, length);
                }
            }
        }

        public string Result(Form1 form1)
        {
            List<string> sourceFiles = new List<string>();
            sourceFiles.Add(getObfuscateFilename());
            sourceFiles.Add(getAssemblyFilename(form1));
            sourceFiles.RemoveAll(s => s == "");//V122
            // Invoke compilation.
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFiles.ToArray());
            string result = "";
            if (cr.Errors.Count > 0)
            {
                result = string.Format("Errors building {0}", cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors) result += "\n" + ce.ToString();
                Console.WriteLine(result);
            }
            return result;
        }
        #endregion
    }
}
