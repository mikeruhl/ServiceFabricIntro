using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.API.Model;
using Ecommerce.CheckoutService.Model;
using Ecommerce.ProductCatalog.Model;

namespace Ecommerce.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(f=>f.IsAvailable, t=>t.MapFrom(s=>s.Availability > 0));

            CreateMap<CheckoutSummary, CheckoutSummaryDto>();

            CreateMap<CheckoutProduct, CheckoutProductDto>();
        }
    }
}
