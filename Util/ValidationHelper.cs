using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace PrviProjektniZadatakHCI.Util
{
   

    public static class ValidationHelper
    {
        private static readonly string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private static readonly string IdPattern = @"^\d+$";

        public static bool IsValidId(string id)
        {
            return Regex.IsMatch(id, IdPattern);
        }
       
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, EmailPattern);
        }
    }

}
