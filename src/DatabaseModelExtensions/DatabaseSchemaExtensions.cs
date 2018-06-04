using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DatabaseModelExtensions
{
    /// <summary>
    /// Determines if the Property should not be 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class SchemaIgnoreAttribute : Attribute
    {
    }

    public static class DatabaseSchemaExtensions
    {
        public static string GetDatabaseSchemas(this object input)
        {
            var props = input.GetType().GetProperties().Where(p => !CustomAttributeExtensions.IsDefined((MemberInfo)p, typeof(SchemaIgnoreAttribute)));
            return BuildSchemas(props);
        }

        public static string GetDatabaseValueSchemas(this object input)
        {
            var props = input.GetType().GetProperties().Where(p => !p.IsDefined(typeof(SchemaIgnoreAttribute)));
            return BuildValueSchemas(props);
        }

        public static string GetDatabaseSchemas(this Type type)
        {
            var props = type.GetProperties().Where(p => !p.IsDefined(typeof(SchemaIgnoreAttribute))).ToList();
            return BuildSchemas(props);
        }

        public static string GetDatabaseValueSchemas(this Type type)
        {
            var props = type.GetProperties().Where(p => !p.IsDefined(typeof(SchemaIgnoreAttribute))).ToList();
            return BuildValueSchemas(props);
        }

        private static string FilterSchemaName(string originalName)
        {
            switch (originalName)
            {
                case "Read":
                    return "[Read]";
                case "User":
                    return "[User]";
            }
            return originalName;
        }

        private static string BuildSchemas(IEnumerable<PropertyInfo> props)
        {
            return props.Aggregate(string.Empty, (current, p) => current + $"{FilterSchemaName(p.Name)},").TrimEnd(',');
        }

        private static string BuildValueSchemas(IEnumerable<PropertyInfo> props)
        {
            return props.Aggregate(string.Empty, (current, p) => current + $"@{p.Name},").TrimEnd(',');
        }
    }
}
