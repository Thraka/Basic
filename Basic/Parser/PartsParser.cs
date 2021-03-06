﻿using System;
using Basic.Statements;
using Basic.Expressions;
using System.Collections.Generic;
using Basic.Infrastructure;
using System.Linq;
using Basic.Functions;
using System.IO;

namespace Basic.Parser
{
    /// <summary>
    /// Parses single parts for a complete statement. Example: the expression in 'A=(1+2)'
    /// </summary>
    public class PartsParser
    {
        private static Dictionary<string, FunctionTypeInfo> _functions;

        /// <summary>
        /// static-ctor: runs once 
        /// </summary>
        static PartsParser()
        {
            _functions = TypeFinder.FindFunctions().ToDictionary(stat => stat.ID, stat => stat, StringComparer.InvariantCultureIgnoreCase);
        }

        private Lexer _lex;

        private TokenType TokenType
        {
            get { return _lex.Current.TokenType; }
        }

        internal string TokenText
        {
            get { return _lex.Current.Text; }
        }

        internal int Column
        {
            get { return _lex.Column; }
        }

        /// <summary>
        /// at end-of-file / end-of-line
        /// </summary>
        internal bool Eof  { get { return TokenType == TokenType.Eof;  } }
        internal bool Eoln { get { return TokenType == TokenType.Eoln; } }

        /// <summary>
        /// True: currently at end-of-statement
        /// </summary>
        internal bool EndOfStatement
        {
            get
            {
                return  (TokenType == TokenType.Eof) ||
                        (TokenType == TokenType.Eoln) ||
                        (TokenType == TokenType.StatementSep);
            }   
        }

 
        /// <summary>
        /// Private ctor
        /// </summary>
        internal PartsParser(TextReader inStream)
        {
            _lex = new Lexer(inStream);
            _lex.MoveNext();
        }

        /// <summary>
        /// Create parts parser from a string. Mainly for unit-tests
        /// </summary>
        public static Expression ParseStringAsExpression(string input)
        {
            var p = new PartsParser(new StringReader(input));
            return p.ReadExpression();
        }

        // --- Parse helpers for Statements

        /// <summary>
        /// Current line might stat with a linenumber. Get it when available.
        /// </summary>
        internal bool TryGetLinenumber(out int lineNumber)
        {
            if (TokenType == TokenType.Number)
            {
                lineNumber = Int32.Parse(TokenText);
                _lex.MoveNext();
                return true;
            }

            lineNumber = 0;
            return false;
        }

        /// <summary>
        /// Either 'REM' statement of error detected in current line: prepare for parse of next line
        /// by skipping everything until eoln.
        /// </summary>
        internal string SkipLine()
        {
            return _lex.SkipLine();
        }

        /// <summary>
        /// Reads the next token
        /// </summary>
        internal void NextToken()
        {
            _lex.MoveNext();
        }

        internal bool TokenIsOfType(TokenType expectedType)
        {
            return TokenType == expectedType;
        }

        /// <summary>
        /// Current token must be an identifier, read it
        /// </summary>
        internal string ReadIdentifier()
        {
            if (TokenType != TokenType.Identifier)
            {
                throw new BasicSyntaxException("Identifier expected");
            }

            var identifier = _lex.Current.Text;
            _lex.MoveNext();

            return identifier;
        }

        /// <summary>
        /// Current token must be an string literal, read it
        /// </summary>
        internal string ReadStringLiteral()
        {
            if (TokenType != TokenType.String)
            {
                throw new BasicSyntaxException("String literal expected");
            }

            var literal = _lex.Current.Text;
            _lex.MoveNext();

            return literal;
        }

        /// <summary>
        /// expect a context-specific keyword. example: THEN after IF
        /// </summary>
        internal void ReadContextualKeyword(string keyword)
        {
            var id = ReadIdentifier().ToUpperInvariant();
            if (!id.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BasicSyntaxException($"'{keyword}' expected");
            }
        }

        /// <summary>
        /// Current token must be an integer
        /// </summary>
        internal int ReadInteger()
        {
            if (TokenType != TokenType.Number || !Int32.TryParse(_lex.Current.Text, out int lineNumber))
            {
                throw new BasicSyntaxException("Number expected");
            }

            _lex.MoveNext();

            return lineNumber;
        }


        /// <summary>
        /// A specific token is expected
        /// </summary>
        internal Token ReadToken(TokenType expectedType, string customMsg = null)
        {
            if (TokenType != expectedType)
            {
                throw new BasicSyntaxException(customMsg ?? $"Token {expectedType} expected");
            }
        
            var savedToken = _lex.Current;
            _lex.MoveNext();
            return savedToken;
        }

        /// <summary>
        /// Parses formal parameter list:
        //  PARAMLIST := '(' PARAMS? ')'
        /// PARAMS := PARAM | PARAM ',' PARAMS
        /// </summary>
        internal List<string> ReadFormalParameterList()
        {
            var paramNames = new List<string>();

            ReadToken(TokenType.ParenOpen, "Expected '('");

            while (_lex.Current.TokenType == TokenType.Identifier)
            {
                paramNames.Add(_lex.Current.Text.ToUpperInvariant());

                _lex.MoveNext();
                if (_lex.Current.TokenType != TokenType.Comma) break;

                _lex.MoveNext();
            }

            if (_lex.Current.TokenType != TokenType.ParenClose)
            {
                throw new BasicSyntaxException("Expected ')'");
            }
            _lex.MoveNext();
            
            return paramNames;
        }

        internal Expression ReadAssignment()
        {
            ReadToken(TokenType.EQ);
            return ReadExpression();
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        internal Expression ReadExpression()
        {
            IExpressionNode expression = ParseTop();

            return new Expression(expression);
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        private IExpressionNode ParseTop()
        {
            return ParseAndOr();
        }

        /// <summary>
        /// AND / OR
        /// </summary>
        private IExpressionNode ParseAndOr()
        {
            var expr = ParseRelational();

            while (TokenType == TokenType.And || TokenType == TokenType.Or)
            {
                var savedToken = TokenType;
                _lex.MoveNext();
                var right = ParseRelational();

                expr = new ShortCircuitBinaryExpression(savedToken, expr, right);
            }

            return expr;
        }

        /// <summary>
        /// Relational operators
        /// </summary>
        private IExpressionNode ParseRelational()
        {
            var expr = ParseAddition();

            while (TokenType.IsRelational())
            {
                var savedToken = TokenType;
                _lex.MoveNext();
                var right = ParseAddition();

                expr = new MixedBinaryExpression(savedToken, expr, right);
            }

            return expr;
        }

        /// <summary>
        /// Addition / subtraction
        /// </summary>
        private IExpressionNode ParseAddition()
        {
            var expr = ParseMultiplication();

            while (TokenType.IsAddition())
            {
                var savedToken = TokenType;
                _lex.MoveNext();
                var right = ParseMultiplication();

                if (savedToken == TokenType.Plus)
                {
                    expr = new MixedBinaryExpression(savedToken, expr, right);
                }
                else
                {
                    expr = new NumericBinaryExpression(savedToken, expr, right);
                }
            }

            return expr;
        }

        /// <summary>
        /// Multiply / divide
        /// </summary>
        private IExpressionNode ParseMultiplication()
        {
            var expr = ParseExponent();

            while (TokenType.IsMultiplication())
            {
                var savedToken = TokenType;
                _lex.MoveNext();
                var right = ParseExponent();

                expr = new NumericBinaryExpression(savedToken, expr, right);
            }

            return expr;
        }

        /// <summary>
        /// exponentiation
        /// </summary>
        private IExpressionNode ParseExponent()
        {
            var expr = ParseFactor();
            if (!TokenType.IsExponent())
            {
                return expr;

            }

            // exponent has right-associativity
            Stack<IExpressionNode>  factors = new Stack<IExpressionNode>();
            factors.Push(expr);

            while (TokenType.IsExponent())
            {
                var savedToken = TokenType;
                _lex.MoveNext();
                factors.Push(ParseFactor());

             }

            while(factors.Count > 1)
            {
                var right = factors.Pop();
                var left = factors.Pop();
                factors.Push(new NumericBinaryExpression(TokenType.Exponent, left, right));
            }
    
            return factors.Pop();
        }

        /// <summary>
        /// Parse a factor: a single value, a function or an expression inside parenthesis
        /// </summary>
        /// <returns>The factor.</returns>
        private IExpressionNode ParseFactor()
        {
            IExpressionNode expr = null;
            switch (TokenType)
            {
                case TokenType.Minus:
                    _lex.MoveNext();
                    // no _lx.ReadToken() required: ParseTop() already does that.
                    return new NumericUnaryExpression(TokenType.Minus, ParseTop());
                case TokenType.Number:
                    expr = new LiteralNode(Value.CreateNumber(_lex.Current.Text));
                    break;
                case TokenType.String:
                    expr = new LiteralNode(Value.CreateString(_lex.Current.Text));
                    break;
                case TokenType.Identifier:
                    if (_functions.TryGetValue(_lex.Current.Text, out FunctionTypeInfo functionInfo))
                    {
                        // call to built-in function
                        expr = ParseFunction(functionInfo);
                    }
                    else if (IsUserFunctionName(_lex.Current.Text))
                    {
                        expr = ParseFunction(null);
                    }
                    else if (_lex.Peek() == '(')
                    {
                        // input is ID '(' : probably call to unknown function 'ID'
                        throw new BasicSyntaxException($"Unknown function {_lex.Current.Text}");
                    }
                    else
                    {
                        // variable reference
                        expr = new VariableNode(_lex.Current.Text);
                    }
                    break;

                case TokenType.ParenOpen:
                    _lex.MoveNext();
                    expr = ParseTop();
                    if (TokenType != TokenType.ParenClose)
                    {
                        throw new BasicSyntaxException("Expected closing parenthesis");
                    }
                    break;

                default:
                    throw new BasicSyntaxException($"Unexpected token '{_lex.Current.Text}' ({_lex.Current.TokenType})");
            }

            _lex.MoveNext();

            return expr;
        }

        /// <summary>
        /// identifier is call to a user-defined function "DEF FNX"; functionname is X
        /// </summary>
        internal string ReadUserFunctionName()
        {
            if (_lex.Current.TokenType != TokenType.Identifier || !IsUserFunctionName(_lex.Current.Text))
            {
                throw new BasicSyntaxException("User-function name expected");
            }

            var functionName = _lex.Current.Text.Substring(2, 1);
            _lex.MoveNext();

            return functionName;
        }

        private bool IsUserFunctionName(string identifier)
        {
            return !string.IsNullOrEmpty(identifier) &&
                    identifier.Length == 3 &&
                    identifier.StartsWith("FN", StringComparison.InvariantCultureIgnoreCase) &&
                    Char.IsLetter(identifier[2]);
        }

        /// <summary>
        /// Parse a function-coall: FUNCTION '(' parameter* ')'
        /// </summary>
        /// <returns>The function.</returns>
        private IExpressionNode ParseFunction(FunctionTypeInfo functionInfo)
        {
            var funName = _lex.Current.Text;

            _lex.MoveNext();
            ReadToken(TokenType.ParenOpen);

            var parameters = new List<IExpressionNode>();
            while (true)
            {
                if (TokenType == TokenType.ParenClose)
                {
                    break;
                }

                var param = ParseTop();
                parameters.Add(param);

                if (TokenType != TokenType.Comma)
                {
                    break;
                }

                _lex.MoveNext();
            }

            var nrParams = parameters.Count;
            if (nrParams < functionInfo.MinNrParameters)
            {
                throw new BasicSyntaxException($"Function '{functionInfo.ID} needs at least {functionInfo.MinNrParameters} parameters");
            }
            if (nrParams > functionInfo.MaxNrParameters)
            {
                throw new BasicSyntaxException($"Function '{functionInfo.ID} has too many parameters. Max={functionInfo.MaxNrParameters}");
            }

            return new FunctionExpression(functionInfo, parameters);
        }
    }
}
