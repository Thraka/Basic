using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// 'RETURN' statement - go back to statement after original 'GOSUB' call
    /// </summary>
    [BasicStatement("RETURN", "Resume after gosub")]
    public class ReturnStatement : IStatement
    {
        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.Return();
        }

        public void Parse(PartsParser p)
        {
            // empty statement
        }

        public void List(TextWriter output)
        {
            output.Write("RETURN");
        }
    }
}
