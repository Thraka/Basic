using System;
using System.IO;
using Basic.Execute;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// REM command - line is comment
    /// </summary>
    [BasicStatement("REM", "Comments")]
    public class RemStatement : IStatement
    {
        private string _comment;

        public void Execute(ExecutionContext ctx)
        {
            // Nothing to do...
        }

        public void Parse(PartsParser p)
        {
            _comment = p.SkipLine();
        }

        public void List(TextWriter output)
        {
            output.Write($"REM {_comment}");
        }
    }
}
