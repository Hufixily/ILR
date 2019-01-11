#if USE_HOT
namespace game.IL
{
    using System.Reflection;
    using ILRuntime.CLR.Utils;
    using ILRuntime.Runtime.Stack;
    using ILRuntime.Runtime.Enviorment;
    using ILRuntime.Runtime.Intepreter;
    using System.Collections.Generic;

    using game.IL;

    public static class HotMgr
    {
        //热更实例(唯一)，其他地方禁止实例化此对象
        public static AppDomain appdomain { get; private set; }

        static RefType m_refType;

        public static void Init()
        {
#if UNITY_ANDROID
            ResLoad.Set(new AndroidResLoad());
#elif UNITY_EDITOR
            ResLoad.Set(new EditorResLoad());
#endif
            //初始化热更模块
            InitHotModule();
#if COM_DEBUG
            //开启调试端口
            appdomain.DebugService.StartDebugService(56000);
#endif
        }

        public const BindingFlags m_bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        static public void InitHotModule()
        {
            if (null != appdomain)
                return;
            appdomain = new AppDomain();
            //跨域继承注册
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            //获取全部绑定类型
            {
                var clrType = System.Type.GetType("ILRuntime.Runtime.Generated.CLRBindings");
                if (null != clrType)
                {
                    clrType.GetMethod("Initialize").Invoke(null, new object[] { appdomain });
                }
            }

#region 加载DLL
            try
            {
                //using (var fs = ResLoad.GetStream("DyncDll.dll"))

                using (var fs = ResLoad.GetStream("Hotfix/Hotfix.dll"))
                {
#if USE_PDB
                    using (var p = ResLoad.GetStream("DyncDll.pdb"))
                    {
                        appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
#else
                    appdomain.LoadAssembly(fs);
#endif
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
            #endregion

            RegDelegate(appdomain);
            Help.Init();
        }

        public static void RegDelegate(AppDomain appdomain)
        {

            #region 注册绑定类型
            // 热更代码当中使用List<T>类型,注册下
            // List<T>
            {
                appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
                appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();

                appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
                appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
                {
                    return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                    {
                        return ((System.Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, int>)act)(x, y);
                    });
                });
            }

            appdomain.DelegateManager.RegisterMethodDelegate<object>();
            appdomain.DelegateManager.RegisterMethodDelegate<object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<long>();
            appdomain.DelegateManager.RegisterMethodDelegate<ulong>();
            appdomain.DelegateManager.RegisterMethodDelegate<int>();
            appdomain.DelegateManager.RegisterMethodDelegate<uint>();
            appdomain.DelegateManager.RegisterMethodDelegate<short>();
            appdomain.DelegateManager.RegisterMethodDelegate<ushort>();
            appdomain.DelegateManager.RegisterMethodDelegate<char>();
            appdomain.DelegateManager.RegisterMethodDelegate<string>();
            #endregion

            #region 注册委托类型
            {
                var clrType = System.Type.GetType("AutoIL.ILRegType");
                if (null != clrType)
                {
                    clrType.GetMethod("RegisterFunctionDelegate").Invoke(null, new object[] { appdomain });
                    clrType.GetMethod("RegisterDelegateConvertor").Invoke(null, new object[] { appdomain });
                    clrType.GetMethod("RegisterMethodDelegate").Invoke(null, new object[] { appdomain });
                }
            }
            #endregion
        }

        //替换相关
        //方法
        public static bool ReplaceFunction(System.Type type,string functionName,MethodInfo info)
        {
            string flag = "";
            string name = functionName;
            if (functionName == ".ctor")
            {
                flag = "_c";
                name = "ctor";
            }

            bool isset = false;
            for (int idx = 0; idx < 99; ++idx)
            {
                string fieldName = string.Format("{0}__Hotfix{1}_{2}", flag, idx, name);
                var field = type.GetField(fieldName, m_bindingFlags);
                if (field == null)
                    break;

                isset = true;

                if (info != null)
                {
                    field.SetValue(null, new global::IL.DelegateBridge(info));
                    UnityEngine.Debug.LogFormat("ReplaceFunction type:{0} name:{1}", type.Name, functionName);
                }
                else
                {
                    field.SetValue(null, null);
                    UnityEngine.Debug.LogFormat("ReplaceFunction type:{0} name:{1} Cannel!", type.Name, functionName);
                }
            }

            if (!isset)
            {
                UnityEngine.Debug.LogErrorFormat("ReplaceFunction type:{0} name:{1} not find!", type.Name, functionName);
            }

            return isset;
        }

        //字段替换
        public static bool ReplaceField(System.Type type, string fieldName, MethodInfo info)
        {
            var field = type.GetField(fieldName, m_bindingFlags);
            if (field == null)
            {
                UnityEngine.Debug.LogErrorFormat("ReplaceField type:{0} fieldName:{1} not find!", type.Name, fieldName);
                return false;
            }

            field.SetValue(null, new global::IL.DelegateBridge(info));
            UnityEngine.Debug.LogFormat("ReplaceFunction type:{0} fieldName:{1}", type.Name, fieldName);
            return true; ;
        }
    }
}
#endif