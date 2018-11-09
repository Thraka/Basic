using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// FOR - Begin of a for-next loop
    /// </summary>
    [BasicStatement("FOR", "for-next")]
    public class ForStatement : IStatement
    {
        private string _varName;
        private Expression _initValue;
        private Expression _endValue;

        //TODO: bleh!  Hoort hier niet
        private Value _endValueX;
        private ExecutionContext _ctx;

        /// <summary>
        /// begin 'for' loop
        /// </summary>
        public void Execute(ExecutionContext ctx)
        {
            Value value = _initValue.Evaluate(ctx);
            Value endValue = _endValue.Evaluate(ctx);

            if (!value.IsNumber || !endValue.IsNumber)
            {
                throw new Exception("'FOR' requires numeric values");
            }

            ctx.Variables.Set(_varName, value);

            ctx.ExecutionUnit.StartFor(this);

            _ctx = ctx;
            _endValueX = endValue;
        }

        internal bool TryNext(string optionalVariableName)
        {
            if (!string.IsNullOrEmpty(optionalVariableName) && optionalVariableName != _varName)
            {
                throw new BasicRuntimeException($"NEXT variable '{optionalVariableName}' does not match FOR variable '{_varName}'");
            }
            if (!_ctx.Variables.TryGetVariable(_varName, out Value currentValue))
            {
                throw new Exception($"Cannot find for-next  variable '{_varName}'");
            }
            double nextValue = currentValue.NumberValue + 1;

            _ctx.Variables.Set(_varName, Value.CreateNumber(nextValue));

            return nextValue <= _endValueX.NumberValue;
        }

        /// <summary>
        /// LIST
        /// </summary>
        public void List(TextWriter output)
        {
            output.Write($"FOR {_varName}=");
            _initValue.List(output);
            output.Write(" TO ");
            _endValue.List(output);
        }

        /// <summary>
        /// Syntax: FOR (id) '=' (initial value 'TO' (end value)
        /// </summary>
        /// <param name="p">P.</param>
        public void Parse(PartsParser p)
        {
            _varName = p.ReadIdentifier();

            p.ReadToken(TokenType.EQ);

            _initValue = p.ReadExpression();

            p.ReadContextualKeyword("TO");

            _endValue = p.ReadExpression();
        }
    }
}
