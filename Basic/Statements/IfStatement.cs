using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// IF-THEN flowcontrol
    /// </summary>
    [BasicStatement("IF", "IF-THEN flowcontrol")]
    public class IfStatement : IStatement
    {
        private Expression _condition;
        private int _whenTrueLinenumber;

        public void Execute(ExecutionContext ctx)
        {
            var conditionValue = _condition.Evaluate(ctx);

            if  (conditionValue.AsBoolean())
            {
                ctx.ExecutionUnit.Goto(_whenTrueLinenumber);
            }
        }

        public void Parse(PartsParser p)
        {
            _condition = p.ReadExpression();

            p.ReadContextualKeyword("THEN");

            _whenTrueLinenumber = p.ReadInteger();

        }

        public void List(TextWriter output)
        {
            output.Write("IF ");

            _condition.List(output);

            output.Write($" THEN {_whenTrueLinenumber}");
        }
    }
}
