using System;
using Xunit;
using NuBkTeste01;
using NuBkTeste01.Data.VO;

namespace NuBkTeste01UnitTest
{
    public class SelectOptionTests
    {
        [Fact]
        public void ValidCreatAccountOption()
        {

            Assert.Empty(Program.ValidateOperation("1"));
        }
        [Fact]
        public void ValidTransactionAutorizationOption()
        {
            Assert.Empty(Program.ValidateOperation("2"));
        }

        [Fact]
        public void InvalidCreatAccountOption()
        {
            Assert.NotEmpty(Program.ValidateOperation("g"));
        }
        [Fact]
        public void InvalidTransactionAutorizationOption()
        {
            Assert.NotEmpty(Program.ValidateOperation("0"));
        }

        [Fact]
        public void InvalidDuplicatedtAccount()
        {
            Program.accounts = new AccountsVO()
            {
                accounts = new AccountVO()
                {
                    activeCard = true,
                    availableLimit = 75
                }
            };
            Assert.NotEmpty(Program.ValidateOperation("1"));
        }
    }
}
