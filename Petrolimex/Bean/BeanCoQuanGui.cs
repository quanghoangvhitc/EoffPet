using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanCoQuanGui : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Parent { get; set; }
        public int? Order { get; set; }
        [Ignore]
        public List<BeanCoQuanGui> Items { get; set; }
        [Ignore]
        public bool IsExpanded { get; set; }
        [Ignore]
        public bool IsCanExpand { get; set; }
        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&lname=" + System.Web.HttpUtility.UrlEncode("Cơ quan gửi") + "&type=1&cols=[\"ID\",\"Title\",\"Parent\",\"Order\"]";
        }
    }
}
