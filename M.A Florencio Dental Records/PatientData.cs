using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M.A_Florencio_Dental_Records
{
    public class PatientData
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string CivilStatus { get; set; }
        public string Religion { get; set; }
        public string GuardianName { get; set; }
        public string GuardianContact { get; set; }
        public string GuardianRelationship { get; set; }

        public DateTime TreatmentDate { get; set; }
        public string Procedure { get; set; }
        public string Remarks { get; set; }
        public string MedicalHistory { get; set; }
    }
}
