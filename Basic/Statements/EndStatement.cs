using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("END", "Stop program execution")]
    public class EndStatement : IStatement
    {
        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.End();
        }

        public void List(TextWriter output)
        {
            output.Write("END");
        }

        public void Parse(PartsParser p)
        {
            // keyword-only statement
        }
    }
}
