using System;
using System.IO;
using Basic.Execute;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// 'RUN' statement - start executing the stored program
    /// </summary>
    [BasicStatement("RUN", "Run")]
    public class RunStatement : IStatement
    {
        /// <summary>
        /// Optional startline: if set, starts execution at tha line. Otherwise: start at first line
        /// </summary>
        private int? _startLine;

        public void Execute(ExecutionContext ctx)
        {
            ctx.StartProgram(_startLine);
        }

        public void Parse(PartsParser p)
        {
            _startLine = null;
            if (p.TokenIsOfType(TokenType.Number))
            {
                _startLine = p.ReadInteger();
            }
        }

        public void List(TextWriter output)
        {
            if (_startLine.HasValue)
            {
                output.Write($"RUN {_startLine.Value}");
            }
            else
            {
                output.Write("RUN");
            }
        }
    }
}
