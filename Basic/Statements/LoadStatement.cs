using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("LOAD", "Load a program from file")]
    public class LoadStatement : IStatement
    {
        private Expression _nameExpr;

        public void Execute(ExecutionContext ctx)
        {
            var name = _nameExpr.Evaluate(ctx);
            if (!name.IsString || string.IsNullOrEmpty(name.StringValue))
            {
                throw new Exception($"Expected name for 'load' statement");
            }

            var path = ctx.FullFilePath(name.StringValue);
            LoadFromFile(ctx, path);
        }

        private void LoadFromFile(ExecutionContext ctx, string fileName)
        {
            if (ProgramList.TryLoad(fileName, out ProgramList newProgram))
            {
                ctx.LoadNewProgram(newProgram);
            }

            ctx.LoadNewProgram(newProgram);
        }

        public void Parse(PartsParser p)
        {
            _nameExpr = p.ReadExpression();
        }

        public void List(TextWriter output)
        {

        }
    }
}
