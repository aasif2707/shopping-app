using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application
{
    public class OrderException :ApplicationException
    {
        public OrderException(string name, object key) : base($"Entity {name} - {key} is not found.") { }
    }
}
