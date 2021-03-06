using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace bar_length_calculator
{
    class ValidatePositiveFloat : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            float v;
            if(float.TryParse(value.ToString(),out v))
            {
                if(v<=0)
                {
                    result = new ValidationResult(false, "Wartość musi być dodatnia."); // TODO: Make this string language dependent
                }
            }
            else
            {
                result = new ValidationResult(false, "Nieprawidłowa wartość."); // TODO: Make this string language dependent
            }
            return result;
        }
    }
}
