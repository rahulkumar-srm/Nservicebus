﻿using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Messages.Commands
{
    public class TestCmd
    {
        public string TestString { get; set; }
    }
}
