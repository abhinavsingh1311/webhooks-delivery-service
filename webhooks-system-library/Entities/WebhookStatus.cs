using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks_System_Library.Entities
{
    public enum WebHookStatus :byte
    {
        Delivered =1,
        DeadLetter=3,
        Pending=0,
        Failed=2
    }
}
