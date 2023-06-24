using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanMyGroup : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public Int32 ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiUser.ashx?func=get&type=2";
        }
    }
}
