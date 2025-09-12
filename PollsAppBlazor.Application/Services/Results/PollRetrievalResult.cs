namespace PollsAppBlazor.Application.Services.Results;

public enum PollRetrievalError
{
    None,
    PollNotFound,
    PollDeleted
}

public record PollRetrievalResult<TPoll>(PollRetrievalError Error, TPoll? Poll = default)
{
    public bool IsSuccess => Error == PollRetrievalError.None;

    public static PollRetrievalResult<TPoll> Success(TPoll poll) => new(PollRetrievalError.None, poll);
    public static PollRetrievalResult<TPoll> Failure(PollRetrievalError error) => new(error);
}
