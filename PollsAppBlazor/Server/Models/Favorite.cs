﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PollsAppBlazor.Server.Models
{
	public class Favorite
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey(nameof(Poll))]
		public int PollId { get; set; }
		/// <summary>
		/// Poll that is marked as favorite.
		/// </summary>
		public Poll? Poll { get; set; }

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;
		/// <summary>
		/// User who marked poll as favorite.
		/// </summary>
		public ApplicationUser? User { get; set; }
	}
}
