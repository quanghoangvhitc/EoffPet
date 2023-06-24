using System;
using System.Collections.Generic;
using System.Text;

namespace Petrolimex.Bean
{
    public class BeanYKien : BeanBase
    {
        public BeanYKien() { }
        public BeanYKien(string title, string value)
        {
            Title = title;
            Value = value;
        }
        public string Title { get; set; }
        public string Value { get; set; }
        public DateTime Created { get; set; }
    }
}
