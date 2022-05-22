using NuBkTeste01.Data.VO;
using System.Collections.Generic;

namespace NuBkTeste01.Business
{
    public interface IAccountBusiness
    {
        AccountsVO CreateAccount(AccountsVO accounts, AccountsVO internalAccount);
    }
}