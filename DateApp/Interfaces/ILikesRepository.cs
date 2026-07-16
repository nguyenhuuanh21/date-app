using DateApp.Entities;
using DateApp.Helpers;

namespace DateApp.Interfaces
{
    public interface ILikesRepository
    {
        Task<MemberLike?>GetMemberLike(string sourceMemberId, string targetMemberId);
        Task<PaginateResult<Member>> GetMemberLikes(LikeParams likeParams);
        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
        void DeleteLike(MemberLike like);
        void AddLike(MemberLike like);
        Task<bool>SaveAllAsync();
    }
}
