using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basic_Test
{
    [TestClass]
    public class UnitTest_Lex
    {
        [TestMethod]
        public void Lexer_SingleChars()
        {
            var expected = new List<TokenType>() {
                TokenType.ParenOpen,    TokenType.Comma,        TokenType.ParenClose,
                TokenType.Plus,         TokenType.StatementSep, TokenType.Mul,
                TokenType.Minus,        TokenType.Div
            };

            var lx = Lexer.Tokenize("  ( ,)+: * -   /");
            var allTokens = lx.Select(tok => tok.TokenType).ToList();

            Assert.IsTrue(expected.SequenceEqual(allTokens));
        }

        [TestMethod]
        public void Lexer_Relop()
        {
            var expected = new List<TokenType>() {
                TokenType.Less,         TokenType.Great,
                TokenType.LessEQ,       TokenType.GreatEQ,
                TokenType.EQ,           TokenType.NotEQ
            };

            var lx = Lexer.Tokenize("  <  >  <=   >=  =  <>   ");
            var allTokens = lx.Select(tok => tok.TokenType).ToList();

            Assert.IsTrue(expected.SequenceEqual(allTokens));
        }

        [TestMethod]
        public void Lexer_Numbers()
        {
            TestNumber("101    1 99   767 1234 98765");
            TestNumber("9");
            TestNumber("1 22  333      4444  55555    ");
            TestNumber("4.93174E-04 3.141  .3143e1");
        }

        /// <summary>
        /// Test numbers: results using known-correct method and own parser must match
        /// </summary>
        private void TestNumber(string input)
        {
            var expectedNumbers = input.Split(new char[] { ' ' })
                                       .Where(txt => !string.IsNullOrEmpty(txt))
                                       .Select(txt => Double.Parse(txt))
                                       .ToList();

            var lx = Lexer.Tokenize(input);
            var allTokens = lx.ToList();

            Assert.IsTrue(allTokens.All(tk => tk.TokenType == TokenType.Number));

            var actualNumbers = allTokens.Select(tk => Double.Parse(tk.Text))
                                         .ToList();
            Assert.IsTrue(expectedNumbers.SequenceEqual(actualNumbers));
        }

        [TestMethod]
        public void Lexer_Strings()
        {
            TestString("\"101\"  |  \"1 \" | \"abcd\" ");
        }

        /// <summary>
        /// Test strings: results using known-correct method and own parser must match
        /// </summary>
        private void TestString(string input)
        {
            var expectedStrings = input.Split(new char[] { '|' })
                                       .Select(txt => txt.Trim())
                                       .Where(txt => !string.IsNullOrEmpty(txt))
                                       .Select(txt => StripDoubleQuotes(txt))
                                       .ToList();

            var cleanedInput = input.Replace("|", string.Empty);
            var lx = Lexer.Tokenize(cleanedInput);
            var allTokens = lx.ToList();

            Assert.IsTrue(allTokens.All(tk => tk.TokenType == TokenType.String));

            var actualStrings = allTokens.Select(tk => tk.Text)
                                         .ToList();
            Assert.IsTrue(expectedStrings.SequenceEqual(actualStrings));
        }

        private static string StripDoubleQuotes(string txt)
        {
            if (txt.Length < 2) throw new Exception($"Bad string '{txt}'");
            if (txt[0] != '"') throw new Exception($"String '{txt}' must start with dbl-quote");
            if (txt[txt.Length - 1] != '"') throw new Exception($"String '{txt}' must start with dbl-quote");

            return txt.Substring(1, txt.Length - 2);
        }

        [TestMethod]
        public void Lexer_Identifiers()
        {
            TestIdentifiers("a abc abcde i");
            TestIdentifiers("    a    abc     abcde     i    ");
            TestIdentifiers("a");
        }

        /// <summary>
        /// Verify Identifiers
        /// </summary>
        private void TestIdentifiers(string input)
        {
            var expectedIDs = input.Split(new char[] { ' ' })
                                   .Where(txt => !string.IsNullOrEmpty(txt))
                                   .Select(txt => txt.ToUpperInvariant())
                                   .ToList();

            var lx = Lexer.Tokenize(input);
            var allTokens = lx.ToList();

            Assert.IsTrue(allTokens.All(tk => tk.TokenType == TokenType.Identifier));

            var actualIDs = allTokens.Select(tk => tk.Text)
                                     .ToList();
            Assert.IsTrue(expectedIDs.SequenceEqual(actualIDs));
        }
   }
}
