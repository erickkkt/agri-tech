using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Farm.Api.Controllers
{
    /// <summary>Community forum: threads + posts.</summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/forum")]
    [Produces("application/json")]
    [ApiController]
    public class ForumController : FarmBaseController
    {
        private readonly FarmDbContext _db;

        public ForumController(FarmDbContext db, IUserService userService) : base(userService)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet("threads")]
        public async Task<ActionResult<IEnumerable<ForumThread>>> Threads([FromQuery] string category, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            var qry = _db.ForumThreads.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(category)) qry = qry.Where(t => t.Category == category);
            var list = await qry.OrderByDescending(t => t.IsPinned)
                                .ThenByDescending(t => t.LastReplyAt ?? t.CreatedAt)
                                .Skip(pageIndex * pageSize).Take(pageSize)
                                .ToListAsync();
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("threads/{id:guid}")]
        public async Task<ActionResult<ForumThread>> Thread(Guid id)
        {
            var thread = await _db.ForumThreads.FirstOrDefaultAsync(t => t.Id == id);
            if (thread == null) return NotFound();
            thread.ViewCount++;
            await _db.SaveChangesAsync();
            return Ok(thread);
        }

        [AllowAnonymous]
        [HttpGet("threads/{id:guid}/posts")]
        public async Task<ActionResult<IEnumerable<ForumPost>>> Posts(Guid id)
        {
            return Ok(await _db.ForumPosts.AsNoTracking()
                .Where(p => p.ThreadId == id)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync());
        }

        [HttpPost("threads")]
        public async Task<ActionResult<Guid>> CreateThread([FromBody] ForumThread thread)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            thread.Id = Guid.Empty;
            thread.AuthorUserId = user.Id;
            thread.AuthorUserName = user.UserName;
            thread.CreatedAt = DateTime.UtcNow;
            thread.LastReplyAt = thread.CreatedAt;
            _db.ForumThreads.Add(thread);
            await _db.SaveChangesAsync();
            return Ok(thread.Id);
        }

        [HttpPost("posts")]
        public async Task<ActionResult<Guid>> CreatePost([FromBody] ForumPost post)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            var thread = await _db.ForumThreads.FirstOrDefaultAsync(t => t.Id == post.ThreadId);
            if (thread == null) return NotFound("Thread not found");
            if (thread.IsLocked) return BadRequest(new { error = "Thread is locked" });

            post.Id = Guid.Empty;
            post.AuthorUserId = user.Id;
            post.AuthorUserName = user.UserName;
            post.CreatedAt = DateTime.UtcNow;
            _db.ForumPosts.Add(post);

            thread.LastReplyAt = post.CreatedAt;
            thread.PostCount++;

            await _db.SaveChangesAsync();
            return Ok(post.Id);
        }
    }
}
