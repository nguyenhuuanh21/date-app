using DateApp.Entities;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class LikesRepository(AppDbContext dbContext) : ILikesRepository
    {
        public void AddLike(MemberLike like)
        {
            dbContext.MemberLikes.Add(like);
        }

        public void DeleteLike(MemberLike like)
        {
            dbContext.MemberLikes.Remove(like);
        }

        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            return await dbContext.MemberLikes
                .Where(like => like.SourceMemberId == memberId)
                .Select(like => like.TargetMemberId)
                .ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await dbContext.MemberLikes
                .FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<PaginateResult<Member>> GetMemberLikes(LikeParams likeParams)
        {
            var query = dbContext.MemberLikes.AsQueryable();
            IQueryable<Member> result;
            switch (likeParams.Predicate)
            {
                case "liked":
                    result = query.Where(like => like.SourceMemberId == likeParams.MemberId)
                        .Select(x => x.TargetMember);
                    break;
                case "likedBy":
                    result = query.Where(like => like.TargetMemberId == likeParams.MemberId)
                        .Select(x => x.SourceMember);
                    break;
                default:
                    var likeIds=await GetCurrentMemberLikeIds(likeParams.MemberId);
                    result= query.Where(like => like.TargetMemberId == likeParams.MemberId && likeIds.Contains(like.SourceMemberId))
                        .Select(x => x.SourceMember);
                    break;
            }
            return await PaginateHelper.CreateAsync(result, likeParams.PageNumber, likeParams.PageSize);

        }

        public Task<bool> SaveAllAsync()
        {
            return dbContext.SaveChangesAsync().ContinueWith(task => task.Result > 0);
        }
    }
}
