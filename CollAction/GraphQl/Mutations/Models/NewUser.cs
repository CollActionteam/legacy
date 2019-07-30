using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Mutations
{
    public class NewUser
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool SubscribeNewsletter { get; set; }
    }
}
