using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanLinhVuc : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Parent { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" 
                    + System.Web.HttpUtility.UrlEncode("Lĩnh vực") 
                    + "&type=1&cols=[\"ID\",\"Title\",\"Parent\"]";
        }
    }
}
