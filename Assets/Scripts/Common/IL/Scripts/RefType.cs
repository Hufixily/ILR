using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.IL
{
    public class RefType
    {
        //带参数构造
        public RefType(string fullType, object param)
        {
            this.fullType = fullType;
            type = Help.GetTypeByFullName(fullType);
            if (null != type)
                instance = Help.CreateInstaince(type, param);
        }
        //不带参数构造
        public RefType(string fullType)
        {
            this.fullType = fullType;
            type = Help.GetTypeByFullName(fullType);
            if (null != type)
                instance = Help.CreateInstaince(type, null);
        }
        //重绑定使用构造
        public RefType(object instance)
        {
            this.instance = instance;
#if USE_HOT
            //判断是否热更类型
            if (instance is ILRuntime.Runtime.Intepreter.ILTypeInstance)
            {
                var realType = (ILRuntime.Runtime.Intepreter.ILTypeInstance)instance;
                type = realType.Type.ReflectionType;
            }
            else
#endif
            {
                type = instance.GetType();
            }
            this.fullType = type.FullName;
        }

        string fullType;//全称
        object instance;//控制权对象
        System.Type type;//类型
        public object Instance { get { return instance; } }
        public System.Type Type { get { return type; } }

        #region 字段相关
        //得到某个字段的值
        public T GetFieldT<T>(string name) { return (T)GetField(name); }

        //设置字段的值
        public void SetField(string name, object value)
        {
            var field = Help.GetField(type, name);
            if (null == field)
            {
                Debug.LogError(string.Format("type :{0} field: {1} not find!", fullType, name));
                return;
            }

            try
            {
                field.SetValue(instance, value);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SetField type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
            }
        }
        public object GetField(string name)
        {
            var field = Help.GetField(type, name);
            if (null == field)
            {
                Debug.LogError(string.Format("type :{0} field: {1} not find!", fullType, name));
                return null;
            }

            try
            {
                return field.GetValue(instance);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("GetField type :{0} name:{1}", type.Name, name));
                return null;
            }
        }
        public T TryGetFieldT<T>(string name)
        {
            return (T)TryGetField(name);
        }
        public object TryGetField(string name)
        {
            var field = Help.GetField(type, name);
            if (null == field)
                return null;

            try
            {
                return field.GetValue(instance);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("TryGetField type :{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
                return null;
            }
        }
        public void TrySetField(string name, object value)
        {
            var field = Help.GetField(type, name);
            if (null == field)
                return;

            try
            {
                field.SetValue(instance, value);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("TrySetField type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
            }
        }
        #endregion
        #region 方法相关
        public void InvokeMethod(string name, params object[] param)
        {
            InvokeMethodReturn(name, param);
        }

        public object InvokeMethodReturn(string name, params object[] param)
        {
            var methodInfo = IL.Help.GetMethod(type, name);
            if (methodInfo == null)
            {
                Debug.LogError(string.Format("type:{0} field:{1} not find!", fullType, name));
                return null;
            }

            try
            {
                return methodInfo.Invoke(instance, param);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("InvokeMethodReturn type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
                return null;
            }
        }
        public void TryInvokeMethod(string name, params object[] param)
        {
            TryInvokeMethodReturn(name, param);
        }

        public object TryInvokeMethodReturn(string name, params object[] param)
        {
            var methodInfo = IL.Help.GetMethod(type, name);
            if (methodInfo == null)
                return null;

            try
            {
                return methodInfo.Invoke(instance, param);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("TryInvokeMethodReturn type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
                return null;
            }
        }

        public System.Reflection.MethodInfo TryGetMethod(string name)
        {
            var methodInfo = IL.Help.GetMethod(type, name);
            if (null == methodInfo)
                Debug.LogError(string.Format("TryGetMethod Error type:{0} name:{1}", type, name));
            return methodInfo;
        }
        #endregion

        #region 属性相关
        //获得属性
        public T GetPropertyT<T>(string name)
        {
            return (T)GetProperty(name);
        }
        public object GetProperty(string name)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
            {
                Debug.LogError(string.Format("type:{0} Property:{1} not find!", fullType, name));
                return null;
            }

            try
            {
                return propertyInfo.GetValue(instance, null);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("GetProperty type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
                return null;
            }
        }
        public object TryGetProperty(string name)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
                return null;
            try
            {
                return propertyInfo.GetValue(instance, null);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("TryGetProperty type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
                return null;
            }
        }
        public T TryGetPropertyT<T>(string name)
        {
            return (T)TryGetProperty(name);
        }
        //设置属性
        public void SetProperty(string name, object value)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
            {
                Debug.LogError(string.Format("type:{0} Property:{1} not find!", fullType, name));
                return;
            }
            try
            {
                propertyInfo.SetValue(instance, value, null);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SetProperty type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
            }
        }
        public void TrySetProperty(string name, object value)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
                return;

            try
            {
                propertyInfo.SetValue(instance, value, null);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("TrySetProperty type:{0} name:{1}", type.Name, name));
                Debug.LogException(ex);
            }
        }
    }
    #endregion
}