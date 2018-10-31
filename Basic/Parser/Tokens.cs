using System;
namespace Basic.Parser
{
    public enum TokenType
    {
        Error,

        Number,         //  digit+
        String,         //  '"' [any char not(" \n)]*  '"'
        Identifier,     //  letter [letter | digit]*

        Plus,           //  '+'
        Minus,          //  '-'
        Mul,            //  '*'
        Div,            //  '/'
        Exponent,

        Less,           //  '<'
        LessEQ,         //  '<='
        EQ,             //  '='
        NotEQ,          //  '<>'
        Great,          //  '>'
        GreatEQ,        //  '>='

        ParenOpen,      //  '('
        ParenClose,     //  ')'
        Comma,          //  ','
        StatementSep,   //  ':'
        PrintItemSep,   //  ';'     seperates items in a print-list

        // boolean operators
        And,
        Or,

        Eoln,
        Eof
    }

    public static class TokenExt
    {
        public static bool IsExponent(this TokenType tokenType)
        {
            return  (tokenType == TokenType.Exponent);
        }

        public static bool IsMultiplication(this TokenType tokenType)
        {
            return  (tokenType == TokenType.Mul) ||
                    (tokenType == TokenType.Div);
        }
 
        public static bool IsAddition(this TokenType tokenType)
        {
            return (tokenType == TokenType.Plus) ||
                    (tokenType == TokenType.Minus);
        }
 
        public static bool IsRelational(this TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Less:
                case TokenType.LessEQ:
                case TokenType.Great:
                case TokenType.GreatEQ:
                case TokenType.EQ:
                case TokenType.NotEQ:
                    return true;

                default:
                    return false;
            }
        }
    }

    public class Token
    {
        public static Token Eof = new Token()
        {
            TokenType = TokenType.Eof
        };
        public static Token Eoln = new Token()
        {
            TokenType = TokenType.Eoln
        };

        public TokenType TokenType { get; set; }
        public string Text { get; set; }
    }
}
