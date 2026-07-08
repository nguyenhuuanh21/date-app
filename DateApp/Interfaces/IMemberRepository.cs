using DateApp.Entities;
using DateApp.Helpers;

namespace DateApp.Interfaces
{
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<bool> SaveAllAsync();
        Task<PaginateResult<Member>> GetMembersAsync(MemberParams memberParams);
        Task<Member?> GetMemberByIdAsync(string id);
        Task<Member?> GetMemberForUpdate(string id);
        Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    }
}
