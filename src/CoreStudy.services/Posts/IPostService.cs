using CoreStudy.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreStudy.Services.Posts
{
    public interface IPostService
    {
        Task<List<Post>> GetListAsync(int pageIndex = 1, int pageSize = 10);

        Task<Post> PostAsync(Post post);

        Task<Post> GetByIdAsync(int id);
    }
}
