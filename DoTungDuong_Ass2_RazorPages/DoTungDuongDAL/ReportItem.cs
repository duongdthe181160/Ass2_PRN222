using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTungDuongDAL
{
    public class ReportDTO
    {
        public string CategoryName { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int Total { get; set; }
    }
}
