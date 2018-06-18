using System;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    [AttributeUsage(System.AttributeTargets.Property)]
    public class ListColumn : Attribute
    {
        public string ColumnName { get; set; }
        public bool ReadOnly { get; set; }

        public ListColumn(string columnName, bool readOnly = false)
        {
            ColumnName = columnName;
            ReadOnly = readOnly;
        }
    }
}
