using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entites;
using Ordering.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            var orderList = await _dbContext.Orders
                .Where(o => o.UserName == userName)
                .ToListAsync();
            return orderList;
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
                _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
        ~OrderRepository() =>
            Dispose();
    }
}
