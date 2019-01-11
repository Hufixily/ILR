
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace game.IL.Editor
{
    public class CopyILFile
    {
        static string parentPath = "Assets/Scripts";
#if USE_HOT
        //[MenuItem("IL/拷贝热更文件",false,4)]
        public static void CopyFileToProject()
        {
            var paths = GetAllScriptFile(parentPath);
            var filePath = string.Empty;
            var targetPath = "E:/test/ILRFrameWork/Hot/Scripts/";//ResourcesPath.HotProjectPath;
            var rootPath = string.Empty;
            var assetPath = string.Empty;
            var nowIndex = 0;
            try
            {
                foreach (var path in paths)
                {
                    assetPath = parentPath + "/" + path;
                    filePath = targetPath + path;
                    var text = System.IO.File.ReadAllText(assetPath);
                    if (null == text || text.Length == 0)
                        continue;
                    if (text.Contains("UnityEditor"))
                        continue;
                    if (!text.Contains("namespace hot"))
                        continue;
                    if (!text.StartsWith("#if !USE_HOT"))
                        continue;
                    Debug.Log(path);
                    rootPath = Path.GetDirectoryName(filePath);
                    EditorUtility.DisplayCancelableProgressBar("热更文件复制中 : " + assetPath, assetPath, (float)++nowIndex / (float)paths.Count);
                    //目录创建
                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }
                    //文件复制
                    System.IO.File.Copy(assetPath, filePath, true);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
#endif

        [MenuItem("Assets/IL/hot空间全局添加宏")]
        public static void AddMarcoAll()
        {
            var paths = GetAllScriptFile(parentPath);
            var assetPath = string.Empty;
            foreach(var path in paths)
            {
                assetPath = string.Format("{0}/{1}", parentPath, path);
                var text = System.IO.File.ReadAllText(assetPath);
                if (null == text || text.Length == 0)
                    continue;
                if (!text.Contains("namespace hot"))
                    continue;
                if (text.StartsWith("#if !USE_HOT"))
                    continue;
                if (text.Contains("UnityEditor"))
                    continue;
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.AppendLine("#if !USE_HOT");
                builder.AppendLine(text);
                builder.Append("#endif");
                System.IO.File.WriteAllText(assetPath, builder.ToString());
                Debug.Log("File Add Marco :" + path);
            }
        }

        static List<string> GetAllScriptFile(string path)
        {
            int startIndex = path.Length + 1;
            if (!System.IO.Directory.Exists(path))
                return new List<string>();

            string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            List<string> resList = new List<string>();
            string tmp;
            string s;
            for (int i = 0; i < files.Length; ++i)
            {
                s = files[i].Replace('\\', '/').Substring(startIndex);
                tmp = s.ToLower();

                if (!tmp.EndsWith(".cs"))
                    continue;

                if (!tmp.EndsWith(".cs", true, null) || tmp.Contains("/Editor/"))
                    continue;

                resList.Add(s);
            }
            return resList;
        }
    }
}