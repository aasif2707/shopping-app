using Ordering.Core.Entites;
using Ordering.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IOrderRepository Orders { get; private set; }
        private readonly OrderContext _context;
        public UnitOfWork(OrderContext context)
        {
            _context = context;
            Orders = new OrderRepository(context);
        }

        public void Dispose() =>
            _context.Dispose();

        public void Save()
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
