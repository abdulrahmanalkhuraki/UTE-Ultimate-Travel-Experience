using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Name { get; }
        bool IsAuthenticated { get; }
    }
}
