﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.DTO
{
    public class SendMessageDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }

}
