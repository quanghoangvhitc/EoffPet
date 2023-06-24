using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanQuaTrinhLuanChuyenWorkflow : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Title { get; set; }
        public int DocumentID { get; set; }
        public int DepartmentId { get; set; }
        public int? TaskID { get; set; }
        public string Category { get; set; }
        public int LookupId { get; set; }
        public bool Type { get; set; }
        public string PersonEmail { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public string PersonAccount { get; set; }
        public string SendUnit { get; set; }
        public int Priority { get; set; }
        public string EmailUpdate { get; set; }
        public string AssignedBy { get; set; }
        public bool Status { get; set; }
        public string Action { get; set; }
        public DateTime? DueDate { get; set; }
        public string Content { get; set; }
        public string DepartmentUrl { get; set; }
        public int Percent { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }

        public int VBId { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Notified { get; set; }
        public bool Reminder { get; set; }
        public string Read { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Email { get; set; }
        public string Group { get; set; }
        public string GroupText { get; set; }
        public string Rate { get; set; }
        public string ListName { get; set; }
        public string HasFile { get; set; }
        public int Step { get; set; }
        public string TaskCategory { get; set; }
        public string SubmitAction { get; set; }
        public string TaskSubCategory { get; set; }
        public string SiteName { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToF { get; set; }
        public string DepartmentName { get; set; }
        public string ChucDanh { get; set; }
        public string TrangThai { get; set; }
    }
}
