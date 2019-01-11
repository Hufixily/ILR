// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// 
// namespace game
// {
// 	namespace editor
// 	{
//         public class EditorMacros
//         {
//             [MenuItem("Tool/Macros/Debug/打开调试宏")]
//             private static void EditorEnableGM()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Add("COM_DEBUG");
//                 macroDefine.Save();
//             }
// 
//             [MenuItem("Tool/Macros/Debug/关闭调试宏")]
//             private static void EditorDisableGM()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Remove("COM_DEBUG");
//                 macroDefine.Save();
//             }
// 
//             [MenuItem("Tool/Macros/Asset/Open")]
//             private static void BuildWithServerRes()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Add("COM_AB_MODE");
//                 macroDefine.Add("_AB_MODE_");
//                 macroDefine.Add("AB_MODE");
//                 macroDefine.Save();
//             }
//             [MenuItem("Tool/Macros/Asset/Close")]
//             private static void BuildWithNoServerRes()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Remove("COM_AB_MODE");
//                 macroDefine.Remove("_AB_MODE_");
//                 macroDefine.Remove("AB_MODE");
//                 macroDefine.Save();
//             }
// 
//             //本地测试模式
//             [MenuItem("Tool/Macros/Net/Open")]
//             private static void EnableNetWork()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Add("NET_MODE");
//                 macroDefine.Save();
//             }
//             [MenuItem("Tool/Macros/Net/Close")]
//             private static void DisableNetWork()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Remove("NET_MODE");
//                 macroDefine.Save();
//             }
// 
//             //本地测试模式
//             [MenuItem("Tool/Macros/LocalGame/Open")]
//             private static void EnablePlay()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Add("BATTLE_TEST");
//                 macroDefine.Save();
//             }
//             [MenuItem("Tool/Macros/LocalGame/Close")]
//             private static void DisablePlay()
//             {
//                 MacroDefine macroDefine = new MacroDefine();
//                 macroDefine.Remove("BATTLE_TEST");
//                 macroDefine.Save();
//             }
// 
//             //资源更新模式
//             [MenuItem("Tool/Macros/AssetUpdater/Open")]
//             private static void EnableUpdater()
//             {
//                 var d = new MacroDefine();
//                 d.Add("ASSET_UPDATER");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/AssetUpdater/Close")]
//             private static void DisableUpdater()
//             {
//                 var d = new MacroDefine();
//                 d.Remove("ASSET_UPDATER");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/AssetURL/Open")]
//             private static void EnableAssetURL()
//             {
//                 var d = new MacroDefine();
//                 d.Add("ASSET_URL");
//                 d.Save();
//             }
//             [MenuItem("Tool/Macros/AssetURL/Close")]
//             private static void DisableAssetURL()
//             {
//                 var d = new MacroDefine();
//                 d.Remove("ASSET_URL");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/GM/Open")]
//             private static void EnableGM()
//             {
//                 var d = new MacroDefine();
//                 d.Add("GM");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/GM/Close")]
//             private static void DisableGM()
//             {
//                 var d = new MacroDefine();
//                 d.Remove("GM");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/NativeCall/Open")]
//             private static void EnableNativeCall()
//             {
//                 var d = new MacroDefine();
//                 d.Add("NATIVE_CALL");
//                 d.Save();
//             }
// 
//             [MenuItem("Tool/Macros/NativeCall/Close")]
//             private static void DisableNativeCall()
//             {
//                 var d = new MacroDefine();
//                 d.Remove("NATIVE_CALL");
//                 d.Save();
//             }
//         }
//     }
// }
// #endif
