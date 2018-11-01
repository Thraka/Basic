using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// NEXT - end of a 'for' loop
    /// </summary>
    [BasicStatement("NEXT", "Next")]
    public class NextStatement : IStatement
    {
        ///
        /// Optional variable name. If supplied, must match inner FOR name
        private string _varName = null;

        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.Next(_varName);
        }

        public void List(TextWriter output)
        {
            output.Write($"NEXT {_varName ?? String.Empty}");
        }

        public void Parse(PartsParser p)
        {
            if (!p.EndOfStatement && p.TokenIsOfType(TokenType.Identifier))
            {
                _varName = p.ReadIdentifier();
            }
        }
    }
}
