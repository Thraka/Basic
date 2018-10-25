using System;
using System.IO;
using Basic.Execute;

namespace Basic.Expressions
{
    public class Expression
    {
        private IExpressionNode _expr;

        public Expression(IExpressionNode newExpression)
        {
            _expr = newExpression;
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            return _expr.Evaluate(ctx);
        }

        public void List(TextWriter output)
        {
            _expr.List(output);
        }
    }
}
