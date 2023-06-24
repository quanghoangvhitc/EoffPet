using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Class
{
    [Serializable]
    public class ConfigVariable
    {
        public ConfigVariable()
        {
            this.DelaySynsTime = 120; // seconds
            this.DataLimitDay = 30;
            this.DataLimit = 20;
            this.AppConfigVersion = "1.0.0";
        }

        public string GetCurrentUserUrl { get; set; }// Đường dẫn lấy thông tin User

        //public string Domain { get; set; }      // Domain kết nối
        public string Subsite { get; set; }     // Subsite kết nối

        public string UserId { get; set; }         // userId login account
        public string AvatarPath { get; set; }
        public string PersonID { get; set; }    // user ID duoi sql Guid
        public string LoginName { get; set; }   // userId login account
        public string SiteName { get; set; }    // SubSite, User thuộc đợn vị Subsite
        public string Title { get; set; }           // user name login account
        public string LoginPassword { get; set; }   // password login account
        public bool ViewPrint { get; set; }
        public string DefaultSite { get; set; }
        public string DisplayName { get; set; }     // display name login account
        public string Email { get; set; }           // email login account
        public string Mobile { get; set; }
        public int? AccountID { get; set; }
        public string Department { get; set; }

        public int DepartmentID { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
        public bool Authencate { get; set; }
        public DateTime? Birthday { get; set; }
        public string Position { get; set; }
        public string DeviceInfo { get; set; }  // Thông tin Device Build bằng Json
        public string AppConfigVersion { get; set; }

        public bool RememberMe { get; set; }
        public int DelaySynsTime { get; set; } // Thời gian lặp lại syns dữ liệu tính bằn phút
        public int DataLimitDay { get; set; } // Số ngày dữ liệu sẽ lưu lại trên máy
        public int DataLimit { get; set; } // Số lượng dữ liệu sẽ tải thêm từ trên server
        public bool IsVanThu { get; set; } //Role văn thư
        public string LoginDomain { get;set;} //Domain Login
    }
}
