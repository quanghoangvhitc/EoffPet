using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanLoaiVanBan : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }

        public int Orders { get; set; }
        public string Code { get; set; }
        public bool Default { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        public string Template { get; set; }
        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Loại văn bản") + "&type=1&cols=[\"ID\",\"Title\",\"Orders\",\"Code\",\"Default\",\"Template\"]";
        }
    }
}
