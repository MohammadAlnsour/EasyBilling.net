using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Infrastructure.Integration.PaymentProviders
{
    public interface IProvider
    {
        string Name { get; }
        string Description { get; }
        R PostInvoice<T, R>(T invoice);
    }
}
