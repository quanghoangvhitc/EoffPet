using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanLoaiDinhKem : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public int Orders { get; set; }
        public DateTime? Modified { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Loại đính kèm") + "&type=1&cols=[\"ID\", \"Title\", \"Orders\", \"Modified\"]";
        }
    }
}
