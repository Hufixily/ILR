namespace game.IL
{
    using System.Collections.Generic;
    using System.Reflection;
    //IL中间类型
    public class CacheType
    {
        public CacheType(System.Type type)
        {
            this.type = type;
        }

        public System.Type type { get; private set; }//类型

        Dictionary<string, MemberInfo> nameToMethodBase = new Dictionary<string, MemberInfo>();//方法组

        List<FieldInfo> serializes;//序列化的字段

        struct Ctor
        {
            public System.Type param;
            public ConstructorInfo info;
        }

        Ctor[] ctors;
        ConstructorInfo defaultCtor;//默认构造
        bool isDefaultCtor = false;

        //返回默认构造器
        public ConstructorInfo GetCtor()
        {
            if (isDefaultCtor)
                return defaultCtor;
            isDefaultCtor = true;
            defaultCtor = type.GetConstructor(Help.Flags, null, Help.EmptyType, null);
            return defaultCtor;
        }
        //返回带参数的构造器
        public ConstructorInfo GetCtor(System.Type param)
        {
            int count = 0;
            if(null != ctors)
            {
                count = ctors.Length;
                foreach (var ctor in ctors)
                    if (ctor.param == param)
                        return ctor.info;
            }

            var p = param;
            while(null != p)
            {
                Help.OneType[0] = p;
                var ctor = type.GetConstructor(Help.Flags, null, Help.OneType, null);
                if(null != ctor)
                {
                    System.Array.Resize(ref ctors, count + 1);
                    ctors[count] = new Ctor() { param = param, info = ctor };
                    return ctor;
                }
                p = p.BaseType;
            }

            System.Array.Resize(ref ctors, count + 1);
            ctors[count] = new Ctor() { param = param, info = null };
            return null;
        }
        //获得方法
        public MethodInfo GetMethod(string name)
        {
            MemberInfo mb = null;
            if (nameToMethodBase.TryGetValue(name, out mb))
                return (MethodInfo)mb;

            System.Type st = type;
            while(true)
            {
                MethodInfo info = st.GetMethod(name, Help.Flags);
                if(null == info)
                {
                    if ((st = st.BaseType) != null)
                        continue;
                }
                nameToMethodBase.Add(name, info);
                return info;
            }
        }

        //获得属性
        public PropertyInfo GetProperty(string name)
        {
            MemberInfo mb = null;
            if (nameToMethodBase.TryGetValue(name, out mb))
                return (PropertyInfo)mb;

            System.Type st = type;
            while (true)
            {
               PropertyInfo info = st.GetProperty(name, Help.Flags);
                if (null == info)
                {
                    if ((st = st.BaseType) != null)
                        continue;
                }
                nameToMethodBase.Add(name, info);
                return info;
            }
        }

        //获得字段
        public FieldInfo GetField(string name)
        {
            MemberInfo mb = null;
            if (nameToMethodBase.TryGetValue(name, out mb))
                return (FieldInfo)mb;

            System.Type st = type;
            while (true)
            {
                FieldInfo info = st.GetField(name, Help.Flags);
                if (null == info)
                {
                    if ((st = st.BaseType) != null)
                        continue;
                }
                nameToMethodBase.Add(name, info);
                return info;
            }
        }

        //获得序列化数据
        public List<FieldInfo> GetSerializeField()
        {
            if(null == serializes)
            {
                serializes = new List<FieldInfo>();
                Help.GetSerializeField(type, serializes);
#if USE_HOT
                if(null != type.BaseType)
                  Help.GetSerializeField(type.BaseType, serializes);
#endif
            }
            return serializes;
        }
    }
}
