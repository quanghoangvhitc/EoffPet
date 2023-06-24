using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
namespace Petrolimex.Bean
{
    public class BeanSearchProperty
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string TrichYeu { get; set; }
        public DateTime? NgayDen { get; set; }
        public string SoDen { get; set; }
        public DateTime? NgayTrenVB { get; set; }
        public string CoQuanGui { get; set; }
        public int ModuleId { get; set; }
        public DateTime? Created { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string Content { get; set; }
        public string LoaiVanBan { get; set; }
        public string NguoiKyVanBan { get; set; }
        public string Type { get; set; }
        public string ModifiedBy { get; set; }
    }
}
