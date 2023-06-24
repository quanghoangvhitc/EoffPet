using System;
namespace Petrolimex.Bean
{
    public class BeanAttachFile : BeanBase
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string OtherSite { get; set; }
        public bool IsRemove { get; set; }
        public string Author { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
        public DateTime? Created { get; set; }
        public long Size { set; get; }
    }
}
