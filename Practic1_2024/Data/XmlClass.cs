using System.Xml.Serialization;

namespace Practic1_2024.Data
{
    public class XmlClass
    {
        [XmlRoot("Tables")]
        public class TablesData
        {
            [XmlElement("Table")]
            public List<TableData> Tables { get; set; } = new List<TableData>();
        }

        public class TableData
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlArray("Rows")]
            [XmlArrayItem("Row")]
            public List<RowData> Rows { get; set; } = new List<RowData>();
        }

        public class RowData
        {
            [XmlElement("Column")]
            public List<ColumnData> Columns { get; set; } = new List<ColumnData>();
        }

        public class ColumnData
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }


    }
}
