using Admin.Common;
using System.Collections.Generic;

namespace Admin.CustomerEdit
{
    public interface IEditService
    {
        CustomerModel? EditCustomerInteractive(IList<CustomerModel> customers);
    }
}
