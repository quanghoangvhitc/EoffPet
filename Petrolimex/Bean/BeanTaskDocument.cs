using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanTaskDocument : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public long ID { get; set; }
        public int DepartmentId { get; set; }
        public long VBId { get; set; }
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public bool DanhGia { get; set; }
        public bool DeBaoCao { get; set; }
        public bool DeBiet { get; set; }
        public bool DeThucHien { get; set; }
        public int Role { get; set; }
        public string ActionJson { get; set; }

        public int DiemChatLuong { get; set; }
        public int DiemThoiGian { get; set; }
        public int DiemDanhGia { get; set; }
        public bool HoanTatTuDong { get; set; }
        public bool AssignedToType { get; set; } // 0 -User / 1-Group
        public int ActionPermission { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public DateTime? ThoiHanGQ { get; set; } // Thời hạn giải quyết của Phân công Đơn vị
        /// <summary>
        /// Kieu Json {ID,Url,Title}
        /// </summary>
        public string HoSoXuLyUrl { get; set; }
        public int Percent { get; set; }
        public string TrangThai { get; set; }
        public int ModuleId { get; set; }
        public int ItemId { get; set; }
        public string SiteName { get; set; }
        public string ListName { get; set; }
        public int ParentItemId { get; set; }


        /// <summary>
        /// 1: High; 2: Normal; 3: Low
        /// </summary>
        public int Priority { get; set; }
        public string YKienCuaNguoiGiaiQuyet { get; set; }
        public string YKienChiDao { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }

        public string AssignedToText { get; set; }
        public string AssignorText { get; set; }

        /// <summary>
        /// Giá trị item id của văn bản trên List
        /// </summary>
        /// 
        [Ignore]
        public string HoSoDuThaoUrl { get; set; }
        [Ignore]
        public long ItemVBId { get; set; }
        [Ignore]
        public string DepartmentTitle { get; set; }
        [Ignore]
        public string DepartmentName { get; set; }
        [Ignore]
        public string DepartmentUrl { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public string TrichYeu { get; set; }
        [Ignore]
        /// <summary>
        /// Số văn bản
        /// </summary>
        public string SoVB { get; set; }
        [Ignore]
        public string Files { get; set; }
        [Ignore]
        public string AssignedToUserValue { get; set; }
        [Ignore]
        public string AssignmentUser { get; set; }          //Đây không phải là Field trên list, khi phân công thêm user hoặc đơn vị xử lý với kiến trúc: UserId;#UserName&&ThucHien&&DeBiet&&DeBaoCao&&DueDate&&Comment&&DanhGia (@@)
        [Ignore]
        public string NguoiPhanViecUserValue { get; set; }
        [Ignore]
        public string DanhSachActionJson { get; set; }
        /// <summary>
        /// Ý kiến từ mobile gửi lên
        /// </summary>
        [Ignore]
        public string YKien { get; set; }
        /// <summary>
        /// Thời hạn giải quyết từ mobile gửi lên
        /// </summary>
        [Ignore]
        public DateTime? ThoiHanGiaiQuyet { get; set; }
        [Ignore]
        public string NguoiChuyen { get; set; }
        [Ignore]
        public string ListAssignmentUsers { get; set; }

        [Ignore]
        public List<BeanTaskDocument> ListParentTask { get; set; }

        [Ignore]
        public List<BeanTaskDocument> ListUserTask { get; set; }
        [Ignore]
        public bool IsDonVi { get; set; }
    }
}
