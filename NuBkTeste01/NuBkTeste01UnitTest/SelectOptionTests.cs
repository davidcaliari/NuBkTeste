using System;
using Xunit;
using NuBkTeste01;
using NuBkTeste01.Data.VO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace NuBkTeste01UnitTest
{
    public class SelectOptionTests
    {
        [Fact]
        public void ValidCreatAccount()
        {
            Program.BeginConfig();

            List<string> jsonParam = new List<string> {
                @"{ ""account"": { ""activeCard"": true, ""availableLimit"": 100 } }"
            };
            AccountsVO account = JsonSerializer.Deserialize<AccountsVO>(jsonParam.First());
            Assert.Equal(JsonSerializer.Serialize(account), JsonSerializer.Serialize(Program.startViewJson(jsonParam, true)));
        }

        [Fact]
        public void ValidTransactionAutorization()
        {
            Program.BeginConfig();

            List<string> jsonParam = new List<string> { 
                @"{ ""account"": { ""activeCard"": true, ""availableLimit"": 100 } }",
                @"{ ""transaction"": { ""merchant"": ""Burger King"", ""amount"": 20, ""time"": ""2019-02-13T10:00:00"" } }"
            };
            Assert.Equal(80, Program.startViewJson(jsonParam, true).account.availableLimit);
        }

        [Fact]
        public void InvalidCreatAccount()
        {
            Program.BeginConfig();

            List<string> jsonParam = new List<string> {
                @"{ { ""activeCard"": true, ""availableLimit"": 100 } }"
            };
            Assert.Null(Program.startViewJson(jsonParam, true).account);
        }

        [Fact]
        public void InvalidTransactionAutorization()
        {
            Program.BeginConfig();

            List<string> jsonParam = new List<string> {
                @"{ ""account"": { ""activeCard"": true, ""availableLimit"": 100 } }",
                @"{ { ""merchant"": ""Burger King"", ""amount"": 20, ""time"": ""2019-02-13T10:00:00"" } }"
            };
            Assert.Equal(100, Program.startViewJson(jsonParam, true).account.availableLimit);
        }

        [Fact]
        public void InvalidAmoutTransactionAutorization()
        {
            Program.BeginConfig();

            List<string> jsonParam = new List<string> {
                 @"{ ""account"": { ""activeCard"": true, ""availableLimit"": 100 } }",
                 @"{ ""transaction"": { ""merchant"": ""Burger King"", ""amount"": 120, ""time"": ""2019-02-13T10:00:00"" } }"
            };
            Assert.Equal(100, Program.startViewJson(jsonParam, true).account.availableLimit);
        }
    }
}
