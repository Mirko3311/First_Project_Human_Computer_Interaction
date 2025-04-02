using PrviProjektniZadatakHCI.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ASystem
{
    public class EmailValidationRule : ValidationRule
    {
        private static readonly Regex _emailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled);

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string email = value as string;
            string message = (string)Application.Current.Resources["InvalidEmailFormat"];
         
            if (!_emailRegex.IsMatch(email))
                return new ValidationResult(false, message);

            return ValidationResult.ValidResult;
        }
    }
}
