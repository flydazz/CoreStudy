using AutoMapper;
using CoreStudy.Core.Domain;
using CoreStudy.Web.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Web.Mapping
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostModel, Post>();

            CreateMap<Post, PostModel>();
        }
    }
}
