using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ConductingContests.Models.Entities
{
    public class OfferedService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public IFormFile File { get; set; }


        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        public string UserId { get; set; }

        public User User { get; set; }


        public int ContestId { get; set; }

        public Contest Contest { get; set; }
    }
}
