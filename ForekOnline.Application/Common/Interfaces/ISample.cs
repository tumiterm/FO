using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface ISample : IRepository<Sample>
    {
        Task<Sample> Update(Sample sample);
    }
}
