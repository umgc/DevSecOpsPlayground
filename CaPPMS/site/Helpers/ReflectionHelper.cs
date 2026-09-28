using CaPPMS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

public static class ReflectionHelper
{
    public static IEnumerable<PropertyInfo> GetNonIgnoredProperties(Type type)
    {
        return type.GetProperties()
            .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreDataMemberAttribute)));
    }

    public static string GetTableName(Type type)
    {
        var tableNameAttribute = type.GetCustomAttributes(typeof(SqlTableNameAttribute), true)
            .FirstOrDefault() as SqlTableNameAttribute;
        return tableNameAttribute?.TableName ?? string.Empty;
    }
}
