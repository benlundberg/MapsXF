using SQLite;

namespace MapsXF.Core
{
    public class GeometryItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Color { get; set; }
        public string GeometryType { get; set; }
        public string GeometryJson { get; set; }
    }
}
