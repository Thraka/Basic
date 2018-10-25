using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// 'GOTO' statement - transfer flow-control to given line
    /// </summary>
    [BasicStatement("GOTO", "Goto linenumber")]
    public class GotoStatement : IStatement
    {
        private int _nextLineNumber;

        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.Goto(_nextLineNumber);
        }

        public void Parse(PartsParser p)
        {
            _nextLineNumber = p.ReadInteger();
        }

        public void List(TextWriter output)
        {
            output.Write($"GOTO {_nextLineNumber}");
        }
    }
}
