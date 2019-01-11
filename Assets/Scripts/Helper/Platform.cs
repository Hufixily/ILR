// namespace game.Assets
// {
//     using UnityEngine;
// 
//     public static class Platform 
//     {
// #if UNITY_EDITOR || UNITY_STANDALONE_WIN
//         public static readonly string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
//         public static readonly string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
//         public static readonly string CACHE_DATA_PATH = Application.dataPath + "/PersistentAssets";
// #elif UNITY_IPHONE ||  UNITY_ANDROID
//         public static readonly string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
//         public static readonly string PERSISTENT_DATA_PATH = Application.persistentDataPath;
//         public static readonly string CACHE_DATA_PATH = Application.persistentDataPath;
// #endif
//     }
// 