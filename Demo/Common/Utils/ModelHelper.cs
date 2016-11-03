using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Utils
{
    public static class ModelHelper
    {
        #region Model转换
        /// <summary>
        /// Model转换
        /// </summary>
        public static T Convert<T>(this object obj) where T : new()
        {
            if (obj == null) return default(T);
            Type baseType = obj.GetType();
            if (typeof(T).FullName == obj.GetType().BaseType.FullName)
            {
                baseType = obj.GetType().BaseType;
            }
            T t = new T();
            PropertyInfo[] propertyInfoArr = GetEntityProperties(baseType);
            foreach (PropertyInfo propertyInfo in propertyInfoArr)
            {
                propertyInfo.SetValue(t, propertyInfo.GetValue(obj, null), null);
            }
            return t;
        }
        #endregion

        #region 获取实体类属性
        /// <summary>
        /// 获取实体类属性
        /// </summary>
        private static PropertyInfo[] GetEntityProperties(Type type)
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            PropertyInfo[] propertyInfoList = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                if (propertyInfo.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false).Length == 0
                    && propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), false).Length == 0)
                {
                    result.Add(propertyInfo);
                }
            }
            return result.ToArray();
        }
        #endregion

    }
}
