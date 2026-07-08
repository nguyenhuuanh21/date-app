using DateApp.Data;
using DateApp.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext =await next();
            if (context.HttpContext.User.Identity?.IsAuthenticated == false) return;
            var memberId = resultContext.HttpContext.User.GetUserId();
            var dbContext = resultContext.HttpContext.RequestServices.GetService<AppDbContext>();
            await dbContext!.Members.Where(m => m.Id == memberId)
                .ExecuteUpdateAsync(m => m.SetProperty(u => u.LastActive, DateTime.UtcNow));
        }
    }
}
