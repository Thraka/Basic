using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Parser;
using Basic.Infrastructure;
using System.IO;

namespace Basic.Statements
{
    /// <summary>
    /// The 'PRINT' statement
    /// </summary>
    [BasicStatement("PRINT", "print")]
    public class PrintStatement : IStatement
    {
        private List<Expression> _expressions = new List<Expression>();

        /// <summary>
        /// ctor
        /// </summary>
        public PrintStatement()
        {
        }

        public void Execute(ExecutionContext ctx)
        {
            foreach (var expr in _expressions)
            {
                Value newValue = expr.Evaluate(ctx);

                ctx.Output.Write(newValue.ConvertToString());
            }

            ctx.Output.WriteLine();
        }

        /// <summary>
        /// Parse. Can have zero, one or more items to print; seperated by ';'
        /// </summary>
        public void Parse(PartsParser p)
        {
            while (!p.EndOfStatement)
            {
                 var valueToPrint = p.ReadExpression();
                _expressions.Add(valueToPrint);

                if (p.TokenIsOfType(TokenType.PrintItemSep))
                {
                    p.ReadToken(TokenType.PrintItemSep);
                }
                else
                {
                    break;
                }
            }
        }

        public void List(TextWriter output)
        {
            output.Write("PRINT ");

            foreach(var expr in _expressions)
            {
                expr.List(output);

                if (expr != _expressions.LastOrDefault())
                {
                    output.Write(";");
                }
            }
        }
    }
}
