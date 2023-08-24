using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public sealed class CompanyNotFoundException : NotFoundException
    {
        public CompanyNotFoundException(Guid companyid)
            : base($"The company with id: {companyid} doesn't exist in the database.")
        {
        }
    }
}
