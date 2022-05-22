using NuBkTeste01.Data.VO;
using System.Collections.Generic;
using System.Text.Json;

namespace NuBkTeste01.Business.Implementation
{
    public class AccountBusinessImplementation : IAccountBusiness
    {
        public AccountsVO CreateAccount(AccountsVO accounts, AccountsVO internalAccount)
        {

            if (accounts.account == null && internalAccount.account.availableLimit > 0)
            {
                accounts.account = internalAccount.account;
                return DuplicateObject(accounts);
            }
            else if (accounts.account == null && internalAccount.account.availableLimit < 0)
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("insufficient-limit-to-create-account");
                return newAccount;
            }
            else
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("account-already-initialized");
                return newAccount;
            }

        }

        public AccountsVO MovimentAccount(AccountsVO accounts, TransactionsVO internalTransaction)
        {
            if (accounts.account.availableLimit >= internalTransaction.transaction.amount)
            {
                accounts.account.availableLimit = accounts.account.availableLimit - internalTransaction.transaction.amount;
                return DuplicateObject(accounts);
            }
            else
            {
                var newAccount = DuplicateObject(accounts);
                newAccount.violations.Add("insufficient-limit");
                return DuplicateObject(newAccount);
            }
        }

        private static AccountsVO DuplicateObject(AccountsVO input)
        {
            var json = JsonSerializer.Serialize(input);
            var accounts = JsonSerializer.Deserialize<AccountsVO>(json);
            return accounts;
        }
    }
}
