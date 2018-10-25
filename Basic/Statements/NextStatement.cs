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
        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.Next();
        }

        public void List(TextWriter output)
        {
            output.Write("NEXT");
        }

        public void Parse(PartsParser p)
        {
            // nothing to parse
        }
    }
}
