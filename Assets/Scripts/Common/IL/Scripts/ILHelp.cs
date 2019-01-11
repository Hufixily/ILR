namespace game.IL
{
    using System.Collections.Generic;
    using System.Reflection;

#if USE_HOT
    using Mono.Cecil;
    using ILRuntime.Reflection;
    using ILRuntime.CLR.TypeSystem;
    using ILRuntime.Runtime.Intepreter;
#endif

    public static partial class Help
    {
        static void X()
        {
            System.Type v = null;
            // List
            {
                v = typeof(List<object>);
                v = typeof(List<char>);
                v = typeof(List<short>);
                v = typeof(List<ushort>);
                v = typeof(List<int>);
                v = typeof(List<uint>);
                v = typeof(List<long>);
                v = typeof(List<ulong>);
                v = typeof(string);

                v = typeof(object[]);
                v = typeof(char[]);
                v = typeof(short[]);
                v = typeof(ushort[]);
                v = typeof(int[]);
                v = typeof(uint[]);
                v = typeof(long[]);
                v = typeof(ulong[]);
                v = typeof(string[]);
            }

            v = typeof(Dictionary<object, object>);

            // map<char,>
            {
                v = typeof(Dictionary<char, object>);
                v = typeof(Dictionary<char, char>);
                v = typeof(Dictionary<char, short>);
                v = typeof(Dictionary<char, ushort>);
                v = typeof(Dictionary<char, int>);
                v = typeof(Dictionary<char, uint>);
                v = typeof(Dictionary<char, long>);
                v = typeof(Dictionary<char, ulong>);
                v = typeof(Dictionary<char, string>);
                v = typeof(Dictionary<char, bool>);
            }
            // map<short,>
            {
                v = typeof(Dictionary<short, object>);
                v = typeof(Dictionary<short, char>);
                v = typeof(Dictionary<short, short>);
                v = typeof(Dictionary<short, ushort>);
                v = typeof(Dictionary<short, int>);
                v = typeof(Dictionary<short, uint>);
                v = typeof(Dictionary<short, long>);
                v = typeof(Dictionary<short, ulong>);
                v = typeof(Dictionary<short, string>);
                v = typeof(Dictionary<short, bool>);
            }
            // map<ushort,>
            {
                v = typeof(Dictionary<ushort, object>);
                v = typeof(Dictionary<ushort, char>);
                v = typeof(Dictionary<ushort, short>);
                v = typeof(Dictionary<ushort, ushort>);
                v = typeof(Dictionary<ushort, int>);
                v = typeof(Dictionary<ushort, uint>);
                v = typeof(Dictionary<ushort, long>);
                v = typeof(Dictionary<ushort, ulong>);
                v = typeof(Dictionary<ushort, string>);
                v = typeof(Dictionary<ushort, bool>);
            }
            // map<int,>
            {
                v = typeof(Dictionary<int, object>);
                v = typeof(Dictionary<int, char>);
                v = typeof(Dictionary<int, short>);
                v = typeof(Dictionary<int, ushort>);
                v = typeof(Dictionary<int, int>);
                v = typeof(Dictionary<int, uint>);
                v = typeof(Dictionary<int, long>);
                v = typeof(Dictionary<int, ulong>);
                v = typeof(Dictionary<int, string>);
                v = typeof(Dictionary<int, bool>);
            }
            // map<uint,>
            {
                v = typeof(Dictionary<uint, object>);
                v = typeof(Dictionary<uint, char>);
                v = typeof(Dictionary<uint, short>);
                v = typeof(Dictionary<uint, ushort>);
                v = typeof(Dictionary<uint, int>);
                v = typeof(Dictionary<uint, uint>);
                v = typeof(Dictionary<uint, long>);
                v = typeof(Dictionary<uint, ulong>);
                v = typeof(Dictionary<uint, string>);
                v = typeof(Dictionary<uint, bool>);
            }
            // map<long,>
            {
                v = typeof(Dictionary<long, object>);
                v = typeof(Dictionary<long, char>);
                v = typeof(Dictionary<long, short>);
                v = typeof(Dictionary<long, ushort>);
                v = typeof(Dictionary<long, int>);
                v = typeof(Dictionary<long, uint>);
                v = typeof(Dictionary<long, long>);
                v = typeof(Dictionary<long, ulong>);
                v = typeof(Dictionary<long, string>);
                v = typeof(Dictionary<long, bool>);
            }
            // map<ulong,>
            {
                v = typeof(Dictionary<ulong, object>);
                v = typeof(Dictionary<ulong, char>);
                v = typeof(Dictionary<ulong, short>);
                v = typeof(Dictionary<ulong, ushort>);
                v = typeof(Dictionary<ulong, int>);
                v = typeof(Dictionary<ulong, uint>);
                v = typeof(Dictionary<ulong, long>);
                v = typeof(Dictionary<ulong, ulong>);
                v = typeof(Dictionary<ulong, string>);
                v = typeof(Dictionary<ulong, bool>);
            }
            // map<string,>
            {
                v = typeof(Dictionary<string, object>);
                v = typeof(Dictionary<string, char>);
                v = typeof(Dictionary<string, short>);
                v = typeof(Dictionary<string, ushort>);
                v = typeof(Dictionary<string, int>);
                v = typeof(Dictionary<string, uint>);
                v = typeof(Dictionary<string, long>);
                v = typeof(Dictionary<string, ulong>);
                v = typeof(Dictionary<string, string>);
                v = typeof(Dictionary<string, bool>);
            }
        }

#if !USE_HOT
        static Help()
        {
            Init();
        }
#endif

        //缓存类型信息
        static Dictionary<System.Type, CacheType> m_caches = new Dictionary<System.Type, CacheType>();
        //构造,注册一些信息
        public static void  Init()
        {
            
#if USE_HOT
#if UNITY_EDITOR
            //加载Dll里面的类型
            foreach(var itor in DllInitByEditor.appdomain.LoadedTypes)
#else
              foreach (var itor in HotMgr.appdomain.LoadedTypes)
#endif
            {
                allTypesByFullName.Add(itor.Key, itor.Value.ReflectionType);
            }
            //基础类型
            Reg<int>();
            Reg<uint>();
            Reg<sbyte>();
            Reg<byte>();
            Reg<short>();
            Reg<ushort>();
            Reg<long>();
            Reg<ulong>();
            Reg<string>();
            Reg<float>();
            Reg<double>();
#endif
            //加入程序集的类型
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("Assembly-CSharp"))
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.FullName.StartsWith("<"))
                            continue;
                        System.Type t = null;
                        if (allTypesByFullName.TryGetValue(type.FullName, out t))
                        {
#if USE_HOT
                            if(t != type)
                            {
                                if((t is ILRuntimeWrapperType) &&((ILRuntimeWrapperType)t).RealType == type)
                                    continue;
                                
                                UnityEngine.Debug.LogError(string.Format("error:{0}", type.FullName));
                            }
#endif
                            continue;
                        }
                        allTypesByFullName.Add(type.FullName, type);
                    }
                }
            }
            baseTypes.Add(typeof(int));
            baseTypes.Add(typeof(uint));
            baseTypes.Add(typeof(sbyte));
            baseTypes.Add(typeof(byte));
            baseTypes.Add(typeof(short));
            baseTypes.Add(typeof(ushort));
            baseTypes.Add(typeof(long));
            baseTypes.Add(typeof(ulong));
            baseTypes.Add(typeof(string));
            baseTypes.Add(typeof(float));
            baseTypes.Add(typeof(double));

            allTypesByFullName.Add("int", typeof(int));
            allTypesByFullName.Add("uint", typeof(uint));
            allTypesByFullName.Add("sbyte", typeof(sbyte));
            allTypesByFullName.Add("byte", typeof(byte));
            allTypesByFullName.Add("short", typeof(short));
            allTypesByFullName.Add("ushort", typeof(ushort));
            allTypesByFullName.Add("long", typeof(long));
            allTypesByFullName.Add("ulong", typeof(ulong));
            allTypesByFullName.Add("string", typeof(string));
            allTypesByFullName.Add("float", typeof(float));
            allTypesByFullName.Add("double", typeof(double));
        }

        public static object Create(System.Type type)
        {
            if (type.IsArray)
            {
                return System.Array.CreateInstance(type.GetElementType(), 0);
            }
            else if (type.Name == "String")
            {
                return string.Empty;
            }
            else
            {
                var constructor = GetOrCreate(type).GetCtor();
                if (null == constructor)
                {
                    UnityEngine.Debug.LogErrorFormat("type:{0} not GetConstructor", type.Name);
                    return null;
                }
                return constructor.Invoke(null);
            }
        }

        #region 公用接口：类型
        public static bool IsBaseType(System.Type type)
        {
            return baseTypes.Contains(type);
        }

        public static bool IsType(System.Type src, System.Type baseType)
        {
            var bt = src.BaseType;
            while (bt != null)
            {
                if (bt == baseType)
                    return true;
                bt = bt.BaseType;
            }
            return false;
        }

        public static System.Type GetType(string name)
        {
            return GetTypeByFullName(name);
        }

        public static bool IsListType(System.Type type)
        {
            if (type.IsGenericType && type.FullName.StartsWith("System.Collections.Generic.List`1[["))
                return true;
            return false;
        }

        public static System.Type GetElementByList(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;
            System.Type element;
#if USE_HOT
             if (type is ILRuntimeWrapperType)
            {
                element = ((ILRuntimeWrapperType)type).RealType.GetGenericArguments()[0];
                string FullName = element.FullName;
                if (element == typeof(ILTypeInstance))
                {
                    ILRuntimeFieldInfo ilField = fieldInfo as ILRuntimeFieldInfo;
                    var vv = ilField.ILFieldType.GenericArguments;
                    return vv[0].Value.ReflectionType;
                }
                else
                {
                    return element;
                }
            }
            else
#endif
            {
                element = type.GetGenericArguments()[0];
            }

            return element;
        }

        public static  System.Type GetElementByList(System.Type type)
        {
            System.Type element;
#if USE_HOT
            if (type is ILRuntimeWrapperType)
            {
                element = ((ILRuntimeWrapperType)type).RealType.GetGenericArguments()[0];
                string FullName = element.FullName;
                if (element == typeof(ILTypeInstance))
                {
                    UnityEngine.Debug.LogError(element.FullName);
                }
            }
#endif
            {
                element = type.GetGenericArguments()[0];
            }
            return element;
        }

        public static System.Type GetTypeByFullName(string name)
        {
#if USE_HOT && UNITY_EDITOR
            if (allTypesByFullName.Count == 0)
            {
                var app = DllInitByEditor.appdomain;
            }
#endif
            System.Type t = null;
            if (allTypesByFullName.TryGetValue(name, out t))
                return t;
            if (name.StartsWith("game.hot"))
                return GetTypeByFullName(name.Substring(4));//hot 开始算
            return null;
        }
#endregion

        public static readonly object[] EmptyObj = new object[0];
        public static readonly object[] OneObj = new object[1];
        public static readonly System.Type[] EmptyType = new System.Type[0];
        public static readonly System.Type[] OneType = new System.Type[1];
        public static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

#region 公共接口：类相关
        public static void ReleaseAll()
        {
            allTypesByFullName.Clear();
            baseTypes.Clear();
            m_caches.Clear();
        }

        //创建实例
        public static object CreateInstaince(System.Type instanceType, object param)
        {
            object instance = null;
            var cache = GetOrCreate(instanceType);
            ConstructorInfo ctor;

            if(null != param)
            {
                var type = param.GetType();
                //创建对象实例
                ctor = cache.GetCtor(type);
                if(null != ctor)
                {
                    try
                    {
                        OneObj[0] = param;
                        instance = ctor.Invoke(OneObj);
                    }
                    catch(System.Exception ex)
                    {
                        UnityEngine.Debug.LogErrorFormat("type:{0}", instanceType.FullName);
                        UnityEngine.Debug.LogException(ex);
                        throw ex;
                    }
                    finally
                    {
                        OneObj[0] = null;
                    }
                    return instance;
                }
            }

            ctor = cache.GetCtor();
            if(null != ctor)
            {
                try
                {
                    instance = ctor.Invoke(EmptyObj);
                }
                catch(System.Exception ex)
                {
                    UnityEngine.Debug.LogErrorFormat("type:{0}", instanceType.FullName);
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }
            }

            return instance;
        }

        static CacheType GetOrCreate(System.Type type)
        {
            CacheType st = null;
            if (!m_caches.TryGetValue(type, out st))
            {
                st = new CacheType(type);
                m_caches.Add(type, st);
            }
            return st;
        }

        //获得属性
        public static PropertyInfo GetProperty(System.Type type, string name)
        {
            if (null == type)
                return null;
            return GetOrCreate(type).GetProperty(name);
        }
        //获得方法
        public static MethodInfo GetMethod(System.Type type, string name)
        {
            if (null == type)
                return null;
            return GetOrCreate(type).GetMethod(name);
        }
        //获得字段
        public static FieldInfo GetField(System.Type type, string name)
        {
            if (null == type)
                return null;
            return GetOrCreate(type).GetField(name);
        }
        //获得序列化字段
        public static List<FieldInfo> GetSerializeField(System.Type type)
        {
            return GetOrCreate(type).GetSerializeField();
        }

        public static List<FieldInfo> GetSerializeField(object obj)
        {
#if USE_HOT
            if(obj is ILTypeInstance)
            {
                var item = (ILTypeInstance)obj;
                return GetOrCreate(item.Type.ReflectionType).GetSerializeField();
            }
#endif
            return GetOrCreate(obj.GetType()).GetSerializeField();
        }

        public static void GetSerializeField(System.Type type, List<FieldInfo> fieldInfos)
        {
            if (type.IsGenericType && type.FullName.StartsWith("System.Collections.Generic.List`1[["))
            {
                return;
            }
#if USE_HOT
            bool isILType = false;
            HashSet<string> noPubs = new HashSet<string>();
            HashSet<string> allfields = new HashSet<string>();
            if (type is ILRuntimeType)
            {
                isILType = true;
                type = ((ILRuntimeType)type).ILType.ReflectionType;
                TypeDefinition typeDef = ((ILRuntimeType)type).ILType.TypeDefinition;
                var fields = typeDef.Fields;
                for (int i = 0; i < fields.Count; ++i)
                {
                    var field = fields[i];
                    allfields.Add(field.Name);
                    if (!field.IsPublic)
                        noPubs.Add(field.Name);
                }
            }
#endif
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0; i < fields.Length; ++i)
                {
                    var field = fields[i];
                    var fieldType = field.FieldType;
#if USE_HOT
                    if (isILType && !allfields.Contains(field.Name))
                        continue;
#endif
                    bool isPublic = field.IsPublic;
                    if (fieldType.BaseType == typeof(System.MulticastDelegate) || fieldType.BaseType == typeof(System.Delegate))
                        continue;
#if USE_HOT
                    if (isPublic)
                    {
                        if (isILType && noPubs.Contains(field.Name))
                            isPublic = false;
                    }
#endif
                    if (isPublic || (field.GetCustomAttributes(typeof(UnityEngine.SerializeField), false).Length != 0))
                    {
                        //数组处理
                        if (fieldType.IsArray)
                        {
                            var element = fieldType.GetElementType();
                            if (element.IsArray || IsListType(element)) continue;
                            if(IsBaseType(element) || IsType(element,typeof(UnityEngine.Object)))
                            {
                                fieldInfos.Add(field);
                            }
                            else 
                            {
                                if (element.IsSerializable)
                                    fieldInfos.Add(field);
                            }
                        }
                        else if (baseTypes.Contains(fieldType))
                        {
                            // 基础类型
                            fieldInfos.Add(field);
                        }
                        else if(IsListType(fieldType))
                        {
                            var element = GetElementByList(field);
                            if (element.IsArray || IsListType(element)) continue;
                            if(IsBaseType(element) || IsType(element,typeof(UnityEngine.Object)))
                            {
                                fieldInfos.Add(field);
                            }
                            else
                            {
                                if (element.IsSerializable)
                                    fieldInfos.Add(field);
                            }
                        }
                        else
                        {
                            if (
#if USE_HOT
                                (field.FieldType is ILRuntimeType && ((ILRuntimeType)field.FieldType).ILType.TypeDefinition.IsSerializable) ||
#endif
                                fieldType.IsSerializable || IsType(fieldType, typeof(UnityEngine.Object)))
                                fieldInfos.Add(field);
                        }
                    }
                }
            }
        }
#endregion

        //类型
        static HashSet<System.Type> baseTypes = new HashSet<System.Type>();
        //类型名称：类型 字典
        static Dictionary<string, System.Type> allTypesByFullName = new Dictionary<string, System.Type>();
        //相关类型注册到字典里
        static void Reg<T>()
        {
            var type = typeof(T);
            allTypesByFullName[type.FullName] = type;
        }

#region 获取属性相关
#if UNITY_EDITOR
        //获取属性
        public static List<System.Type> GetCustomAttributesType(System.Type type)
        {
            List<System.Type> types = new List<System.Type>();
            foreach (var itor in allTypesByFullName)
            {
                if (itor.Value.IsAbstract)//虚函数
                    continue;
                if (itor.Value.IsInterface)//接口
                    continue;

                if (HasCustomAttributes(itor.Value, type))
                {
                    types.Add(itor.Value);
                }
            }
            return types;
        }
        //是否拥有属性
        static bool HasCustomAttributes(System.Type type, System.Type customAtt)
        {
            if (null == type)
                return false;
            bool has = type.GetCustomAttributes(customAtt, true).Length == 0 ? false : true;

#if USE_HOT
            if (has)
                return true;
            ILRuntimeType rt = type as ILRuntimeType;
            if (rt == null)
                return false;
            try
            {
                return HasCustomAttributes(type.BaseType, customAtt);
            }
            catch (System.Exception ex)
            {
                return false;
            }
#else
            return has;
#endif
        }
        //得到继承Type的类型
        public static List<System.Type> GetBaseType(string baseTypeFullName)
        {
            List<System.Type> types = new List<System.Type>();
            foreach (var itor in allTypesByFullName)
            {
                if (itor.Value.IsAbstract)
                    continue;
                if (itor.Value.IsInterface)
                    continue;
                if (itor.Value.BaseType != null && itor.Value.BaseType.FullName == baseTypeFullName)
                    types.Add(itor.Value);
            }
            return types;
        }
#endif
    }
#endregion
}

