using System;

namespace CaPPMS.Attributes
{
    /// <summary>
    /// Attribute to associate a class with the backing table name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlTableNameAttribute : Attribute
    {
        /// <summary>
        /// Initialize the attribute with the table name.
        /// </summary>
        /// <param name="tableName"></param>
        public SqlTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// Associated table name.
        /// </summary>
        public string TableName { get; }
    }
}
