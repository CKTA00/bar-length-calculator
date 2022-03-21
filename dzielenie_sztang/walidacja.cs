using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace bar_length_calculator
{
    class Walidacja : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            float v;
            if(float.TryParse(value.ToString(),out v))
            {
                if(v<=0)
                {
                    result = new ValidationResult(false, "Wartość musi być dodatnia.");
                }
            }
            else
            {
                result = new ValidationResult(false, "Nieprawidłowa wartość.");
            }
            return result;
        }
    }
}
