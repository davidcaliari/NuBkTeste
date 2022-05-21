using System;
using System.Collections.Generic;
using System.Text;

namespace NuBkTeste01.Data.VO
{
    public class AccountsVO
    {
        public AccountVO account { get; set; }// = new AccountVO();
        public List<string> violations { get; set; } = new List<string>();

    }
}
