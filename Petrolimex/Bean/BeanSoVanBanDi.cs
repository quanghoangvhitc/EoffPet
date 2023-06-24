using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanSoVanBanDi : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public int SoBatDau { get; set; }
        public string Nam { get; set; }
        public string KyHieu { get; set; }
        public bool Default { get; set; }
        public bool TinhTrangSo { get; set; }

        public override string GetServerUrl()
        {
            //http://petrolimex.vuthao.com.vn/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=Sổ văn bản đi&type=1&cols=["ID","Title","SoBatDau","Nam","KyHieu"]
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Sổ văn bản đi") + "&type=1&cols=[\"ID\", \"Title\", \"SoBatDau\", \"Nam\", \"KyHieu\", \"Default\", \"TinhTrangSo\"]";
        }
    }
}
