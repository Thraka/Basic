using System;
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
    /// The parser itself
    /// </summary>
    public class StatementParser
    {
        private static Dictionary<string, StatementTypeInfo> _statements;

        /// <summary>
        /// static-ctor: runs once 
        /// </summary>
        static StatementParser()
        {
            _statements = TypeFinder.FindStatements().ToDictionary(stat => stat.ID, stat => stat, StringComparer.InvariantCultureIgnoreCase);
        }

        private PartsParser _partsParser;
        private int? _currentLineNumber;

        /// <summary>
        /// Current line, only valid when IsProgramLine == true
        /// </summary>
        public int LineNumber { get { return _currentLineNumber.Value; }}

        /// <summary>
        /// True: this is a program line, add to program
        /// false: immediate statements
        /// </summary>
        public bool IsProgramLine { get { return _currentLineNumber.HasValue; }}

        /// <summary>
        /// Private ctor
        /// </summary>
        private StatementParser(PartsParser pparser)
        {
            _partsParser = pparser;
        }

        /// <summary>
        /// ctor from string
        /// </summary>
        public StatementParser(string input)
            : this(new PartsParser(new StringReader(input)))
        {
        }

        public StatementParser(TextReader input)
            : this(new PartsParser(input))
        {
        }

        /// <summary>
        /// Parses one line of statements
        /// </summary>
        public StatementList ParseLine()
        {
            var statements = new StatementList();

            // optional line-number
            _currentLineNumber = null;
            if (_partsParser.TryGetLinenumber(out int lineNumber))
            {
                _currentLineNumber = lineNumber;
            }

            if (_partsParser.Eoln) return statements;
            
            try
            {
                while (TryParseStatement(out IStatement newStatement))
                {
                    statements.Add(newStatement);

                    // should be eof or end-of-statement
                    if (_partsParser.Eof) break;
                    if (!_partsParser.EndOfStatement)
                    {
                        throw new BasicSyntaxException($"Did not expect '{_partsParser.TokenText}'");
                    }

                    bool isEoln = _partsParser.Eoln;
                    _partsParser.NextToken();

                    if (isEoln) break;
                }
            }
            catch (Exception ex)
            {
                // skip rest of current lione and set-up for next line
                _partsParser.SkipLine();
                _partsParser.NextToken();

                throw new BasicSyntaxException(ex.Message)
                {
                    Location = IsProgramLine ? $"({LineNumber})" : string.Empty
                };
            }

            return statements;
        }

        /// <summary>
        /// Parse a single statement
        /// </summary>
        private bool TryParseStatement(out IStatement newStatement)
        {
            newStatement = null;
            if (_partsParser.Eof) return false;

            // statement must start with statement-ID or identifier (for assignment)
            if (!_partsParser.TokenIsOfType(TokenType.Identifier))
            {
                throw new BasicSyntaxException($"Unknown statement {_partsParser.TokenText}");
            }

            if (!TryGetStatement(_partsParser.TokenText, out newStatement))
            {
                if (!TryAssignment(out newStatement))
                {
                    throw new BasicSyntaxException($"Unknown statement {_partsParser.TokenText}");
                }
            }

            // make sure input is ready for parser in statements.
            // Exception: REM wants whole input, unmodified
            //TODO: find a cleaner way to handle thia
            if (!typeof(RemStatement).IsAssignableFrom(newStatement.GetType()))
            {   
                _partsParser.NextToken();
            }

            newStatement.Parse(_partsParser);
             
            return true;
        }

        /// <summary>
        /// Statement does not start with known keyword. Might be assignment to identifier
        /// </summary>
        private bool TryAssignment(out IStatement newStatement)
        {
            newStatement = null;

            if (_partsParser.TokenIsOfType(TokenType.Identifier))
            {
                var varName = _partsParser.TokenText;

                _partsParser.NextToken();
                if (_partsParser.TokenIsOfType(TokenType.EQ))
                {
                    newStatement = new AssignmentStatement(varName);
                    return true;
                }

                throw new BasicSyntaxException($"Unknown statement {varName}");
            }

            return false;
        }

        /// <summary>
        /// Have the keyword, get correct statement parser
        /// </summary>
        private bool TryGetStatement(string tokenText, out IStatement stat)
        {
            stat = null;

            if (!_statements.TryGetValue(tokenText, out StatementTypeInfo info))
            {
                return false;
            }

            stat = info.CreateInstance();
            return true;
        }
    }
}
