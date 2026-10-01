using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Catalog.Application.Mapper
{
    public static class CustomMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsAssembly == true;
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);

            return config.CreateMapper();
        });

        public static IMapper Mapper => Lazy.Value;
    }
}