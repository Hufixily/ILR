#if USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace game.IL.Editor
{
    /// <summary>
    /// CLR绑定
    /// </summary>
    public static class GenerateCLRBinding
    {
        [MenuItem("IL/导出CLR绑定")]
        static void Generate()
        {
            HashSet<System.Type> types = new HashSet<System.Type>();
            //基础类型
            types.Add(typeof(int));
            types.Add(typeof(float));
            types.Add(typeof(long));
            types.Add(typeof(object));
            types.Add(typeof(string));
            types.Add(typeof(System.Array));
            types.Add(typeof(Vector2));
            types.Add(typeof(Vector3));
            types.Add(typeof(Quaternion));
            types.Add(typeof(GameObject));
            types.Add(typeof(Object));
            types.Add(typeof(Transform));
            types.Add(typeof(RectTransform));
            types.Add(typeof(Time));
            types.Add(typeof(RefType));
            types.Add(typeof(System.Type));
            types.Add(typeof(System.Reflection.MethodInfo));
            types.Add(typeof(System.Reflection.FieldInfo));
            types.Add(typeof(System.Reflection.PropertyInfo));
            //日志输出部分
            types.Add(typeof(Debug));
            types.Add(typeof(Debug));
            types.Add(typeof(Logger));
            //types.Add(typeof(Log));
            
            //所有DLL内的类型的真实C#类型都是ILTypeInstance
            types.Add(typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance));
            types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

            //types.Add(typeof(UIMakeNumber));

            //game engine
            //types.Add(typeof(HotMgr));
            //types.Add(typeof(GameApp));
            //types.Add(typeof(ui.UISystem));
           // types.Add(typeof(ModuleBase));

            //types.Add(typeof(EventSet));
            //types.Add(typeof(EventArgs));
            //types.Add(typeof(EventAgent));
            //types.Add(typeof(game.UIEventAgent));
            //types.Add(typeof(core.EventDispatch));
            //types.Add(typeof(game.TimeMgr));
            //types.Add(typeof(LocalPlayer));
            //types.Add(typeof(game.Main));
            //types.Add(typeof(game.Coroutine));

            // types.Add(typeof(UIGroup));
            //types.Add(typeof(xys.UI.State.StateRoot));

            //types.Add(typeof(game.PanelAnimator));
            //types.Add(typeof(game.GameStateBase));

           // types.Add(typeof(UnityEngine.UI.EmptyGraphic));

           // types.Add(typeof(game.NetSender));
            //types.Add(typeof(game.NetReceiver));
           // types.Add(typeof(game.NetCore));
            
            //Proto
            //types.Add(typeof(Sproto.Protocol));
           // types.Add(typeof(Sproto.ProtocolBase));
            
           // types.Add(typeof(CoinHitData));

            //
            types.Add(typeof(System.Math));
            types.Add(typeof(System.Convert));
            types.Add(typeof(System.DateTime));
            //types.Add(typeof(System.TimeSpan));
            types.Add(typeof(System.String));
            types.Add(typeof(System.Text.StringBuilder));
            types.Add(typeof(Dictionary<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(Dictionary<int, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(Dictionary<uint, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(Dictionary<long, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(Dictionary<ulong, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(Dictionary<string, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
            types.Add(typeof(System.Text.RegularExpressions.Regex));
            types.Add(typeof(System.Text.RegularExpressions.Group));
            types.Add(typeof(System.Text.RegularExpressions.Match));
            types.Add(typeof(System.Text.RegularExpressions.CaptureCollection));
            types.Add(typeof(System.Text.RegularExpressions.GroupCollection));
            types.Add(typeof(UnityEngine.UI.LayoutElement));
            types.Add(typeof(UnityEngine.UI.Text));
            types.Add(typeof(UnityEngine.UI.Image));
            types.Add(typeof(UnityEngine.UI.Graphic));
            types.Add(typeof(UnityEngine.EventSystems.UIBehaviour));
            types.Add(typeof(UnityEngine.MonoBehaviour));
            types.Add(typeof(UnityEngine.Color));
            types.Add(typeof(HashSet<long>));
            types.Add(typeof(HashSet<int>));
            types.Add(typeof(System.Linq.Enumerable));
            types.Add(typeof(Canvas));
            types.Add(typeof(Matrix4x4));
            types.Add(typeof(UnityEngine.Video.VideoPlayer));
            types.Add(typeof(System.Enum));
            types.Add(typeof(Screen));

            AssetDatabase.Refresh();

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(new List<System.Type>(types), "Assets/Scripts/Common/IL/Auto");
        }
    }
}
#endif