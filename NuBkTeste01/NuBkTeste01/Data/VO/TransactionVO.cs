using System;
using System.Collections.Generic;

namespace NuBkTeste01.Data.VO
{
    public class TransactionVO
    {
        public string merchant { get; set; }
        public int amount { get; set; }
        public DateTimeOffset time { get; set; }
        
    }
}
