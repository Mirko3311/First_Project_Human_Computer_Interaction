using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace ASystem
{
    public class AcademicYearValidationRule : ValidationRule
    {
        public int MinValue { get; set; } = 1;  
        public int MaxValue { get; set; } = 5;  

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            string message = (string)Application.Current.Resources["AcademicIntegerValue"];
            string message1 = (string)Application.Current.Resources["EmptyField"];
            string message2 = (string)Application.Current.Resources["AcademicYearRange"];
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) 
                return new ValidationResult(false, message1);
            if (!int.TryParse(value.ToString(), out int year))
                return new ValidationResult(false, message);

            if (year < MinValue || year > MaxValue)
                return new ValidationResult(false, message2);

            return ValidationResult.ValidResult;
        }
    }
}
