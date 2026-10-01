using Ordering.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Core.IRepositories
{
    public interface IOrderRepository :IGenericRepository<Order>,IDisposable
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
