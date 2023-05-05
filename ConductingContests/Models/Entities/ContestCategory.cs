using System.Collections.Generic;

namespace ConductingContests.Models.Entities
{
    public class ContestCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public List<Contest> Contests { get; set; }
    }
}
