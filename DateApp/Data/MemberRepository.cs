using DateApp.Entities;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class MemberRepository(AppDbContext context) : IMemberRepository
    {
        public async Task<Member?> GetMemberByIdAsync(string id)
        {
            return await context.Members.FindAsync(id);
        }

        public async Task<PaginateResult<Member>> GetMembersAsync(MemberParams memberParams)
        {
            var query=context.Members.AsQueryable();
            query=query.Where(x=>x.Id!=memberParams.CurrentMemberId);
            if (memberParams.Gender != null)
            {
                query = query.Where(x => x.Gender == memberParams.Gender);
            }
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));  
            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            query = memberParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };
            return await PaginateHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
        {
            return await context.Members.Where(p=>p.Id == memberId)
                .SelectMany(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;
        }
        public async Task<Member?> GetMemberForUpdate(string id)
        {
            return await context.Members.Include(x=>x.AppUser).Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.Id == id);
        }
    }
}
