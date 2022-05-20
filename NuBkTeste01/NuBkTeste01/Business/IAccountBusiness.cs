using NuBkTeste01.Data.VO;

namespace NuBkTeste01.Business
{
    public interface IAccountBusiness
    {
        AccountsVO CreateAccount(string creditValue);
    }
}