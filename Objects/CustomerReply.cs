﻿using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public class CustomerReply
    {
            public string Message { get; set; }
            public OrderReplyMessage ReplyMessage { get; set; }        
    }
}
