#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace game
{
    public static partial class Utility
    {
        public static bool HasExportScene(string assetPath)
        {
            if (assetPath.StartsWith("Assets/__copy__/"))
                return false;

            if (assetPath.Contains("/SceneExport/") && assetPath.EndsWith(".unity", true, null))
            {
                return true;
            }

            return false;
        }

        static public List<string> GetAllSceneList()
        {
            List<string> scenes = new List<string>();
            ForEach("Assets", (AssetImporter ai) =>
            {
                scenes.Add(ai.assetPath);
            },
            (string assetPath, string root) =>
            {
                return HasExportScene(assetPath);
            });

#if SCENE_OPT
            HashSet<string> allscene = new HashSet<string>(scenes);
            for (int i = scenes.Count - 1; i <= 0; --i)
            {
                string path = scenes[i];
                if (path.EndsWith("_Opt.unity"))
                {

                }
                else
                {
                    string optScene = path.Substring(0, path.LastIndexOf('.')) + "_Opt.unity";
                    if (allscene.Contains(optScene))
                        scenes.RemoveAt(i);
                }
            }
#endif
            scenes.Sort((string x, string y) => { return y.CompareTo(x); });
            return scenes;
        }

        public static void Swap<TT>(ref TT x, ref TT y)
        {
            TT t = x;
            x = y;
            y = t;
        }

        static public void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (Application.isEditor) UnityEngine.Object.DestroyImmediate(obj);
                else UnityEngine.Object.Destroy(obj);
#else
                Object.Destroy(obj);
#endif
            }
        }

        static public void DestroyChildren(this Transform t)
        {
            bool isPlaying = Application.isPlaying;

            while (t.childCount != 0)
            {
                Transform child = t.GetChild(0);

                if (isPlaying)
                {
                    child.parent = null;
                    UnityEngine.Object.Destroy(child.gameObject);
                }
                else UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }

       // public delegate bool CopyFolderFunc(ref string fullpath);

//         public static void CopyFolder(string direcSource, string direcTarget, CopyFolderFunc fun = null)
//         {
//             if (!Directory.Exists(direcSource))
//                 return;
// 
//             //             if (!Directory.Exists(direcTarget))
//             //                 Directory.CreateDirectory(direcTarget);
// 
//             DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
//             FileInfo[] files = direcInfo.GetFiles();
//             foreach (FileInfo file in files)
//             {
//                 string d = Path.Combine(direcTarget, file.Name).Replace('\\', '/');
//                 if (fun != null && !fun(ref d))
//                     continue;
// 
//                 Directory.CreateDirectory(d.Substring(0, d.LastIndexOf('/')));
//                 try
//                 {
//                     file.CopyTo(d, true);
//                 }
//                 catch (System.Exception)
//                 {
// 
//                 }
//             }
// 
//             DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
//             foreach (DirectoryInfo dir in direcInfoArr)
//                 CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name), fun);
//         }

        public static string GetTypeName(System.Type type)
        {
            string ns = type.Namespace;
            if (string.IsNullOrEmpty(ns))
            {
                ns = type.FullName;
            }
            else
            {
                if (ns == "UnityEngine")
                    return type.FullName;

                ns = ns + "." + type.Name;
            }

            int s = ns.LastIndexOf('[');
            if (s != -1)
            {
                ns = string.Format("{0}<{1}>", ns.Substring(0, ns.LastIndexOf("`1")), ns.Substring(s + 1, ns.IndexOf(',', s) - s - 1));
                ns = ns.Replace('+', '.');
            }

            return ns;
        }

#if MEMORY_CHECK
        static string GetNameType(System.Type type)
        {
            string name = type.FullName;
            int s = name.LastIndexOf('[');
            if (s != -1)
            {
                int last_1 = name.LastIndexOf("`1");
                string temptype = name.Substring(s + 1, name.IndexOf(',', s) - s - 1);
                if (temptype.StartsWith("UnityEngine."))
                    temptype = temptype.Substring(12);

                name = string.Format("{0}<{1}>{2}", 
                    name.Substring(0, last_1),
                    temptype, 
                    name.Substring(last_1 + 2, s - last_1 - 3));
                name = name.Replace('+', '.');
            }

            return name;
        }
        public static string GetTypeNameFull(string fullname)
        {
            System.Type type = TypeFind.Get(fullname);
            if (type != null)
                return GetTypeNameFull(type);

            return fullname;
        }


        public static string GetTypeNameFull(System.Type type)
        {
            return GetNameType(type);
            //string ns = type.Namespace;

            //List<System.Type> parentTypes = new List<System.Type>();
            //System.Type current = type;
            //while (current.BaseType != null)
            //{
            //    if (current.BaseType == typeof(MemoryObject))
            //        break;

            //    parentTypes.Add(current.BaseType);
            //    if (current.BaseType == typeof(MemoryMonoBehaviour))
            //        break;

            //    current = current.BaseType;
            //}

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //if (!string.IsNullOrEmpty(ns))
            //    sb.AppendFormat("{0}/", ns);

            //for (int i = parentTypes.Count - 1; i >= 0; --i)
            //{
            //    sb.AppendFormat("{0}/", GetNameType(parentTypes[i]));
            //}

            //sb.Append(GetNameType(type));

            //return sb.ToString();
        }
#endif

        public static void CopyFolder(string direcSource, string direcTarget, System.Func<string, bool> fun)
        {
            if (!Directory.Exists(direcSource))
                return;

            DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
            FileInfo[] files = direcInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string d = Path.Combine(direcTarget, file.Name).Replace('\\', '/');
                if (fun != null && !fun(d))
                    continue;

                Directory.CreateDirectory(d.Substring(0, d.LastIndexOf('/')));
                file.CopyTo(d, true);
            }

            DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
            foreach (DirectoryInfo dir in direcInfoArr)
                CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name), fun);
        }

        public static void DeleteFolder(string dir)
        {
            if (dir[dir.Length - 1] == '/')
                dir = dir.Substring(0, dir.Length - 1);

            if (!Directory.Exists(dir))
                return;

            foreach (string file in Directory.GetFiles(dir))
                File.Delete(file);

            foreach (string d in Directory.GetDirectories(dir + "/"))
                DeleteFolder(d);

            try
            {
                Directory.Delete(dir + "/");
            }
            catch (System.Exception)
            {

            }

            if (File.Exists(dir + ".meta"))
                File.Delete(dir + ".meta");
        }

        // direcSource有direcTarget也有就删除
        public static void RemoveFile(string direcSource, string direcTarget, System.Func<string, bool> fun = null)
        {
            if (!Directory.Exists(direcSource))
                return;

            if (!Directory.Exists(direcTarget))
                return;

            DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
            FileInfo[] files = direcInfo.GetFiles();
            string fullpath;
            foreach (FileInfo file in files)
            {
                if (fun != null && !fun(file.Name))
                    continue;

                fullpath = Path.Combine(direcTarget, file.Name);
                if (File.Exists(fullpath))
                {
                    try
                    {
                        File.Delete(fullpath);
                    }
                    catch (System.Exception /*ex*/)
                    {

                    }
                }
            }

            DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
            foreach (DirectoryInfo dir in direcInfoArr)
                RemoveFile(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name), fun);
        }

        public static void ForEach<T>(string path, System.Action<T> action, System.Func<string, string, bool> isUsed = null) where T : AssetImporter
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("非法的目录:" + path);
                return;
            }

            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            if (File.Exists(path))
            {
                if (isUsed != null)
                {
                    if (!isUsed(path, path.Substring(0, path.LastIndexOf('/'))))
                        return;
                }

                // 是个文件
                T assetImporter = AssetImporter.GetAtPath(path) as T;
                if (assetImporter != null)
                {
                    action(assetImporter);
                }
            }
            else
            {
                HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { path }));
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (assetPath == null)
                        continue;
                    if (!File.Exists(assetPath))
                        continue;

                    if (isUsed != null)
                    {
                        if (!isUsed(assetPath, path))
                            continue;
                    }

                    T assetImporter = AssetImporter.GetAtPath(assetPath) as T;
                    if (assetImporter != null)
                        action(assetImporter);
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static void ForEachSelect<T>(System.Action<T> action, System.Func<string, string, bool> isUsed = null) where T : AssetImporter
        {
            ForEach(Selection.objects, action, isUsed);
        }

        public static void ForEach<T>(Object[] objs, System.Action<T> action, System.Func<string, string, bool> isUsed = null) where T : AssetImporter
        {
            foreach (Object o in objs)
            {
                ForEach(AssetDatabase.GetAssetPath(o), action, isUsed);
            }
        }

        public static IEnumerator ForEachSelectASync<T>(System.Action<T> action, System.Func<string, bool> isUsed = null, IEnumerator ator_end = null) where T : AssetImporter
        {
            foreach (Object o in Selection.objects)
            {
                IEnumerator itor = ForEachAsync(AssetDatabase.GetAssetPath(o), action, isUsed, ator_end);
                while (itor.MoveNext())
                    yield return 0;
            }
        }

        public class ForEachInfo
        {
            public int total = 0;
            public int current = 0;
        }

        public static IEnumerator ForEachAsync<T>(string path, System.Action<T> action, System.Func<string, bool> isUsed = null, IEnumerator ator_end = null, ForEachInfo info = null) where T : AssetImporter
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("非法的目录:" + path);
                yield break;
            }

            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            if (File.Exists(path))
            {
                // 是个文件
                T assetImporter = AssetImporter.GetAtPath(path) as T;
                if (assetImporter != null)
                {
                    if (info != null)
                    {
                        info.total = 1;
                        info.current = 0;
                    }

                    action(assetImporter);
                }
            }
            else
            {
                HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { path }));
                if (info != null)
                {
                    info.total = guids.Count;
                    info.current = -1;
                }

                List<string> ts = new List<string>();
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (info != null)
                    {
                        info.current++;
                        if ((info.current % 100) == 0)
                            yield return 0;
                    }

                    if (assetPath == null)
                        continue;

                    if (isUsed != null && !isUsed(assetPath))
                        continue;

                    T assetImporter = AssetImporter.GetAtPath(assetPath) as T;
                    if (assetImporter != null)
                    {
                        action(assetImporter);
                        yield return 0;
                    }
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            if (ator_end != null)
            {
                while (ator_end.MoveNext())
                    yield return 0;
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static List<Texture> GetMaterialTexture(Material mat)
        {
            List<Texture> texs = new List<Texture>();
            if (mat != null)
            {
                MaterialProperty[] props = MaterialEditor.GetMaterialProperties(new Object[1] { mat });
                foreach (MaterialProperty prop in props)
                {
                    if (prop.type == MaterialProperty.PropType.Texture)
                    {
                        if (mat.GetTexture(prop.name) != null)
                            texs.Add(mat.GetTexture(prop.name));
                    }
                }
            }

            return texs;
        }

        // 注意，此接口，只对png格式有效
        public static Texture2D CopyTexture2D(Texture2D t2d)
        {
            string filepath = Application.dataPath + "/" + AssetDatabase.GetAssetPath(t2d).Substring(7);
            if (!System.IO.File.Exists(filepath))
                return null;
            Texture2D _tex2 = new Texture2D(128, 128);
            _tex2.LoadImage(System.IO.File.ReadAllBytes(filepath));
            return _tex2;
        }

        public static void SetTextureWriteReader(Texture2D tx)
        {
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if (ai != null && ai is TextureImporter)
            {
                TextureImporter textureImporter = (TextureImporter)ai;
                if (textureImporter.isReadable != true)
                {
                    textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
                }
            }
        }

        public static bool GetTextureWriteReader(Texture2D tx)
        {
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if (ai != null && ai is TextureImporter)
            {
                TextureImporter textureImporter = (TextureImporter)ai;
                return textureImporter.isReadable;
            }

            return true;
        }

        public static Texture2D Texture2DCopy(Texture2D tx)
        {
            Texture2D copy = null;
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if (ai != null && ai is TextureImporter)
            {
                TextureImporter textureImporter = (TextureImporter)ai;
                bool isalter = false;
                if (textureImporter.isReadable != true)
                {
                    textureImporter.isReadable = true;
                    isalter = true;
                }

                bool isf = false;
                TextureImporterFormat f = textureImporter.textureFormat;
                {
                    if (f != TextureImporterFormat.AutomaticTruecolor)
                    {
                        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                        isf = true;
                    }
                }

                if (isf || isalter)
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));

                copy = (Texture2D)Object.Instantiate(tx);

                if (isalter)
                {
                    textureImporter.isReadable = false;
                }

                if (isf)
                {
                    textureImporter.textureFormat = f;
                }

                if (isf || isalter)
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
            }
            else
            {
                copy = Object.Instantiate(tx);
            }

            return copy;
        }

        public static Vector3 GetViewScenePosition()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                sceneView = (SceneView)(SceneView.sceneViews.Count == 0 ? null : SceneView.sceneViews[0]);

            if (sceneView == null || sceneView.camera == null)
                return Vector3.zero;

            Ray ray = sceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 10000f))
                return hitInfo.point;

            return ray.GetPoint(50f);
        }

        public static void ShowMonoScript(MonoBehaviour mb)
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mb), typeof(MonoScript), true);
        }

        static public List<string> GetAllFileList(string path)
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

                if (tmp.EndsWith(".meta") ||
                    tmp.Contains("__copy__/") ||
                    tmp.Contains(".svn/") ||
                    tmp.EndsWith(".dll") ||
                    tmp.EndsWith(".js") ||
                    tmp.EndsWith(".cs"))
                    continue;

                resList.Add(s);
            }
            return resList;
        }

        static public List<string> GetAllPrefabFile(string path)
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
                if (!tmp.EndsWith(".prefab")) continue;
                resList.Add(s);
            }
            return resList;
        }

        static public List<string> GetAllXmlFile(string path)
        {
            int startIndex = Application.dataPath.Length + 1;
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
                if (!tmp.EndsWith(".xml")) continue;
                resList.Add(s);
            }
            return resList;
        }

        static public List<Sprite> FindSprites(string assetPath)
        {
            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { assetPath }));

            Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

            List<Sprite> sprites = new List<Sprite>();
            Sprite s = null;
            foreach (string guid in guids)
            {
                string file = AssetDatabase.GUIDToAssetPath(guid);
                int pos = file.LastIndexOf('.');
                if (pos == -1)
                    continue;

                string suffix = file.Substring(pos + 1).ToLower();
                switch (suffix)
                {
                case "png":
                case "tga":
                case "psd":
                    break;

                case "prefab":
                case "mat":
                case "unity":
                case "assets":
                case "cs":
                case "js":
                case "dll":
                case "so":
                case "fbx":
                    continue;
                default:
                    break;
                }

                Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(file);
                foreach (Object o in objs)
                {
                    if (o is Sprite)
                    {
                        s = (Sprite)o;
                        sprites.Add(s);

                        if (Sprites.ContainsKey(s.name))
                        {
                            Debug.LogFormat("精灵名重复:{0}!", s.name);
                        }
                        else
                        {
                            Sprites.Add(s.name, s);
                        }
                    }
                }
            }

            return sprites;
        }

        static public List<Sprite> FindAllSprites()
        {
            return FindSprites("Assets");
        }

        static int mSizeFrame = -1;
        static System.Reflection.MethodInfo s_GetSizeOfMainGameView;
        static Vector2 mGameSize = Vector2.one;

        static public Vector2 screenSize
        {
            get
            {
                int frame = Time.frameCount;

                if (mSizeFrame != frame || !Application.isPlaying)
                {
                    mSizeFrame = frame;

                    if (s_GetSizeOfMainGameView == null)
                    {
                        System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                        s_GetSizeOfMainGameView = type.GetMethod("GetSizeOfMainGameView",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    }
                    mGameSize = (Vector2)s_GetSizeOfMainGameView.Invoke(null, null);
                }
                return mGameSize;
            }
        }
        public static void RestoreShader(Material[] materials, string key)
        {
            Shader shader = null;
            for (int j = 0; j < materials.Length; ++j)
            {
                Material mat = materials[j];
                if (mat != null && ((shader = mat.shader) != null))
                {
                    string n = shader.name;
                    if (n.EndsWith(key))
                    {
                        Shader s = Shader.Find(n.Remove(n.Length - key.Length));
                        if (s != null)
                            mat.shader = s;
                    }
                }
            }
        }

        public static void RelpaceShader(Material[] materials, string key)
        {
            for (int j = 0; j < materials.Length; ++j)
            {
                Shader shader;
                Material mat = materials[j];
                if (mat != null && ((shader = mat.shader) != null))
                {
                    if (!shader.name.EndsWith(key))
                    {
                        Shader s = Shader.Find(shader.name + key);
                        if (s != null)
                            mat.shader = s;
                    }
                }
            }
        }

        public static string ToMb(long bytes)
        {
            if (bytes <= 1024)
                return bytes + " b";

            float kb = bytes / 1024f;
            if (kb < 1024f)
                return kb.ToString("F1") + "k";

            return (bytes / (1024f * 1024f)).ToString("0.00") + "m";
        }

        //static public bool isCopyMat(Material mat)
        //{
        //    string name = mat.name;
        //    if (name.EndsWith("(Instance)", true, null) || name.EndsWith("(clone)", true, null))
        //        return true;

        //    return false;
        //}

        //static public Material CopyMaterial(Material mat)
        //{
        //    CopyMaterial(ref mat);
        //    return mat;
        //}

        //static public bool CopyMaterial(ref Material mat)
        //{
        //    if (mat == null)
        //        return false;

        //    if (isCopyMat(mat))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        Material copy = UnityEngine.Object.Instantiate(mat) as Material;
        //        Debuger.DebugLog("复制材质{0}->{1}", mat.name, copy.name);
        //        mat = copy;
        //        return true;
        //    }
        //}

        //public static Material[] GetCopyMaterial(Material[] mats)
        //{
        //    Material[] copys = new Material[mats.Length];
        //    for (int i = 0; i < mats.Length; ++i)
        //    {
        //        copys[i] = mats[i];
        //        CopyMaterial(ref copys[i]);
        //    }

        //    return copys;
        //}

        //public static void SetCopyMaterial(Material[] mats)
        //{
        //    for (int i = 0; i < mats.Length; ++i)
        //    {
        //        CopyMaterial(ref mats[i]);
        //    }
        //}

        public static bool ArrayRemove<T>(ref T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
                return false;

            T[] newv = new T[array.Length - 1];
            if (index == 0)
            {
                System.Array.Copy(array, 1, newv, 0, newv.Length);
            }
            else if (index == newv.Length)
            {
                System.Array.Copy(array, newv, newv.Length);
            }
            else
            {
                System.Array.Copy(array, newv, index);
                System.Array.Copy(array, index + 1, newv, index, array.Length - index - 1);
            }

            array = newv;
            return true;
        }

        public static bool Color32Equal(ref Color32 x, ref Color32 y)
        {
            return x.a == y.a && x.b == y.b && x.g == y.g && x.r == y.r;
        }
    }
}
#endif