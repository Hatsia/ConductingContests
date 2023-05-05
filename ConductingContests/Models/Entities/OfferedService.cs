using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConductingContests.Models.Entities
{
    public class OfferedService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string FilePath { get; set; }


        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        public string UserId { get; set; }

        public User User { get; set; }


        public int ContestId { get; set; }

        public Contest Contest { get; set; }
    }
}
