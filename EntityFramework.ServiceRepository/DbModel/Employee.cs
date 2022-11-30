using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.ServiceRepository.DbModel
{
    public class Employee
    {

        [Key]
        public Guid employee_Id { get; set; }

        
        public string first_name { get; set; }

        public string last_name { get; set; }


        public string department { get; set; }

        public DateTime date_of_joining { get; set; }

        public string desination { get; set; }

        public string status { get; set; }


    }
}
