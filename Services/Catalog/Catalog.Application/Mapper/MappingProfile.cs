using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core;
using Catalog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Mapper
{
    public class MappingProfile :Profile
    {
        public MappingProfile() 
        {
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<ProductBrand, BrandResponse>().ReverseMap();
            CreateMap<ProductType, TypeResponse>().ReverseMap();

            CreateMap<Product, CreateProductCommand>().ReverseMap();
            CreateMap<Pagination<Product>, Pagination<ProductResponse>>().ReverseMap();



            //CreateMap<List<ProductType>, List<TypeResponse>>().ReverseMap();
            //CreateMap<List<ProductBrand>, List<BrandResponse>>().ReverseMap();
            //CreateMap<List<Product>, List<ProductResponse>>().ReverseMap();
        }
    }
}
