using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Petrolimex.Class;

namespace Petrolimex.Bean
{
    public class BeanUser : BeanBase
    {
        public int AppUserId { get; set; }
        [PrimaryKey, PrimaryKeyS]
        public string ID_SQL { get; set; }
        public int? AccountID { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string ImagePath { get; set; }
        public string Manager { get; set; }
        public string DepartmentManager { get; set; }
        public string StaffID { get; set; }
        public string Position { get; set; }
        public bool UserStatus { get; set; }
        public bool Gender { get; set; }
        public int? After_CompletedDate { get; set; }
        public DateTime? DayOfHire { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public bool ViewPrint { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string PersonID { get; set; }
        public string Department { get; set; }
        public string DepartmentTitle { get; set; }
        public int DepartmentID { get; set; }
        public string SiteName { get; set; }
        public DateTime Modified { get; set; }
        public string Editor { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public bool IsVanThu { get; set; }

        [Ignore]
        public string PositionText
        {
            get
            {
                if (string.IsNullOrEmpty(Position))
                    return string.Empty;

                if (Position.Contains(";#"))
                    return Position.Split(new[] { ";#" }, StringSplitOptions.None)[1];
                else
                    return Position;
            }
        }

        [Ignore]
        public string DefaultSite { get; set; }

        [Ignore]
        public int UserType { get; set; } // Phân biệt User hay Group
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public bool IsLoadImage { get; set; }

        [Ignore]
        public bool IsThucHienSelected { get; set; }
        [Ignore]
        public bool IsPhoiHopSelected { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiUser.ashx?func=getUser";
        }
        public string GetCurrentUserUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiUser.ashx?func=CurrentUser";
        }

        public BeanUser CloneBean()
        {
            return (BeanUser)this.MemberwiseClone();
        }
    }
}
