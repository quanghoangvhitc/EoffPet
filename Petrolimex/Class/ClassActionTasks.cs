using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Petrolimex.Class
{
    public class ClassActionTasks
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; }
        public int Index { get; set; }
        public string Color { get; set; }

        public ClassActionTasks()
        {
            Color = "0b61ae";
        }

        public CancellationToken CancelToken { get; set; }
    }
}
