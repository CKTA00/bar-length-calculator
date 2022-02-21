using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dzielenie_sztang
{
    public class Profil
    {
        public string nazwa_profilu { get; set; }
        public string prezentacja { get; set; }
        int ID { get; }
        static int obecneID;

        public Profil(string prezentacja)
        {
            if(String.IsNullOrWhiteSpace(prezentacja))
            {
                this.prezentacja = "";
                nazwa_profilu = "profil nieistotny";
            }
            else
            {
                this.prezentacja = prezentacja;
                nazwa_profilu = prezentacja;
            }
            ID = obecneID++;
        }
    }
}
