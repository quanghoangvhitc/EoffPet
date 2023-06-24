using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanUnitGroup : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public override string GetServerUrl()
        {
            //http://petrolimex.vuthao.com.vn/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=Nhóm đơn vị&type=1&cols=["ID","Title"]
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Nhóm đơn vị") + "&type=1&cols=[\"ID\", \"Title\"]";
        }
    }
}
