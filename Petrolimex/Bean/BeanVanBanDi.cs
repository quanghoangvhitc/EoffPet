using System;
using SQLite;
using Petrolimex.Class;
namespace Petrolimex.Bean
{
    public class BeanVanBanDi : BeanVanBan
    {
        public string Title { get; set; }
        public string TrichYeu { get; set; }
        public string DonVi { get; set; }
        public string LoaiVanBan { get; set; }
        public string ActionStatus { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }// Chỉ chứa danh sách tên người xử lý
        public bool Read { get; set; }
        public string FilesJson { get; set; }
        public string YKien { get; set; }
        public string CCForm { get; set; }
        public string Workflow { get; set; }
        public int BtnAction { get; set; }

        [Ignore]
        public string CommentValue { get; set; }        // Đây không phải là field trên List, giá trị nhập ý kiến để lưu

        [Ignore]
        public string ChooseUserValue { get; set; }     // Đây không phải là field trên List, nhằm để chọn người duyệt ở mỗi bước

        [Ignore]
        public string UserForwardValue { get; set; }     // Đây không phải là field trên List, nhằm để chứa giá trị chuyển xử lý cho ai (ID;#Name)

        [Ignore]
        public string DanhSachActionJson { get; set; }  // Đây không phải là field trên List, Danh sách hành động của workflow

        [Ignore]
        public string QuaTrinhLuanChuyenJson { get; set; }  // Đây không phải là field trên List, Danh sách thông tin xử lý

        public DateTime Modified { get; set; }

        //public DateTime? Created { get; set; }

        public string Author { get; set; }
        public string ImagePath { get; set; }

        public string Editor { get; set; }

        public override string GetApiUrlExec()
        {
            //Vinh BS sau
            //return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.Api/ApiWorkflow.ashx";
            return "";
        }

        public override string GetServerUrl()
        {
            //Vinh BS sau
            //if (string.IsNullOrEmpty(CmmVariable.sysConfig.SiteName))
            //    return "<#SiteName#>/vanbantct/_layouts/15/VuThao.SNP.API/ApiVanBanBanHanh.ashx?func=get";
            //else
            //    return "<#SiteName#>/vanban/_layouts/15/VuThao.SNP.API/ApiVanBanBanHanh.ashx?func=get";
            return "";
        }
    }
}
