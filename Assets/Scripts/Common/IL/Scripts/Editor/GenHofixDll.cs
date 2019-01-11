namespace game.IL
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.CodeDom.Compiler;
    using System.Linq;

    static class GenHofixDLL
    {
        static string ScriptSources = "Assets/Scripts";
        static string TargetPath = Application.streamingAssetsPath + "/Hotfix";
        static string HotDLLFilePath = TargetPath + "/" + "Hotfix.dll";
        static string BaseDLLFilePath = TargetPath + "/" + "Base.dll";

        [MenuItem("IL/生成热更DLL",false,4)]
        static void Gen_HotfixDll()
        {
            //获得所有文件路径
            var filePaths = GetAllScriptFile(ScriptSources);
            
            var baseFilePaths = new List<string>();//本地文件
            var hotFilePaths = new List<string>(); //热更文件

            GetScriptFile(filePaths, ref baseFilePaths, ref hotFilePaths);

            //生成DLL
           Gen(baseFilePaths, hotFilePaths);
        }

        static void Gen(List<string> baseFiles,List<string> hotFiles)
        {
            //收集Unity库文件信息
            var ui_path = EditorApplication.applicationContentsPath + @"\UnityExtensions\Unity\GuiSystem";
            var engine_path = EditorApplication.applicationContentsPath + @"\Managed\UnityEngine";
            if(!Directory.Exists(ui_path) || !Directory.Exists(engine_path))
            {
                Debug.LogError("DLL Path Exists Error!");
                return;
            }

            //依赖信息集
            var assembly = new Dictionary<string,string>();
            var dlls = Directory.GetFiles(ui_path, "*.dll", SearchOption.AllDirectories);
            foreach (var d in dlls)
            {
                var fileName = Path.GetFileName(d);
                if (assembly.ContainsKey(fileName) || fileName.Contains("editor") || fileName.Contains("Editor"))
                    continue;
                assembly.Add(fileName, d);
            }
            dlls = Directory.GetFiles(engine_path, "*.dll", SearchOption.AllDirectories);
            foreach (var d in dlls)
            {
                var fileName = Path.GetFileName(d);
                if (assembly.ContainsKey(fileName) || fileName.Contains("editor") || fileName.Contains("Editor"))
                    continue;
                assembly.Add(fileName, d);
            }
            //
            assembly.Add("System","System.dll");
            assembly.Add("System.Core","System.Core.dll");
            assembly.Add("System.XML","System.XML.dll");
            assembly.Add("System.Data","System.Data.dll");

            if (Directory.Exists(TargetPath))
                Directory.Delete(TargetPath,true);
            Directory.CreateDirectory(TargetPath);

            //编译BaseDll(类似Assembly-CSharp)
            bool result = BuildDLL(BaseDLLFilePath, assembly.Values.ToArray(), baseFiles);
            if (!result)
                return;
            //编译HotfixDll
            assembly.Add(Path.GetFileName(BaseDLLFilePath), BaseDLLFilePath);
            result = BuildDLL(HotDLLFilePath, assembly.Values.ToArray(), hotFiles);
            if (!result)
                return;
            AssetDatabase.Refresh();
        }

        static bool BuildDLL(string outPut,string[] assemblys,List<string> filePaths)
        {
            var cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.IncludeDebugInformation = true;
            cp.TempFiles = new TempFileCollection(".", true);
            cp.OutputAssembly = outPut;
            cp.TreatWarningsAsErrors = false;
            cp.WarningLevel = 0;
            cp.CompilerOptions = "/optimize+ /unsafe";
            foreach (var refAssembly in assemblys)
                cp.ReferencedAssemblies.Add(refAssembly);

            var provider = CodeDomProvider.CreateProvider("CSharp");
            var cr = provider.CompileAssemblyFromFile(cp, filePaths.ToArray());
            if (cr.Errors.HasErrors)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var ce in cr.Errors)
                    sb.AppendLine(ce.ToString());
                Debug.LogError(sb);
                return false;
            }
            else
            {
                return true;
            }
        }

        static void  GetScriptFile(List<string> paths,ref List<string> localFiles,ref List<string> hotFiles)
        {
            foreach (var path in paths)
            {
                var text = File.ReadAllText(path);
                if (null == text || text.Length == 0)
                    continue;
                //排出Editor类
                if (text.Contains("using UnityEditor;"))
                    continue;
                //
                if (!text.StartsWith("#if !USE_HOT") || !text.Contains("namespace hot"))
                    localFiles.Add(path);
                else
                    hotFiles.Add(path);
            }
        }

        static List<string> GetAllScriptFile(string path)
        {
            var startIndex = path.Length + 1;
            if (!Directory.Exists(path))
                return new List<string>();

            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            var resList = new List<string>();
            var tmp = string.Empty;
            var s = string.Empty;
            var filePath = string.Empty;

            for (int i = 0; i < files.Length; ++i)
            {
                filePath = files[i].Replace('/', '\\');
                s = filePath.Substring(startIndex);
                tmp = s.ToLower();

                if (!tmp.EndsWith(".cs"))
                    continue;

                if (!tmp.EndsWith(".cs", true, null) || tmp.Contains("\\editor\\"))
                    continue;

                resList.Add(filePath);
            }
            return resList;
        }
    }
}

