using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanBanLanhDao : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string DonVi { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public string LanhDao { get; set; }
        public string UyQuyen { get; set; }
        public int Orders { get; set; }
        public string Group { get; set; }
        public bool OneAssign { get; set; }
        public string ThayThe { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public bool IsLoadImage { get; set; }

        public override string GetServerUrl()
        {
            ////return System.Web.HttpUtility.UrlDecode("/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=getV2&type=1&lname=Ban%20l%C3%A3nh%20%C4%91%E1%BA%A1o&cols=[%22ID%22,%20%22Title%22,%20%22DonVi%22,%20%22Modified%22,%20%22Created%22,%20%22LanhDao%22,%20%22UyQuyen%22,%20%22Orders%22,%20%22Group%22,%20%22OneAssign%22,%20%22ThayThe%22]&wname=vanban");
            // https://stackoverflow.com/questions/1405048/how-do-i-decode-a-url-parameter-using-c
            return System.Uri.UnescapeDataString("<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=getV2&type=1&lname=Ban%20l%C3%A3nh%20%C4%91%E1%BA%A1o&cols=[%22ID%22,%20%22Title%22,%20%22DonVi%22,%20%22Modified%22,%20%22Created%22,%20%22LanhDao%22,%20%22UyQuyen%22,%20%22Orders%22,%20%22Group%22,%20%22OneAssign%22,%20%22ThayThe%22]&wname=vanban");
        }
    }
}
