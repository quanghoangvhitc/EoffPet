using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
namespace Petrolimex.Bean
{
    public class BeanSettings : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string KEY { get; set; }
        public string VALUE { get; set; }
        public string DESC { get; set; }
        public string DEVICE { get; set; }
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Lấy đường dẫn Url tương ứng lấy dữ liệu từ Server
        /// </summary>
        /// <returns></returns>
        public override string GetServerUrl()
        {
            //return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=getSettings";
        }
    }

}
