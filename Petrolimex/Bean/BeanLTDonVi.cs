using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanLTDonVi : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string PID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Index { get; set; }
        public bool? Status { get; set; }
        public int? ChildDept { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
    }
}
