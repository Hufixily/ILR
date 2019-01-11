namespace game.Assets
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public static class FileHelper 
    {
        public static bool CopyDirectory(string src,string dest,bool overwrite = false)
        {
            try
            {
                var dir = new DirectoryInfo(src);
                var fileInfo = dir.GetFileSystemInfos();
                foreach(var i in fileInfo)
                {
                    var destPath = dest + "\\" + i.Name;
                    if (i is DirectoryInfo)
                    {
                        if (!Directory.Exists(destPath))
                            Directory.CreateDirectory(destPath);
                        CopyDirectory(i.FullName, destPath);
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath, overwrite);
                    }
                }
                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        public static bool CopyFile(string src,string dest,bool overwrite = false)
        {
            if (!File.Exists(src))
                return false;

            var dire = Path.GetDirectoryName(dest);
            if (!Directory.Exists(dire))
                Directory.CreateDirectory(dire);

            File.Copy(src, dest, overwrite);
            return true;
        }
        /// <summary>
        /// 判断是否可忽略的扩展名
        /// </summary>
        public static bool IgnoreFile(string fileName,string[] ignores)
        {
            if (ignores == null)
                return false;

            var extension = System.IO.Path.GetExtension(fileName);
            for(int i = 0; i < ignores.Length;i++)
            {
                if (ignores[i] == extension)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 获取除去后缀的路径
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string GetPathWithoutExtension(string fullName)
        {
            return Path.GetFileNameWithoutExtension(fullName);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public static void WriteBytesToFile(string path, byte[] bytes, int len)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var info = new FileInfo(path);
            using (var sw = info.Open(FileMode.Create, FileAccess.ReadWrite))
            {
                if(bytes != null && len > 0)
                {
                    sw.Write(bytes, 0, len);
                }
            }
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        public static void WriteTextToFile(string path,string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            WriteBytesToFile(path, bytes, bytes.Length);
        }
        /// <summary>
        /// 拷贝文件
        /// </summary>
        public static IEnumerator CopyStreamingAssetsToFile(string src,string dest)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IPHONE
            src = "file:///" + src;
#endif
            using (var w = new WWW(src))
            {
                yield return w;
                if(string.IsNullOrEmpty(w.error))
                {
                    while (w.isDone == false)
                        yield return null;
                    WriteBytesToFile(dest, w.bytes, w.bytes.Length);
                }
                else
                {
                    Debug.LogError(w.error);
                }
            }
        }

        public static long GetFileSize(string fileName)
        {
            long size = 0;
            if (!File.Exists(fileName))
            {
                return size;
            }
            else
            {
                var info = new FileInfo(fileName);
                return info.Length;
            }
        }
    }
}

