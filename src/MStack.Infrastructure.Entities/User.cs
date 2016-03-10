using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Infrastructure.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string HashedPassword { get; set; }
    }
}
