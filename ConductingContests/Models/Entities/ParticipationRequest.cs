using System;


namespace ConductingContests.Models.Entities
{
    public class ParticipationRequest
    {
		public int Id { get; set; }

		public DateTime SubmissionDate { get; set; }

		public StatusRequest? Status { get; set; }


		public string UserId { get; set; }

		public User User { get; set; }


		public int ContestId { get; set; }

		public Contest Contest { get; set; }
	}
}
