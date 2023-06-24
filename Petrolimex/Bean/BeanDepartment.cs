using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanDepartment : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int? ID { get; set; }
        public string SiteName { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int? ParentID { get; set; }
        public string ChartCode { get; set; }
        public string Manager { get; set; }
        public string GroupManager { get; set; }
        public int? DeptLevel { get; set; }
        public string ParentDept { get; set; }
        public string Url { get; set; }
        public int? Order { get; set; }
        public int? DeptStatus { get; set; }
        public string Status { get; set; }
        public bool? IsRoot { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string DepartmentTitle { get; set; } // cái này để lưu tạm của User khi chọn phòng ban
        public int? TotalChildCount { get; set; }

        public string SPNhomUserId { get; set; } // Field này dùng để phân công Task văn bản đến

        public int? ChildCount { get; set; }
        [Ignore]
        public bool IsExpanded { get; set; } // Biến này dùng để lưu trạng thái collapse/expand của ExpandableListview
        [Ignore]
        public string Comment { get; set; } //Comment
        [Ignore]
        public string HanXuLy { get; set; } // Han Xu Ly cua phan cong
        [Ignore]
        public bool IsCanExpand { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public bool IsThucHienSelected { get; set; }
        [Ignore]
        public bool IsPhoiHopSelected { get; set; }
        [Ignore]
        public string Position { get; set; }
        [Ignore]
        public bool IsUser { get; set; }
        [Ignore]
        public string ImagePath { get; set; }
        public string MultipleManager { get; set; }
        public string MultipleManagerGroup { get; set; } // Field này dùng để phân công Task văn bản đến -  Hiện tại đã đổi thành SPNhomUserId
        [Ignore]
        public List<BeanDepartment> ListChildUser { get; set; }
        [Ignore]
        public List<BeanDepartment> ListChildDepartment { get; set; }
        public override string GetServerUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiMobilePublic.ashx?func=getV2&type=2&bname=" + GetType().Name;
        }
        public BeanDepartment CloneBean()
        {
            return (BeanDepartment)this.MemberwiseClone();
        }
    }
}
