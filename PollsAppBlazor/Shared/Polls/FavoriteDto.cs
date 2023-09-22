namespace PollsAppBlazor.Shared.Polls
{
	public class FavoriteDto
	{
		[Required]
		public int PollId { get; set; }
		[Required]
		public bool IsFavorite { get; set; }
	}
}
