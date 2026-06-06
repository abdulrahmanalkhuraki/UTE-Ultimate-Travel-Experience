using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User.Request
{
    public sealed record DeleteAccountRequest
    (
        string Password
    );
}
