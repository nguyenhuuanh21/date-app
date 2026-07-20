namespace DateApp.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository MemberRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikeRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
