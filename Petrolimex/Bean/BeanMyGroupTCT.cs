using System;
using SQLite;
using Petrolimex.Class;
namespace Petrolimex.Bean
{
    public class BeanMyGroupTCT : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public Int32 ID { get; set; }
        public string Name { get; set; }

        public override string GetServerUrl()
        {
            // Vinh Bs sau
            //return "/vanbantct/_layouts/15/VuThao.SNP.API/ApiUser.ashx?func=MyGroup";
            return "";
        }
    }
}
