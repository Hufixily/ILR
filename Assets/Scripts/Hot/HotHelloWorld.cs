#if !USE_HOT
namespace hot
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    class HotHelloWorld
    {
        void SayHello()
        {
            Debug.Log("Hello AutoDLL ILRuntime");
        }
    }
}
#endif