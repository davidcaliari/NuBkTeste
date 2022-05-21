using NuBkTeste01.Data.VO;
using System;
using System.Collections.Generic;
using System.Text;
using NuBkTeste01;

namespace NuBkTeste01.Business.Implementation
{
    public class TransactionBusinessImplementation : IAccountBusiness
    {
        public AccountsVO CreateAccount(string creditValue)
        {
            var account = new AccountVO();
            account.activeCard = true;
            account.availableLimit = Convert.ToInt32(creditValue);

            var accounts = new AccountsVO() { account = account };
            return accounts;
        }
    }
}
