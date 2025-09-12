using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Models
{
    public class Messages
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? MessageType { get; set; }
        public string? TargetURL { get; set; }
        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;
    }
}
