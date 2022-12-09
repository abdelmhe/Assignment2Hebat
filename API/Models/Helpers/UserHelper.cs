using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Persistence;

namespace API.Models.Helpers
{
    public class UserHelper
    {
        
        public static bool IsEmailPresent(DataContext context, string email)
        {
            return context.Users.Count(u => u.Email.Equals(email)) > 0;
        }

        public static bool IsEmailValid(string email)
        {
            return email.Contains('@') && email.Contains('.');
        }

        public static bool IsUserPresent(DataContext context, string userId)
        {
            User user = context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(userId)));
            return user != null;
        }
        
    }
}