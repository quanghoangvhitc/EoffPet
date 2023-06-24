using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanVanBanBanHanh : BeanVanBan
    {
        public string Title { get; set; }
        public string TitleNum { get; set; }
        public bool? ChenSo { get; set; }
        public string TrangThai { get; set; }
        public string DoKhan { get; set; }
        public string DoMat { get; set; }
        public string DonVi { get; set; }
        public string Files { get; set; }
        public bool? IsLibrary { get; set; }
        public string DocNum { get; set; }
        public string NguoiSoanThaoText { get; set; }
        public int DocSignType { get; set; }
        public string InfoVBDi { get; set; }
        public string ItemVBPH { get; set; }
        public string LoaiBanHanh { get; set; }
        public string DocumentType { get; set; }
        public string NoiLuuTru { get; set; }
        public string AuthorText { get; set; }
        public string NoiNhan { get; set; }
        public string AuthorID { get; set; }
        public string ActionJson { get; set; }
        public int? CodeItemId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? SignDate { get; set; }
        public string NguoiKyVanBan { get; set; }
        public string CommentJson { get; set; }

        [Ignore]
        public string NguoiKyVBRemoveID
        {
            get
            {
                if (string.IsNullOrEmpty(NguoiKyVanBan))
                    return string.Empty;

                if (NguoiKyVanBan.Contains(";#"))
                    return NguoiKyVanBan.Split(new[] { ";#" }, StringSplitOptions.None)[1];

                return NguoiKyVanBan;
            }
        }
        public string NguoiKyVanBanText { get; set; }
        public string ChucVu { get; set; }
        public bool? PhanCong { get; set; }
        public string DonViSoanThao { get; set; }
        public string DonViSoanThaoText { get; set; }
        public string TenVanBan { get; set; }
        public string VBBiThayThe { get; set; }
        public string LinhVuc { get; set; }
        public DateTime? DueDate { get; set; }
        public int? SoBan { get; set; }
        public int? SoTrang { get; set; }
        public string SoVanBan { get; set; }
        public string SoVanBanText { get; set; }
        public string TrichYeu { get; set; }
        public string BanLanhDao { get; set; }
        public string BanLanhDaoTCT { get; set; }
        public string YKien { get; set; }
        public string YKienLanhDao { get; set; }
        public string SoVBDenLink { get; set; }
        public int? BanHanhThem { get; set; }
        public int? ModuleId { get; set; }
        public string SiteName { get; set; }
        public string ListName { get; set; }
        public int? ItemId { get; set; }
        public string YearMonth { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public int? MigrateFlg { get; set; }
        public int? MigrateErrFlg { get; set; }
        public string PeopleText { get; set; }
        public string MigrateErrMess { get; set; }
        public string DocID { get; set; }
        public string TaskJson { get; set; }
        public int? WorkflowStep { get; set; }
        public string LTDonVi { get; set; }
        [RemoveAccentFrom("TrichYeu")]
        public string SearchData { get; set; }
        public int? SoLanThuHoi { get; set; }
        public string EmailExternal { get; set; }
        public bool? IsHSTL { get; set; }
        [Ignore]
        public string ActionType { get; set; }
        [Ignore]
        public string NguoiDanhGiaUserValue { get; set; }
        [Ignore]
        public string AssignmentUser { get; set; }
        [Ignore]
        public string DeleteAssignmentUser { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }  //Dùng để kiểm tra object có được focus hay không
        [Ignore]
        public string UrlApiErr { get; set; }
        public string ImagePath { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiVanBanBanHanhMobile.ashx?func=get";
        }
    }
}
