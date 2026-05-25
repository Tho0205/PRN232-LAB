using System.Dynamic;
using System.Reflection;

namespace PRN232.LAB1.Services.Helpers
{
    public static class DataShaper
    {
        public static ExpandoObject ShapeData<T>(T entity, string fieldsString)
        {
            var expandoObject = new ExpandoObject();
            if (entity == null) return expandoObject;

            var propertyInfoList = GetPropertyInfos<T>(fieldsString);
            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(entity);
                ((IDictionary<string, object?>)expandoObject).Add(propertyInfo.Name, propertyValue);
            }
            return expandoObject;
        }

        public static IEnumerable<ExpandoObject> ShapeData<T>(IEnumerable<T> entities, string fieldsString)
        {
            var propertyInfoList = GetPropertyInfos<T>(fieldsString);
            var expandoObjectList = new List<ExpandoObject>();

            foreach (var entity in entities)
            {
                var expandoObject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(entity);
                    ((IDictionary<string, object?>)expandoObject).Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(expandoObject);
            }
            return expandoObjectList;
        }

        private static IEnumerable<PropertyInfo> GetPropertyInfos<T>(string fieldsString)
        {
            var propertyInfos = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fieldsString))
            {
                return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }

            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    propertyInfos.Add(propertyInfo);
                }
            }
            return propertyInfos;
        }
    }
}
