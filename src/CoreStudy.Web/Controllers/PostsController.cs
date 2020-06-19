using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreStudy.Core.Domain;
using CoreStudy.Services.Categories;
using CoreStudy.Services.Posts;
using CoreStudy.Web.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace CoreStudy.Web.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;

        public PostsController(
            IMapper mapper,
            IPostService postService,
            ICategoryService categoryService
            )
        {
            _mapper = mapper;
            _postService = postService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// 获取博文列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>博文列表</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<PostModel>), 200)]
        public async Task<IActionResult> GetListAsync(int page = 1, int pageSize = 10)
        {
            var posts = await _postService.GetListAsync(page, pageSize);

            return Ok(_mapper.Map<List<PostModel>>(posts));
        }

        /// <summary>
        /// 获取一条博文内容（通过Id）
        /// </summary>
        /// <param name="id">博文 id</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = nameof(GetAsync))]
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound($"没有id为{id}的Blog");
            }

            return Ok(_mapper.Map<PostModel>(post));
        }

        /// <summary>
        /// 创建一条博文（不论是保存草稿还是正式发布，首次调用的都是本接口）
        /// </summary>
        /// <param name="postModel">创建博文的请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PostModel postModel)
        {
            var post = _mapper.Map<Post>(postModel);

            await _postService.PostAsync(post);

            return CreatedAtRoute(nameof(GetAsync), new { id = post.Id }, _mapper.Map<PostModel>(post));
        }

    }
}
