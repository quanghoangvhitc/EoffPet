using System;
using System.ComponentModel;
namespace Petrolimex.Class
{
    public enum ControlType
    {
        // 1: Text, 2: Text Area, 4: TextNum, 8: DateTime, 16: Date, 32: Time, 64: Combobox drop, 128: choice, 256: multi, 8388608: User
        FieldText = 1,
        FieldTextArea = 2,
        FieldTextNum = 4,
        FieldDatetime = 8,
        FieldDate = 16,
        FieldTime = 32,
        FieldComboboxDrop = 64,
        FieldChoice = 128,
        FieldMultichoice = 256,
        FieldBool = 512,
        FieldUser = 8388608
    }


    public enum RoleVBDen
    {
        // 32, 1024, 16384 = Lanh dao TCT
        // 128, 2048, 32768 = Lanh dao VP
        GroupBanLanhDao = 4,
        GroupLanhDaoTCT = 32,
        GroupLanhDaoVPDN = 128,
        GroupThayTheLanhDaoTCT = 1024,
        GroupThayTheLanhDaoVPDN = 2048,
        GroupUyQuyenLanhDaoTCT = 16384,
        GroupUyQuyenLanhDaoVPDN = 32768
    }

    public static class WorkflowAction
    {
        public enum Action ///Hành động văn bản đi
        {
            [Description("Duyệt")]
            Next = 1,

            [Description("Đồng ý ban hành")]
            Approve = 2,

            [Description("Chuyển xử lý")]
            Forward = 4,

            [Description("Yêu cầu hiệu chỉnh")]
            Return = 8,

            [Description("Từ chối")]
            Reject = 16,

            [Description("Thu hồi")]
            Recall = 32,

            [Description("Yêu cầu bổ sung")]
            RequestInformation = 64,

            [Description("Thu hồi đã phê duyệt")]
            RecallAfterApproved = 128,

            [Description("Xác nhận đóng dấu")]
            ConfirmMarked = 256,

            [Description("Chia sẻ")]
            Share = 2048,
        }
    }

    public static class VanBanDenAction
    {
        public enum Action ///Hành động văn bản đến
        {
            [Description("Ưu tiên")]
            Priority = 1,

            [Description("Thu hồi hoàn tất/ Thu hồi")]
            Recall = 2,

            [Description("Cho ý kiến")]
            Comment = 4,

            [Description("Chuyển xử lý/Chuyển văn thư")]
            Forward = 8,

            [Description("Phân công lại")]
            ReAssignment = 16,

            [Description("Phân công")]
            Assignment = 32,

            [Description("Trình lãnh đạo/Chuyển xử lý (Ban giám đốc)")]
            SubmitBOD = 64,

            [Description("Bổ sung/thu hồi")]
            RecallOrForward = 128,

            [Description("Kết thúc")]
            Completed = 256,

            [Description("Ban giám đốc cho ý kiến")]
            CommentBOD = 512,

            [Description("Chuyển văn thư")]
            FowardArchives = 1024,

            [Description("Thu hồi trình lãnh đạo/Thu hồi")]
            RecallBOD = 2048,

            [Description("Sửa thông tin văn bản")]
            EditVB = 8192,
        }
    }

    public static class VBBHAction
    {
        public enum Action
        {
            [Description("Phân công")]
            Assignment = 1,
            [Description("Phân công lại")]
            ReAssignment = 2,
            [Description("Sửa thông tin văn bản")]
            EditVB = 32,
            [Description("Bổ sung/thu hồi")]
            RecallOrForward = 64,
        }
    }

    public static class TaskVanBanDenAction
    {
        public enum Action ///Hành động task văn bản đến
        {
            [Description("Lưu")]
            Save = 1,
            [Description("Phân công")]
            Assignment = 2,
            [Description("Lập hồ sơ xử lý")]
            CreateRecord = 4,
            [Description("Lập hồ sơ dự thảo")]
            CreateRecordDraft = 8,
            [Description("Trao đổi lại")]
            Feedback = 16,
            [Description("Hoàn tất")]
            Completed = 32,
            [Description("Thực hiện lại")]
            Again = 64,
            [Description("Đánh giá")]
            Evalute = 128
        }

        [Description("Role Task Văn bản đến được sửa hoặc không")]
        public enum Role
        {
            [Description("Người nhận Task")]
            AssignedTo = 64,
            [Description("Người giao Task")]
            AssignorPBan = 320,
            Assignor = 256,
            [Description("Người xem")]
            Viewer = 0
        }

        public enum FormHoSoTaiLieu
        {
            [Description("Văn bản(VBDi ko ký số/có ký số")]
            VanBan = 1,
            [Description("Hợp đồng")]
            HopDong = 2,
            [Description("Biên bản thanh toán")]
            BienBanThanhToan = 3,
            [Description("Báo cáo nội bộ/Quyết định")]
            BaoCaoNoiBo = 4,
            [Description("Biên bản nội bộ")]
            BienBanNoiBo = 5,
            [Description("Hồ sơ tài liệu khác")]
            HoSoTaiLieuKhac = 6,
            [Description("Tờ trình nội bộ")]
            ToTrinhNoiBo = 7,
            [Description("Hồ sơ lưu trữ")]
            HoSoLuuTru = 8,
        }
    }
}
