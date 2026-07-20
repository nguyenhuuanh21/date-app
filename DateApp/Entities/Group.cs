using System.ComponentModel.DataAnnotations;

namespace DateApp.Entities
{
    public class Group(string name)
    {
        [Key]
        public required string Name { get; set; }= name;
        //nav
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
