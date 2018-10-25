using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("SAVE", "Save a program to file")]
    public class SaveStatement : IStatement
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
            using (var os = File.CreateText(path))
            {
            }
         }

        public void List(TextWriter output)
        {
            //
        }

        public void Parse(PartsParser p)
        {
            _nameExpr = p.ReadExpression();
        }
    }
}
