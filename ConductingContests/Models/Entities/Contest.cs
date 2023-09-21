using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConductingContests.Models.Entities
{
    public class Contest
    {
		public int Id { get; set; }

		public string Title { get; set; }

		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public StatusContest Status { get; set; }

		public string WinnerUserName { get; set; }



		public string UserId { get; set; }

		public User User { get; set; }


		public int CategoryId { get; set; }

		public ContestCategory Category { get; set; }


		public List<ParticipationRequest> ParticipationRequest { get; set; }

		public List<OfferedService> OfferedService { get; set; }
	}
}
