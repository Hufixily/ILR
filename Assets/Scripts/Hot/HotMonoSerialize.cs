#if !USE_HOT
namespace hot
{
    using game.IL;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    class TestValue
    {
        [SerializeField]
        int[] m_IntValues;
        [SerializeField]
        List<string> m_StrValues;

        public int test { get { return m_IntValues[0]; } }
    }

    [AutoILMono]
    class HotMonoSerialize
    {
        [SerializeField]
        int[] m_IntValues; 

        [SerializeField]
        List<string> m_StrValues;

        [SerializeField]
        TestValue[] m_ClassArray;

//         [SerializeField]
//         List<TestValue> m_ClassList;

        void Start()
        {
            Debug.LogFormat("intValue : {0} ", m_IntValues[0]);
            Debug.LogFormat("StrValue : {0} ", m_StrValues[0]);
            Debug.LogFormat("m_ClassArray : {0} ", m_ClassArray[0].test);
        }
    }
}
#endif
