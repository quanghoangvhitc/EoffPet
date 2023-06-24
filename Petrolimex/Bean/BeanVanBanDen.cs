using System;
using SQLite;
using Petrolimex.Class;
using System.ComponentModel;
namespace Petrolimex.Bean
{
    public class BeanVanBanDen : BeanVanBan
    {
        public string AssignedTo { get; set; }
        public bool TepDinhKem { get; set; } // đưa vào trường Files nếu có giá trị thì là true
        public bool ChenSo { get; set; }
        public string UserShared { get; set; }
        public string DonVi { get; set; }
        public string Files { get; set; }
        public string DoKhan { get; set; }
        public string DoMat { get; set; }
        public string FilesYKLD { get; set; }
        public string GroupLanhDaoVPDN { get; set; }
        public string GroupThayTheLanhDaoVPDN { get; set; }
        public string GroupUyQuyenLanhDaoVPDN { get; set; }
        public string ItemVBDTCT { get; set; }
        public string ItemVBPH { get; set; }
        public string LinhVuc { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string ActionJson { get; set; }
        public string DocumentType { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CommentJson { get; set; }
        public string CoQuanGui { get; set; }
        public string CoQuanGuiText { get; set; }           // Lúc tạo mới văn bản, có giá trị do user tự nhập
        public DateTime? NgayDen { get; set; }
        public DateTime? NgayTrenVB { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string NguoiDanhGiaUserValue { get; set; }
        public int SoBan { get; set; }
        public string SoDen { get; set; }               // Số đến
        public int SoTrang { get; set; }
        public string SoVanBan { get; set; }            // Sổ văn bản đến
        public string Title { get; set; }                   // Số văn bản
        public int Step { get; set; }                       // 2: BOD1 trình lên BOD2, 3: BOD2 chuyển ngang, 4: BOD2 đã chuyển về BOD1 và BOD1 trình lên
        public string LoaiVanBan { get; set; }
        public string TrangThai { get; set; }
        public DateTime? ThoiHanGQ { get; set; }
        public string TrichYeu { get; set; }
        public string BanLanhDao { get; set; }
        public string VanBanTraLoi { get; set; }
        public string LanhDaoTCT { get; set; }
        public string LanhDaoTCTDeBiet { get; set; }
        public string YKienLanhDaoTCT { get; set; }
        public string YKienLanhDaoTCTJson { get; set; }
        public string YKienLanhDao { get; set; }
        public string LanhDaoVPDN { get; set; }
        public string YKienLanhDaoJson { get; set; }
        public string YKienLanhDaoVPDN { get; set; }
        public string YKienLanhDaoVPDNJson { get; set; }
        public string YKienCuaLDVPChoVanThu { get; set; }
        public string YKienCuaLDVPChoVanThuJson { get; set; }
        public string YKien { get; set; }
        public string SubmitLanhDaoTCTJSON { get; set; }
        public string SubmitLanhDaoTCTDeBietJSON { get; set; }
        public string YearMonth { get; set; } // archive data
        public bool IsLibrary { get; set; }
        public int ModuleId { get; set; } // 10,13 => TCT
        public int Role { get; set; }
        public string SiteName { get; set; }
        public string ListName { get; set; }
        public string Text1 { get; set; }
        public string Value { get; set; }
        public string TicketRequestDetails { get; set; }
        public string SoVanBanNum { get; set; }
        public string DocumentTypeText { get; set; }
        public string BanLanhDaoDaXuLy { get; set; }
        public string BanLanhDaoPrevious { get; set; }
        public string CreatedBy { get; set; }

        [Ignore]
        public string Position { get; set; }

        [Ignore]
        public string UuTien { get; set; } // Set ưu tiên cho action chuyển xử lý

        [Ignore]
        public string ListIDDelete { get; set; } // Danh sách ban đã phân công trước đó khi chọn action phân công lại
        public int BtnAction { get; set; }
        [Ignore]
        public string NguoiChuyen { get; set; } // Người chuyển phân công

        [Ignore]
        public string actionType { get; set; } // Check Bổ sung
        [Ignore]
        public string chkPhanCong { get; set; } //Chọn Dropdown phân công
        [Ignore]
        public string chkLanhDao { get; set; } //Chọn Dropdown lãnh đạo
        [Ignore]
        public string ListAssignmentUsers { get; set; } // Chọn phân công phòng ban
        [Ignore]
        public string ListAssignmentDept { get; set; } // Chọn phân công đơn vị
        [Ignore]
        public string chkLanhDaoValue { get; set; } // Giá trị của dropdown list khi chọn lãnh đạo
        [Ignore]
        public string NguoiNhan { get; set; } // Người nhạn lấy từ dropdown list khi click chọn lãnh đạo
        [Ignore]
        public string UserCC { get; set; } // CC nguoi co the xem
        [Ignore]
        public string chkCVP_TPTHValue { get; set; } // Giá trị drowdown khi check chuyển dến CPV/TP tổng hợp
        [Ignore]
        public string chkCVP_TPTH { get; set; } // Có check vào chuyển dến CPV/TP tổng hợp hay không
        [Ignore]
        public string CCFrom { get; set; } // CC nguoi co the xem
        [Ignore]
        public string NewFiles { get; set; }        // Loại file
        [Ignore]
        public string AssignmentUser { get; set; }          //Đây không phải là Field trên list, khi phân công thêm user hoặc đơn vị xử lý với kiến trúc: UserId;#UserName&&ThucHien&&DeBiet&&DeBaoCao&&DueDate&&Comment&&DanhGia (@@)
        [Ignore]
        public string AssignmentDepartment { get; set; }    //Đây không phải là Field trên list, khi phân công đơn vị xử lý với kiến trúc: ID;#Title&&Url&&Duedate(yyyy-MM-dd)@@ Lặp lại.
        [Ignore]
        public string DeleteAssignmentUser { get; set; }    //Đây không phải là Field trên list, khi phân công sẽ xóa bỏ phân công với kiến trúc: TaskId|TaskId
        [Ignore]
        public string CommentValue { get; set; }            //Đây không phải là Field trên list, Chứa ItemId của ListComments để khi Lãnh đạo cho ý kiến mà chọn từ danh mục ý kiến trong autocomplete thì không cần kiểm tra và lưu trữ.
        public bool Read { get; set; }                      // Đây không phải là Field trên list, True là đã đọc văn bản này rồi
        [Ignore]
        public string TaskVanBanJson { get; set; }          //Đây không phải là Field trên list, Chứa danh sách thông tin Task xử lý
        [Ignore]
        public string NhiemVuXuLyJson { get; set; }          //Đây không phải là Field trên list, Chứa danh sách thông tin Đơn vị đã chuyển
        [Ignore]
        public string QuaTrinhLuanChuyenJson { get; set; }  //Đây không phải là Field trên list, Chứa danh sách quá trình luân chuyển văn bản
        [Ignore]
        public string QuaTrinhLuanChuyenDonViJson { get; set; }  //Đây không phải là Field trên list, Chứa danh sách quá trình luân chuyển văn bản của các đơn vị đã chuyển
        [Ignore]
        public string DanhSachNguoiXemJson { get; set; }   //Đây không phải là Field trên list, Chứa danh sách người xem văn bản
        public string DanhSachActionJson { get; set; }    //Đây không phải là Field trên list, {Type : Dictionary<int,BeanVanBanAction>}, Chứa danh sách những Action được cho phép của User hiện tại trong hệ thống

        [Ignore]
        public string NguoiNhanDeBaoCaoText { get; set; }   //Đây không phải là Field trên list, Chỉ để hiển thị danh sách người phối hợp      
        [Ignore]
        public string BanLanhDaoLevel1 { get; set; }        //Đây không phải là Field trên list, Khi Lãnh đạo cấp 1 đang vào thao tác chọn trình lãnh đạo cấp 1

        public Int32 MobileSyncID { get; set; }

        [Ignore]
        public DateTime? ModifiedForeignTable { get; set; }  // Đây là ngày gọi API lấy data các List liên quan như : Thông tin luân chuyển,DS người xem
        [Ignore]
        public DateTime? ModifiedTask { get; set; }         //Đây là ngày gọi API get văn bản hoặc ngày gọi API getTask
        [Ignore]
        public DateTime? ModifiedNVXL { get; set; }         // Đây là ngày gọi API get văn bản hoặc ngày gọi API getNVXL
        public DateTime Modified { get; set; }
        [Ignore]
        public string Author { get; set; }
        [Ignore]
        public string Editor { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }                //Dùng để kiểm tra object có được focus hay không
        [RemoveAccentFrom("TrichYeu")]
        public string SearchData { get; set; }
        [RemoveAccentFrom("BanLanhDao")]
        public string BanLanhDaoNoAccent { get; set; }

        public string ImagePath { get; set; }

        [Ignore]
        public string UrlApiErr { get; set; }

        public string GetApiUrlExecTCT()
        {
            //Vinh bo sung sau
            //return "/vanbantct/_layouts/15/VuThao.SNP.API/ApiVanBanDen.ashx";
            return "";
        }
        public override string GetApiUrlExec()
        {
            //Vinh bo sung sau
            //return "<#SiteName#>/vanban/_layouts/15/VuThao.SNP.API/ApiVanBanDen.ashx";
            return "";
        }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanDenMobile.ashx?func=get";
        }
    }
}
