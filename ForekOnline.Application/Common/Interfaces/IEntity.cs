using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IEntity
    {
        string Code { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        Guid Id { get; set; }
        bool IsRetired { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string UserCreated { get; set; }
        string UserModified { get; set; }

      
    }
}
