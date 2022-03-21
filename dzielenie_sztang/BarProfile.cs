using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    public class BarProfile
    {
        public string profileName { get; set; }
        public string presentationName { get; set; }

        public BarProfile(string presentationName)
        {
            if(String.IsNullOrWhiteSpace(presentationName))
            {
                this.presentationName = "";
                profileName = "profil nieistotny"; // TODO: Make this string language dependent
            }
            else
            {
                this.presentationName = presentationName;
                profileName = presentationName;
            }
        }
    }
}
