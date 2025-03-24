using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using PrviProjektniZadatakHCI.DataAccess;
namespace ASystem
{
     public class DomaciZadatak
    {
     

        public string idDomaciZadatak { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }
        public DateTime rok { get; set; }

        public int idPredmeta { get; set; }
        public int idProfesora { get; set; }
        public Predmet predmet { get; set; }

        public DomaciZadatak DeepCopy()
        {
            return (DomaciZadatak)this.MemberwiseClone();
        }

    }
}
