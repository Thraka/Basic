using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("GOSUB", "Gosub")]
    public class GosubStatement : IStatement
    {
        private int _nextLineNumber;

        public void Execute(ExecutionContext ctx)
        {
            ctx.ExecutionUnit.Gosub(_nextLineNumber);
        }

        public void Parse(PartsParser p)
        {
            _nextLineNumber = p.ReadInteger();
        }

        public void List(TextWriter output)
        {
            output.Write($"GOSUB {_nextLineNumber}");
        }
    }
}
