using Dapper;
using Discount.Core;
using Discount.Core.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value))
            {
                var query = QueryLoader.GetSqlQuery("CreateCoupon");
                var affected = await connection.ExecuteAsync(query,new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value))
            {
                var query = QueryLoader.GetSqlQuery("DeleteCoupon");
                var affected = await connection.ExecuteAsync(query, new { ProductName = productName });
                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value))
            {
                var query = QueryLoader.GetSqlQuery("GetCoupon");
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(query, new { ProductName = productName });
                if (coupon == null)
                    return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Available" };
                return coupon;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value))
            {
                var query = QueryLoader.GetSqlQuery("UpdateCoupon");
                var affected = await connection.ExecuteAsync(query,new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

                if (affected == 0)
                    return false;

                return true;
            }
        }
    }
}
