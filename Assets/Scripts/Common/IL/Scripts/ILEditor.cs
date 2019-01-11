#if UNITY_EDITOR && USE_HOT
using System.Reflection;
using ILRuntime.Runtime.Enviorment;

//热更环境启动类
namespace game.IL
{
    public class ILEditor
    {
        static AppDomain appdomain_;

        public static AppDomain appdomain
        {
            get
            {
                if (HotMgr.appdomain != null)
                {
                    return HotMgr.appdomain;
                }
                else if (appdomain_ == null)
                {
                    LoadDLL();
                }

                return appdomain_;
            }
        }

        [UnityEditor.MenuItem("Assets/LoadDLL")]
        static void LoadDLL()
        {
#if USE_HOT
            appdomain_ = new AppDomain();
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            //获取全部绑定类型
            System.Type clrType = System.Type.GetType("ILRuntime.Runtime.Generated.CLRBindings");
            if (null != clrType)
            {
                clrType.GetMethod("Initialize").Invoke(null, new object[] { appdomain });
            }
            //加载dll
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream("Assets/StreamingAssets/DyncDll.dll", System.IO.FileMode.Open))
                {
#if USE_PDB
                    using (System.IO.FileStream p = new System.IO.FileStream("Assets/StreamingAssets/DyncDll.pdb", System.IO.FileMode.Open))
                    {
                        appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
#else
                    appdomain_.LoadAssembly(fs);
#endif
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
#endif
        }
    }
}
#endif