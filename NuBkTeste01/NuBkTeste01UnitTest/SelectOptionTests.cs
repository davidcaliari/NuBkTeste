using System;
using Xunit;

namespace NuBkTeste01UnitTest
{
    public class SelectOptionTests
    {
        [Fact]
        public void ValidCreatAccountOption()
        {
            Assert.Empty(NuBkTeste01.Program.ValidateOperation("1"));
        }
    }
}
