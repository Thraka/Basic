using System;
using System.IO;
using System.Collections.Generic;
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
            if (ProgramList.TryLoad(fileName, out ProgramList newProgram, out List<string> errorMessages))
            {
                ctx.LoadNewProgram(newProgram);
            }
            else
            {
                foreach(var msg in errorMessages)
                {
                    Console.WriteLine(msg);
                }
            }
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
