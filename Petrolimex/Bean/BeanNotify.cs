using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanNotify : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public DateTime? Created { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public float? DocumentID { get; set; }
        public float? TaskID { get; set; }
        public string Category { get; set; }
        public string CategoryText { get; set; }
        /// <summary>
        /// 0: Nhận để biết, 1: cần xử lý
        /// </summary>
        public bool Type { get; set; }
        public string SendUnit { get; set; }
        /// <summary>
        /// 1: cao, 2: thuong, 3: thap
        /// </summary>
        public int? Priority { get; set; }
        public string EmailUpdate { get; set; }
        public string AssignedBy { get; set; }
        /// <summary>
        /// 0: chưa hoàn tất, 1: hoàn tất
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// remove: loại bỏ
        /// </summary>
        public string Action { get; set; }
        public string BanLanhDao { get; set; }
        public DateTime? DueDate { get; set; }
        public string Content { get; set; }
        public float Percent { get; set; }
        public string Note { get; set; }
        public DateTime? Modified { get; set; }
        public string ItemUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Notified { get; set; }
        public bool Reminder { get; set; }
        public bool? Read { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Email { get; set; }
        public string Group { get; set; }
        public int? Rate { get; set; }
        public string SiteName { get; set; }
        public string ListName { get; set; }
        public bool? HasFile { get; set; }
        public int? Step { get; set; }
        public string TaskCategory { get; set; }
        public string TaskSubCategory { get; set; }
        public int? SubCateID { get; set; }
        public float? Parent { get; set; }
        public string TitleEN { get; set; }
        public float? OverDueNum { get; set; }
        public string SubmitAction { get; set; }
        public bool Automatic { get; set; }
        public string TaskType { get; set; }
        /// <summary>
        /// Dùng để phân biệt loại văn bản - 3: Văn bản đến; 8: Văn bản phát hành ; 7: Văn bản dự thảo
        /// </summary>
        public int ModuleId { get; set; }
        [RemoveAccentFromMultiColumn(new string[] { "TaskCategory", "Content" })]
        public string SearchData { get; set; }
        /// <summary>
        /// Value 0: link vào chi tiết van ban, 1: link vào chi tiết task của văn bản
        /// </summary>
        public int FormType { get; set; }
        /// <summary>
        /// Dùng để kiểm tra object có được focus hay không
        /// </summary>
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public int GroupCreated
        {
            get
            {
                if (Created.HasValue)
                {
                    if (WeekOfYear(DateTime.Now.Date) - WeekOfYear(Created.Value.Date) == 0)
                    {
                        if (Created.Value.Date == DateTime.Now.Date)
                            return 0;
                        if (Created.Value.Date == DateTime.Now.Date.AddDays(-1))
                            return 1;
                    }
                    //else if (WeekOfYear(DateTime.Now.Date) - WeekOfYear(Created.Value.Date) == 1)
                    //    return 7;
                    //else if (WeekOfYear(DateTime.Now.Date) - WeekOfYear(Created.Value.Date) == 2)
                    //    return 14;
                }

                return 99;
            }
        }

        int WeekOfYear(DateTime time)
        {
            var day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public string ImagePath { get; set; }

        public override string GetServerUrl()
        {
            return "<#SiteName#>/_layouts/15/VuThao.Petrolimex.API/ApiPublish.ashx?func=getMyNotify";
        }
        public override string GetApiUrlExec()
        {
            return "<#SiteName#>/vanban/_layouts/15/VuThao.Petrolimex.API/ApiPublish.ashx";
        }
    }
}
