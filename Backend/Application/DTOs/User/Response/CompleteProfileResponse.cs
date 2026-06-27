using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User.Response
{
    public class CompleteProfileResponse
    {
        public UserResponse User { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
