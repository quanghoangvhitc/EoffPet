using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanCodeItem : BeanBase
    { 
        [PrimaryKey, PrimaryKeyS]
        public long ID { get; set; }
        public string SPListId { get; set; }
        public int? SPItemId { get; set; }
        public int CodeCategoryId { get; set; } //Loại văn bản : 1-Văn bản, 2-Hợp đồng, 3- Biên bản thanh toán, 4-Tờ trình/báo nội bộ, 5-Biên bản nội bộ, 6-Hồ sơ tài liệu.
        public string Title { get; set; }
        public string CodeCategoryTitle { get; set; }
        public string TitleShort { get; set; }
        public int? Step { get; set; }
        public DateTime? SignDate { get; set; }
        public int DocSignType { get; set; }
        public string DocumentName { get; set; }
        public int CodeDocumentTypeId { get; set; }
        public int? CodeFieldId { get; set; }
        public int CodeContractFieldId { get; set; }
        public int DepartmentId { get; set; }
        public int CodeDepartmentId { get; set; }
        public bool? IsDelegate { get; set; }
        public int YearShort { get; set; }
        public string DocumentTitle { get; set; }
        public int Year { get; set; }
        public string SoBienBanHop { get; set; }
        public string Subject { get; set; }
        public string Customer { get; set; }
        public string CIF { get; set; }
        public string HoSoTitle { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string CodeRefer { get; set; }
        public string VersionText { get; set; }
        public int Version { get; set; }
        public string Partner { get; set; }
        public string PartnerNum { get; set; }
        public string ThamQuyenDuyet { get; set; }
        public string Author { get; set; }
        public string TicketRequestDetails { get; set; } //Json ticket details
        public int? WorkflowStep { get; set; }
        public string CCForm { get; set; }//lockup sharepoint ;#
        public string UserCC { get; set; }//Gửi mail
        public string Users { get; set; } //lockup sharepoint ;#
        public string ChooseUserValue { get; set; }  //lockup sharepoint ;#
        public string CommentValue { get; set; }
        public string Department { get; set; }
        [RemoveAccentFromMultiColumn(new string[] { "Subject", "DocumentTitle" })]
        public string SearchData { get; set; }

        /// <summary>
        /// 0: Draft
        /// 1: In progress
        /// 2: Approve/Completed
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        //-Chờ phát hành
        //-Đã phát hành
        //-Đã lấy số
        //-Kết thúc
        //-Phê duyệt
        /// </summary>
        public string StatusText { get; set; }
        public string RootFieldText { get; set; }
        public string UserSigned { get; set; }
        public int WorkflowId { get; set; }
        public bool ConfirmMarked { get; set; }
        public string NguoiNhan { get; set; }
        public string NoiNhan { get; set; }
        public string ActionJson { get; set; }
        public string NoiLuu { get; set; }
        public int? ModuleId { get; set; }
        public string EmailExternal { get; set; }
        public string WorkLocationId { get; set; }
        /// <summary>
        /// Người được chia sẻ
        /// </summary>
        public string UserShared { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Approver { get; set; }
        /// <summary>
        /// Ngày phê duyệt hoàn tất
        /// </summary>
        public DateTime? ApprovedDate { get; set; }
        public string AssignedToText { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string YKienTrinh { get; set; }
        public string ListId { get; set; }
        public string WorkflowHistory { get; set; }//2 item mới nhất trong quá trình luân chuyển 

        [Ignore]
        public string UrlApiErr { get; set; }
        /// <summary>
        /// Danh sach cot can update
        /// </summary>
        [Ignore]
        public List<string> UpdateCols { get; set; }
        // Ignore field
        public string DepartmentTitle { get; set; }
        [Ignore]
        public string CodeDepartmentCode { get; set; }
        [Ignore]
        public string WorkLocationGroup { get; set; }
        public string CreatedByText { get; set; }
        /// <summary>
        /// Khi tạo văn bản có thể chọn bộ hồ sơ để lưu vào luôn, cột này để chứa giá trị id bộ hồ sơ trong trường hợp đó
        /// </summary>
        [Ignore]
        public long AddToSetOfRecord { get; set; }
        //[Ignore]
        //public long NotifyId { get; set; }
        [Ignore]
        public string CodeFieldTitle { get; set; }
        // Sau này sẽ bỏ, thêm vào để clone qua không bị lỗi
        [Ignore]
        public long? ContractValue { get; set; }
        // Sau này sẽ bỏ, thêm vào để clone qua không bị lỗi        
        public string CodeDocumentTypeTitle { get; set; }

        public string ImagePath { get; set; }

        // Property này để join với User
        [Ignore]
        public string Position { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiDocMan.ashx?func=get";
        }
    }
}
