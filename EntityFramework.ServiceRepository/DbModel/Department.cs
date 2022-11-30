using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.ServiceRepository.DbModel
{
    public class Department
    {

        [Key]
        public Guid department_Id { get; set; }


        public string name { get; set; }

        public string head_of_department { get; set; }


        public string head_count { get; set; }

    }
}
