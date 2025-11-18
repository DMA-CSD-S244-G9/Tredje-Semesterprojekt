using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.WinFormsApp.Forms
{
    public class Influencer
    {
        public int Id { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public DateTime Fødselsdato { get; set; }
        public string CPRNummer { get; set; }   // gem originalt (maskeres kun i grid)
        public bool IsVerified { get; set; }

        // Beregnet property (viser i grid, men lagres ikke i DB)
        public int Alder
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - Fødselsdato.Year;
                if (Fødselsdato.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
