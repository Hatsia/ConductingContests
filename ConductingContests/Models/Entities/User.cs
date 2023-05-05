using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ConductingContests.Models.Entities
{
    public class User : IdentityUser
    {
        public List<Contest> Contests { get; set; }

        public List<ParticipationRequest> Participationrequest { get; set; }

        public List<OfferedService> OfferedService { get; set; }
    }
}
