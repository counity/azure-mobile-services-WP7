using System;
using System.Globalization;
using System.Reflection;
using ServiceStack.Text;

namespace coUnity.WindowsAzure.MobileServices.Util
{
    public static class TypeExtensions
    {
        public static void Patch<T>(this T orig, T updated) where T : class
        {
            foreach (var info
                in typeof(T).GetPublicProperties())
                info.SetValue(orig, info.GetValue(updated, null), null);
        }

        public static object GetIdValue(this object instance)
        {
            //todo add support for ColumnNameAttribute == id

            var idProperty = instance.GetType().GetProperty(MobileServiceTable.IdPropertyName,
                                                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (idProperty == null)
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.InvariantCulture,
                        "Expected {0} member not found.",
                        MobileServiceTable.IdPropertyName),
                    "instance");

            var retVal = idProperty.GetValue(instance, null);

            if (retVal.Equals(Activator.CreateInstance(retVal.GetType())))
                return null;

            return retVal;
        }

        public static string ToJson(this object value, bool ignoreId)
        {
            var json = JsonSerializer.SerializeToString(value);

            if (ignoreId)
            {
                var idStart = json.IndexOf("\"id\"");
                var idEnd = json.IndexOf(",", idStart, json.Length - idStart);
                if (idEnd == -1)
                    idEnd = json.Length - 2; //id is the last attribute
                json = json.Remove(idStart, idEnd);
            }

            return json;
        }

        public static string GetTableName(this Type type)
        {
            //todo add support for DataTableAttribute

            return type.Name;
        }

        public static string ToDateString(this DateTime date)
        {
            return date.ToUniversalTime().ToString(
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                CultureInfo.InvariantCulture);
        }
    }
}
