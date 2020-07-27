using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CoreStudy.Core.Domain;
using CoreStudy.Services.Categories;
using CoreStudy.Services.Posts;
using CoreStudy.Web.Models.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreStudy.Web.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;

        public PostsController(
            IMapper mapper,
            IPostService postService,
            ICategoryService categoryService,
            IConfiguration configuration
            )
        {
            _mapper = mapper;
            _postService = postService;
            _categoryService = categoryService;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取博文列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>博文列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<PostModel>), 200)]
        public async Task<IActionResult> GetListAsync(int page = 1, int pageSize = 10)
        {
            if (!Authorization())
            {
                return Unauthorized();
            }

            var posts = await _postService.GetListAsync(page, pageSize);

            return Ok(_mapper.Map<List<PostModel>>(posts));
        }

        /// <summary>
        /// 获取一条博文内容（通过Id）
        /// </summary>
        /// <param name="postId">博文 id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{postId}", Name = "GetPostById")]
        public async Task<IActionResult> GetPostById([FromRoute] int postId)
        {
            if (!Authorization())
            {
                return Unauthorized();
            }

            var post = await _postService.GetByIdAsync(postId);

            if (post == null)
            {
                return NotFound($"没有id为{postId}的Blog");
            }

            return Ok(_mapper.Map<PostModel>(post));
        }

        /// <summary>
        /// 创建一条博文（不论是保存草稿还是正式发布，首次调用的都是本接口）
        /// </summary>
        /// <param name="postModel">创建博文的请求</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostModel postModel)
        {
            if (!Authorization())
            {
                return Unauthorized();
            }

            var post = _mapper.Map<Post>(postModel);

            await _postService.PostAsync(post);

            return await GetPostById(post.Id);
            //想用下面这个方法的，但是报错No route matches the supplied values，以后看
            //return CreatedAtRoute(nameof(GetPostById), new { postId = post.Id });
        }


        #region 加密验证PrivateKey

        private string GetPrivateKey()
        {
            return _configuration.GetValue<String>("PrivateKey");
        }

        private string CreateMD5(string str)
        {

            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(str);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }

        }

        private bool Authorization()
        {
            string authorization = Request.Headers["Authorization"];

            //var settings = new Settings();
            //_configuration.GetSection("Settings").Bind(settings);
            string privateKey = _configuration.GetValue<String>("PrivateKey");
            

            if (CreateMD5(privateKey)?.ToUpper() == authorization?.ToUpper())
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
