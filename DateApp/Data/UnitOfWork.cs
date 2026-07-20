using DateApp.Interfaces;

namespace DateApp.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IMemberRepository _memberRepository;
        private IMessageRepository _messageRepository;
        private ILikesRepository _likeRepository;
        public IMemberRepository MemberRepository => _memberRepository??=new MemberRepository(context);

        public IMessageRepository MessageRepository => _messageRepository??=new MessageRepository(context);

        public ILikesRepository LikeRepository => _likeRepository??=new LikesRepository(context);
        public async Task<bool> Complete()
        {
            try
            {
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        public bool HasChanges()
        {
           return context.ChangeTracker.HasChanges();
        }
    }
}
