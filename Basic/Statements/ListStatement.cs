using System;
using System.IO;
using System.Linq;
using Basic.Execute;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("LIST", "Show program")]
    public class ListStatement : IStatement
    {
        private int _startLine = 0;
        private int _endLine = int.MaxValue;

        /// <summary>
        /// Print lines
        /// </summary>
        public void Execute(ExecutionContext ctx)
        {
            var selectedLines = ctx.LineRange(_startLine, _endLine);
            foreach(var line in selectedLines)
            {
                ctx.Output.Write($"{line.Number,5} ");

                ListStatements(ctx, line.Statements);

                ctx.Output.WriteLine();
            }
        }

        /// <summary>
        /// List all statements in a single line
        /// </summary>
        private void ListStatements(ExecutionContext ctx, StatementList statements)
        {
            var lastStat = statements.LastOrDefault();
            foreach(var stat in statements)
            {
                stat.List(ctx.Output);

                if (stat != lastStat)
                {
                    ctx.Output.Write(":");
                }
            }
        }

        /// <summary>
        /// Allowed syntax:
        ///     LIST 10     : just line 10
        ///     LIST -10    : lines 0..10
        ///     LIST 10-90  : lines 10..90
        ///     LIST 90-
        /// </summary>
        /// <param name="p">P.</param>
        public void Parse(PartsParser p)
        {
            if (p.TokenIsOfType(TokenType.Number))
            {
                _startLine = p.ReadInteger();

                if (p.TokenIsOfType(TokenType.Minus))
                {
                    p.ReadToken(TokenType.Minus);

                    if (p.TokenIsOfType(TokenType.Number))
                    {
                        _endLine = p.ReadInteger();
                    }
                }
                else if (p.EndOfStatement)
                {
                    _endLine = _startLine;
                }
                else
                {
                    throw new BasicSyntaxException("'-' or nothing expected after line number");
                }
            }
            else if (p.TokenIsOfType(TokenType.Minus))
            {
                p.ReadToken(TokenType.Minus);
                _endLine = p.ReadInteger();
            }
            else if (!p.EndOfStatement)
            {
                throw new BasicSyntaxException("'-' or line number expected");
            }
        }

        public void List(TextWriter output)
        {

        }
    }
}
