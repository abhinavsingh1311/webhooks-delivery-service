using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks_System_Library.Entities
{
    public enum WebHookStatus : byte
    { 
      Pending = 0,
      Delivered = 1,
      Failed = 2,
      DeadLetter = 3
    }
}

