using CoreStudy.Core.Domain;
using CoreStudy.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreStudy.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly MyContext _myContext;

        public PostService(MyContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            var posts = _myContext.Posts
                    .Where(p => p.Id == id);


            return await posts.SingleOrDefaultAsync();
        }

        public async Task<List<Post>> GetListAsync(int pageIndex = 1, int pageSize = 10)
        {
            var posts = _myContext.Posts.AsQueryable();

            posts = posts.OrderByDescending(p => p.CreateTime);
            
            var skip = (pageIndex - 1) * pageSize;
            posts = posts.Skip(skip).Take(pageSize);

            return await posts.ToListAsync();
        }

        public async Task<Post> PostAsync(Post post)
        {
            //test
            post.CreateTime = DateTime.Now;

            _myContext.Posts.Add(post);

            await _myContext.SaveChangesAsync();

            return post;
        }
    }
}
