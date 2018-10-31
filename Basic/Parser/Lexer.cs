using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Basic.Infrastructure;

namespace Basic.Parser
{
    /// <summary>
    /// Lexican analyzer
    /// </summary>
    public class Lexer
    {
        private static Dictionary<char, TokenType> _singleCharTokens = new Dictionary<char, TokenType>()
        {
            {':', TokenType.StatementSep},
            {'(', TokenType.ParenOpen},
            {')', TokenType.ParenClose},
            {',', TokenType.Comma},
            {'+', TokenType.Plus},
            {'/', TokenType.Div},
            {'-', TokenType.Minus},
            {'*', TokenType.Mul},
            {'^', TokenType.Exponent},
            {'=', TokenType.EQ},
            {'\n',TokenType.Eoln},
            {';', TokenType.PrintItemSep}
        };

        /// <summary>
        /// List of known keywords. NOT case-sensitive.
        /// Does not include functions and statements. See BasicStatementAttribute and BasicFunctionAttribute
        /// </summary>
        private static Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"and",     TokenType.And},
            {"or",      TokenType.Or}
        };


        private TextReader _input;
        private StringBuilder _sb;
        private int _nrCharsRead;

        /// <summary>
        /// Current column
        /// </summary>
        internal int Column { get { return _nrCharsRead; }}

        /// <summary>
        /// Current token
        /// </summary>
        internal Token Current { get; set; } = Token.Eof;

        /// <summary>
        /// ctor
        /// </summary>
        internal Lexer(TextReader input)
        {
            _input = input;
            _sb = new StringBuilder();
            _nrCharsRead = 0;
        }

        /// <summary>
        ///Used when testing the lexer directly
        /// </summary>
        public static List<Token> Tokenize(string input)
        {
            var allTokens = new List<Token>();
            var lex = new Lexer(new StringReader(input));

            while (lex.MoveNext())
            {
                allTokens.Add(lex.Current);
            }

            return allTokens;
        }

        /// <summary>
        /// Make next token the current token, if still available
        /// </summary>
        public bool MoveNext()
        {
            Current = Token.Eof;
            _sb.Clear();

            char ch;

            // skip leading whitespace
            do
            {
                if (_input.Peek() < 0) return false;
                ch = ReadChar();
            } while (Char.IsWhiteSpace(ch) && ch != '\n');

            // '?' is shorthand for 'PRINT'
            if (ch == '?')
            {
                Current = new Token() { Text = "PRINT", TokenType = TokenType.Identifier };
                return true;
            
            }

            // single-char tokens
            if (_singleCharTokens.TryGetValue(ch, out TokenType singleToken))
            {
                Current = new Token() { Text = string.Empty, TokenType = singleToken };
                return true;
            }

            // multi-char tokens such as '<='
            char nextCh = (char)_input.Peek();
            switch(ch)
            {
                case '<':
                    if (nextCh == '>')
                    {
                        ReadChar();
                        return SetToken(TokenType.NotEQ, "<>");
                    }
                    if (nextCh == '=')
                    {
                        ReadChar();
                        return SetToken(TokenType.LessEQ, "<=");
                    }
                    return SetToken(TokenType.Less, "<");
                case '>':
                    if (nextCh == '=')
                    {
                        ReadChar();
                        return SetToken(TokenType.GreatEQ, ">=");
                    }
                    return SetToken(TokenType.Great, ">");

                case '"':
                    ReadStringLiteral();
                    return true;

                case '.':
                case char digit when Char.IsDigit(ch):
                    ReadNumber(ch);
                    return true;

                case char letter when Char.IsLetter(ch):
                    ReadIdentifier(ch);
                    return true;

                default:
                    return SetToken(TokenType.Error, new string(ch, 1));
            }
        }

        /// <summary>
        /// Skips the line.
        /// </summary>
        internal string SkipLine()
        {
            _sb.Clear();
            while (_input.Peek() >= 0)
            {
                char ch = ReadChar();
                if (ch == '\n')
                {                   
                    break;
                }
                else
                {
                    _sb.Append(ch);
                }
            }

            Current = Token.Eoln;
            return _sb.ToString();
        }

        /// <summary>
        /// Peek at next character, skipping spaces
        /// </summary>
        public char Peek()
        {
            while (_input.Peek() >= 0)
            {
                char ch = (char) _input.Peek();
                if (char.IsWhiteSpace(ch) && ch != '\n')
                {
                    _input.Read();
                }
                else
                {
                    return ch;
                }
            }

            return (char)0;
        }

        /// <summary>
        /// String literal
        /// </summary>
        private void ReadStringLiteral()
        {
            bool foundTerminator = false;

            while (_input.Peek() >= 0)
            {
                char nextCh = ReadChar();
                if (nextCh != '"')
                {
                    _sb.Append(nextCh);
                }
                else
                {
                    foundTerminator = true;
                    break;
                }
            }

            if (!foundTerminator)
            {
                throw new BasicSyntaxException("Unterminated string");
            }

            SetToken(TokenType.String, _sb.ToString());
        }

        /// <summary>
        /// Reads a floating-point number
        ///     state   description
        ///     0       integral number
        ///     1       '.' read
        ///     2       fractional number
        ///     3       'e'
        ///     4       '-' after 'e'
        ///     5       mantissa
        /// </summary>
        private void ReadNumber(char initialChar)
        {
            int state = (initialChar == '.') ? 1 : 0;

            if (state == 1) _sb.Append('0');
            _sb.Append(initialChar);

            while (true)
            {
                char ch = (char)_input.Peek();
                switch(state)
                {
                    case 0:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                        }
                        else if (ch == '.')
                        {
                            _sb.Append(ReadChar());
                            state = 1;
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            _sb.Append(ReadChar());
                            state = 3;
                        }
                        else
                        {
                           state = 99;
                        }
                        break;

                    case 1:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                            state = 2;
                        }
                        else
                        {
                            throw new BasicSyntaxException("Digit expected after '.'");
                        }
                        break;

                    case 2:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            _sb.Append(ReadChar());
                            state = 3;
                        }
                        else
                        {
                            state = 99;
                        }
                        break;

                    case 3:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                            state = 5;
                        }
                        else if (ch == '-')
                        {
                            _sb.Append(ReadChar());
                            state = 4;
                        }
                        else
                        {
                            throw new BasicSyntaxException("Digit expected after 'e'");
                        }
                        break;

                    case 4:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                            state = 5;
                        }
                        else
                        {
                            throw new BasicSyntaxException("Digit expected after 'e-'");
                        }
                        break;

                    case 5:
                        if (char.IsDigit(ch))
                        {
                            _sb.Append(ReadChar());
                            state = 5;
                        }
                        else
                        {
                            state = 99;
                        }
                        break;
                    default:
                        throw new Exception($"Logic failure in ReadNumber - state={state}");
                }

                if (state == 99) break;
            }

            SetToken(TokenType.Number, _sb.ToString());
        }

        /// <summary>
        /// reads identifier or keyword
        /// </summary>
        private void ReadIdentifier(char initialChar)
        {
            _sb.Append(initialChar);
 
            while (true)
            {
                char nextCh = (char)_input.Peek();
                if (Char.IsLetter(nextCh) || char.IsDigit(nextCh))
                {
                    nextCh = ReadChar();
                    _sb.Append(nextCh);
                }
                else
                {
                    break;
                }
            }

            // may end on a '$'
            if ((char)_input.Peek() == '$')
            {
                _sb.Append(ReadChar());               
            }

            var id = _sb.ToString().ToUpperInvariant();
            if (_keywords.TryGetValue(id, out TokenType tokType))
            {
                SetToken(tokType, id);
            }
            else
            {
                SetToken(TokenType.Identifier, id);
            }
        }

        /// <summary>
        /// Saves the token found.
        /// </summary>
        private bool SetToken(TokenType newType, string text)
        {
            Current = new Token() { TokenType = newType, Text = text };
            return true;
        }

        private char ReadChar()
        {
            char ch = (char)_input.Read();
            _nrCharsRead = (ch == '\n') ? 0 : _nrCharsRead + 1;
            return ch;
        }
    }
}
