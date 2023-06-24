using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanNguoiKyVanBan : BeanBase
    {
        [PrimaryKey,PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public bool? IsHidden { get; set; }
        public string ChucVu { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Người ký văn bản") + "&type=1&cols=[\"ID\",\"Title\",\"IsHidden\",\"ChucVu\"]";
        }
    }
}
