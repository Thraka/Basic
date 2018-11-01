using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("STOP", "Stop program execution")]
    public class StopStatement : IStatement
    {
        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.End();
        }

        public void List(TextWriter output)
        {
            output.Write("STOP");
        }

        public void Parse(PartsParser p)
        {
            // keyword-only statement
        }
    }
}
