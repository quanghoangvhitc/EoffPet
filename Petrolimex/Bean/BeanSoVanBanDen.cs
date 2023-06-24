using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanSoVanBanDen : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public bool TinhTrangSo { get; set; }
        public bool DonThu { get; set; }
        public int Orders { get; set; }
        public bool Default { get; set; }
        public string BanLanhDao { get; set; }
        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Sổ văn bản đến") + "&type=1&cols=[\"ID\", \"Title\", \"TinhTrangSo\", \"DonThu\", \"Orders\", \"Default\", \"BanLanhDao\"]";
        }
    }
}
