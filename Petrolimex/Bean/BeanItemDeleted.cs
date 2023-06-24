using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Petrolimex.Bean
{
    public class BeanItemDeleted : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string TableName { get; set; }
        public string BeanName { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.Api/ApiPublish.ashx?func=getItemDeleted&Modified=" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
        }

        //de cap nhat sau
        //public override string GetServerUrl()
        //{
        //    return "/_layouts/15/VuThao.SNP.API/ApiHandler.ashx?func=getdata";
        //}
    }
}
