using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanVanBan : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public DateTime? Created { get; set; }
    }
}
