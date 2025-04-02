using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PrviProjektniZadatakHCI.Util;
namespace ASystem
{
    public class IdValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string id = value as string;
            if (string.IsNullOrEmpty(id) || !ValidationHelper.IsValidId(id))
            {
                return new ValidationResult(false, "Identifikator mora biti cijeli broj.");
            }
            return ValidationResult.ValidResult;
        }
    }

}


