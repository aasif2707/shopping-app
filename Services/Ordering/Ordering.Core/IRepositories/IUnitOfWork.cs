using Ordering.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Core.IRepositories
{
    public interface IUnitOfWork :IDisposable
    {
        IOrderRepository Orders { get; }
        void Save();
    }
}
