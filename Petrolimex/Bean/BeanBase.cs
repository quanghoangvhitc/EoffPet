using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Petrolimex.Bean
{
    public class BeanBase
    {
        public static BeanBase TryParse(Type type, JObject jsData)
        {
            object retValue = Activator.CreateInstance(type);

            foreach (JProperty prop in jsData.Properties())
            {
                System.Reflection.PropertyInfo perInfo = type.GetProperty(prop.Name);
                if (perInfo != null && prop.Value.Type != JTokenType.Null)
                {
                    perInfo.SetValue(retValue, Convert.ChangeType(prop.Value, perInfo.PropertyType), null);
                }
            }

            return (BeanBase)retValue;
        }
        /// <summary>
        /// Tên Column so sánh dữ liệu
        /// </summary>
        /// <returns></returns>
        public virtual string GetModifiedColName()
        {
            return "Modified";
        }
        /// <summary>
        /// Lấy địa chỉ Url API xử lý trên mục Bean tương ứng
        /// </summary>
        /// <returns></returns>
        public virtual string GetApiUrlExec()
        {
            return "";
        }

        /// <summary>
        /// Lấy đường dẫn Url tương ứng lấy dữ liệu từ Server trong phương thức get data
        /// </summary>
        /// <returns></returns>
        public virtual string GetServerUrl()
        {
            return "";
        }

        /// <summary>
        /// Lấy Các Biến Post có định tương ứng với từng đối tượng
        /// </summary>
        /// <returns></returns>
        public virtual List<KeyValuePair<string, string>> GetParaPost()
        {
            return null;
        }

        /// <summary>
        /// Lấy các khóa chính trong table
        /// </summary>
        /// <param name="type">Kiểu dữ liệu (kiển bean ánh xạ table)</param>
        /// <returns></returns>
        public static List<string> getPriKey(Type type)
        {
            return BeanBase.getLstProName(type, typeof(PrimaryKeyAttribute));
        }

        /// <summary>
        /// Lấy các khóa chính trong table
        /// </summary>
        /// <param name="type">Kiểu dữ liệu (kiển bean ánh xạ table)</param>
        /// <returns></returns>
        public static List<string> getPriKeyS(Type type)
        {

            return BeanBase.getLstProName(type, typeof(PrimaryKeySAttribute));
        }

        public static List<string> getLstProName(Type type, Type typeAttr)
        {
            List<string> retValue = new List<string>();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var p in props)
            {
                if (p.GetCustomAttributes(typeAttr, true).Length > 0)
                {
                    retValue.Add(p.Name);
                }
            }
            return retValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeySAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlEncodeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MobileKeyDiffAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlRemoveAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RemoveAccentFromAttribute : Attribute
    {
        public string ColFrom { get; set; }

        public RemoveAccentFromAttribute(string colFrom)
        {
            ColFrom = colFrom;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RemoveAccentFromMultiColumnAttribute : Attribute
    {
        public string[] ColFrom { get; set; }

        public RemoveAccentFromMultiColumnAttribute(string[] colFrom)
        {
            ColFrom = colFrom;
        }
    }
}

