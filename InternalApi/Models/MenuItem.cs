namespace TESTAPI.Models
{
    public class MenuItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string toUrl { get; set; }
        public int? parentId { get; set; }
        public int? sortOrder { get; set; }
        public List<MenuItem> children { get; set; } = new List<MenuItem>();
    }

}
