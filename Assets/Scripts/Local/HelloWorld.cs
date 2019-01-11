namespace game
{
    using game.IL;
    using UnityEngine;

#if USE_HOT
    using ILRuntime.CLR.Utils;
    using ILRuntime.Runtime.Stack;
    using ILRuntime.Runtime.Enviorment;
    using ILRuntime.Runtime.Intepreter;
#endif

    public class HelloWorld : MonoBehaviour
    {
        RefType m_HotType;

#if USE_HOT
        public AppDomain appdomain { get { return game.IL.HotMgr.appdomain; } }
#endif

        private void Start()
        {
#if USE_HOT
            HotMgr.Init();
#endif
        }

        public void OnCreateHotObject()
        {
            Debug.Log("创建HotType对象");
            m_HotType = new RefType("hot.HotHelloWorld");
            if (null != m_HotType)
                Debug.Log("HotType对象创建成功");
        }

        public void OnLocalLog()
        {
            Debug.Log("LocalLog : Hello World");
        }

        public void OnHotLog()
        {
            if(null == m_HotType)
            {
                Debug.LogError("HotType对象未创建");
                return;
            }
            m_HotType.InvokeMethod("SayHello");
        }
    }
}

