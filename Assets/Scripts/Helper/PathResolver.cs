using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace game.Assets
{
    /// <summary>
    /// 路径解决器
    /// </summary>
    public static class PathResolver
    {
        /// <summary>
        /// AB 保存的文件夹路径
        /// </summary>
        public static string BundleSaveDirName { get { return "SealySmb"/*Application.productName*//*"AssetBundles"*/; } }
        /// <summary>
        /// AB 保存的路径
        /// </summary>
        public static string BundleSavePath { get { return Path.Combine(BundleSaveDirName, GetPlatformName()); } }
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器环境下打包路径
        /// </summary>
        public static readonly string BUILD_PATH = System.IO.Directory.GetCurrentDirectory() + "\\" + BundleSavePath + "\\";
        /// <summary>
        /// 编辑器环境下的主文件路径
        /// </summary>
        public static readonly string EDITOR_MAIN_MANIFEST_FILE_PATH = BUILD_PATH + "/" + MAIN_MANIFEST_FILE_NAME;
        /// <summary>
        /// 编辑器环境下的资源文件路径
        /// </summary>
        public static readonly string EDITOR_RESOURCE_MANIFEST_FILE_PATH = BUILD_PATH + "/" + RESOURCES_MANIFEST_FILE_NAME;
        /// <summary>
        /// AB打包的原文件HashCode要保存到的路径，下次可供增量打包
        /// </summary>
        public static string HashCacheSaveFile { get { return Path.Combine(BundleSavePath, AssetCacheFileName); } }// "Assets/StreamingAssets/cache.txt"; } }
        /// <summary>
        /// 在编辑器模型下将 abName 转为 Assets/... 路径
        /// 这样就可以不用打包直接用了
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string GetEditorModePath(string abName)
        {
            //将 Assets.AA.BB.prefab 转为 Assets/AA/BB.prefab
            abName = abName.Replace(".", "/");
            int last = abName.LastIndexOf("/");

            if (last == -1)
                return abName;

            string path = string.Format("{0}.{1}", abName.Substring(0, last), abName.Substring(last + 1));
            return path;
        }
#endif

        /// <summary>
        /// 获取 AB 源文件路径
        /// </summary>
        public static string GetBundleSourceFile(string path, bool forWWW = true)
        {
            string filePath = null;
            var platformName = GetPlatformName();
#if ASSET_UPDATER
            filePath = PATH + "/" + path;
#elif UNITY_EDITOR
            if (forWWW)
                filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platformName, path);
            else
                filePath = string.Format("{0}/StreamingAssets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName,platformName, path);
#elif UNITY_ANDROID
            if (forWWW)
                filePath = string.Format("jar:file://{0}!/assets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName,platformName, path);
            else
                filePath = string.Format("{0}!assets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName,platformName, path);
#elif UNITY_IOS
            if (forWWW)
                filePath = string.Format("file://{0}/Raw/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName,platformName, path);
            else
                filePath = string.Format("{0}/Raw/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName,platformName, path);
#else
            filePath = Application.streamingAssetsPath; 
#endif
            return filePath;
        }
        /// <summary>
        /// 获取 主文件路径
        /// </summary>
        public static string GetFileSourceFile(string path,bool forWWW = true)
        {
            var filePath = string.Empty;
            var platform = GetPlatformName();
#if ASSET_UPDATER
            filePath = PATH + "/" + path;
#elif UNITY_EDITOR
            if (forWWW)
                filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform,path);
            else
                filePath = string.Format("{0}/StreamingAssets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform, path);
#elif UNITY_ANDROID
            if (forWWW)
                filePath = string.Format("jar:file://{0}!/assets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform,path);
            else
                filePath = string.Format("{0}!assets/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform,path);
#elif UNITY_IOS
            if (forWWW)
                filePath = string.Format("file://{0}/Raw/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform,path);
            else
                filePath = string.Format("{0}/Raw/{1}/{2}/{3}", Application.dataPath, BundleSaveDirName, platform,path);
#else
            filePath = Application.streamingAssetsPath; 
#endif
            return filePath;
        }
        /// <summary>
        /// 主文件后缀
        /// </summary>
        public static string FileExtension = ".all";
        /// <summary>
        /// 主资源文件后缀
        /// </summary>
        public static string ManiFestExtension = ".manifest";

        /// <summary>
        /// 主资源文件名
        /// </summary>
        public static string MainManifestFileName { get { return MAIN_MANIFEST_FILE_NAME + ManiFestExtension; } }
        /// <summary>
        /// AB 依赖信息文件名
        /// </summary>
        public static string DependFileName { get { return MAIN_DEP_FILE_NAME+ FileExtension; } }
        /// <summary>
        /// 精灵映射信息文件名
        /// </summary>
        public static string SpriteFileName { get { return MAIN_SPRITE_FILE_NAME + FileExtension; } }
        /// <summary>
        /// 资源名字映射信息文件名
        /// </summary>
        public static string AssetFileName { get { return MAIN_ASSET_FILE_NAME + FileExtension; } }
        /// <summary>
        /// 资源列表缓存文件名
        /// </summary>
        public static string AssetCacheFileName { get { return MAIN_CACHE_FILE_NAME + FileExtension; } }
        
        static DirectoryInfo cacheDir;
        /// <summary>
        /// 用于缓存AB的目录，要求可写
        /// </summary>
        public static string BundleCacheDir
        {
            get
            {
                if (cacheDir == null)
                {
#if ASSET_UPDATER
                    string dir = PATH;
#elif UNITY_EDITOR
                    string dir = string.Format("{0}/{1}", Application.streamingAssetsPath, BundleSavePath);
#else
					string dir = string.Format("{0}/{1}", Application.persistentDataPath,BundleSaveDirName);
#endif
                    cacheDir = new DirectoryInfo(dir);
                    if (!cacheDir.Exists) cacheDir.Create();
                }
                return cacheDir.FullName;
            }
        }

        public static string GetPlatformName()
        {
            return MAIN_MANIFEST_FILE_NAME;
        }

        /// <summary>
        /// DLL常驻外部路径
        /// </summary>
        public static readonly string DLL_PATH = Application.dataPath + "/PersistentAssets" + "/" + BundleSaveDirName + "/";

        /// <summary>
        /// 外部资源常驻根路径
        /// </summary>
        public static readonly string PATH = Application.dataPath + "/PersistentAssets" + "/" + BundleSaveDirName + "/" + GetPlatformName();

        /// <summary>
        /// 包内资源
        /// </summary>
        public static readonly string INITIAL_PATH = Application.streamingAssetsPath + "/" + BundleSaveDirName + "/" + GetPlatformName();

        /// <summary>
        ///外部资源缓存路径(下载，拷贝)
        /// </summary>
        public static readonly string CACHE_PATH = PATH + "/Cache";

        /// <summary>
        /// 外部缓存数据路径（分享）
        /// </summary>
        public static readonly string CACHE_DATA_PATH = Application.dataPath + "/PersistentAssets";

        /// <summary>
        /// 下载文件配置路径
        /// </summary>
        public static readonly string DOWNLOADCACHE_FILE_PATH = CACHE_PATH + "/DownloadConfig.cfg";

        /// <summary>
        /// 主Manifest文件
        /// </summary>
        public const string MAIN_MANIFEST_FILE_NAME = "AssetBundle";
        /// <summary>
        /// ResourcesManifest文件
        /// </summary>
        public const string RESOURCES_MANIFEST_FILE_NAME = "ResourcesManifest.cfg";

        /// <summary>
        /// 哈希配置表（用于检测更新）
        /// </summary>
        public const string MAIN_CACHE_FILE_NAME = "cache";
        /// <summary>
        /// AB依赖表
        /// </summary>
        public const string MAIN_DEP_FILE_NAME = "dep";
        /// <summary>
        /// 精灵图集映射表
        /// </summary>
        public const string MAIN_SPRITE_FILE_NAME = "sprite";
        /// <summary>
        /// 资源名称映射表
        /// </summary>
        public const string MAIN_ASSET_FILE_NAME = "asset";

        /// <summary>
        /// 此系统用到的主文件，可扩展
        /// </summary>
        public static readonly string[] MAIN_CONFIG_NAME_ARRAY =
        {
            MAIN_MANIFEST_FILE_NAME,//主文件
            RESOURCES_MANIFEST_FILE_NAME,//资源文件
            DependFileName,//依赖文件
            SpriteFileName,//精灵映射
            AssetFileName,//资源映射
            AssetCacheFileName,//资源哈希文件
        };

        /// <summary>
        /// 获取外部路径
        /// </summary>
        public static string GetFileFullName(string file)
        {
            return PATH + "/" + file;
        }

        /// <summary>
        /// 获取外部缓存路径
        /// </summary>
        public static string GetCacheFileFullName(string path)
        {
            return CACHE_PATH + "/" + path;
        }

        /// <summary>
        /// 获取包内路径资源
        /// </summary>
        public static string GetInitialFileFullName(string path)
        {
            return INITIAL_PATH + "/" + path;
        }

        /// <summary>
        /// 加载MainManifest文件
        /// </summary>
        public static AssetBundleManifest LoadMainManifest()
        {
            var path = GetFileFullName(MAIN_MANIFEST_FILE_NAME);
            return LoadMainManifestByPath(path);
        }
        public static AssetBundleManifest LoadCacheMainManifest()
        {
            var path = GetCacheFileFullName(MAIN_MANIFEST_FILE_NAME);
            return LoadMainManifestByPath(path);
        }

        /// <summary>
        /// 加载MainManifest文件
        /// </summary>
        public static AssetBundleManifest LoadMainManifestByPath(string path)
        {
            if (!File.Exists(path))
                return null;
            AssetBundleManifest manifest = null;
            var manifestBundle = AssetBundle.LoadFromFile(path);
            if (manifestBundle == null)
                return null;
            manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
            manifestBundle.Unload(false);
            return manifest;
        }
        /// <summary>
        /// 加载ResourcesManifest文件
        /// </summary>
//         public static ResourcesManifest LoadResourceManifestByPath(string path)
//         {
//             var result = new ResourcesManifest();
//             result.Load(path);
//             return result;
//         }

        /// <summary>
        /// 加载全局ResourcesManifest文件
        /// </summary>
//         public static ResourcesManifest LoadResourceManifest()
//         {
//             var path = GetFileFullName(RESOURCES_MANIFEST_FILE_NAME);
//             return LoadResourceManifestByPath(path);
//                
/// <summary>
        /// 拷贝本地资源到全局路径
        /// </summary>
        public static IEnumerator CopyInitialFileFile(string file)
        {
            yield return FileHelper.CopyStreamingAssetsToFile(GetInitialFileFullName(file), GetFileFullName(file));
        }
        public static string CalcAssetBundleDownloadURL(string url)
        {
            var value = url;
            if (value[value.Length - 1] != '/')
                value = value + '/';
            value = value + BundleSaveDirName + "/" + GetPlatformName() + "/";
            return value;
        }
    }
}