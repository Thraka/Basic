using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// LET command - 'LET identifier '=' expression
    /// </summary>
    [BasicStatement("LET", "Assigns a value to a new variable")]
    public class LetStatement : IStatement
     {
        private string _variableName;
        private Expression _newValue;

        /// <summary>
        /// Execute the the LET assignment
        /// </summary>
        public void Execute(ExecutionContext ctx)
        {
            Value newValue = _newValue.Evaluate(ctx);
            ctx.Variables.DeclareNew(_variableName, newValue);
        }

        /// <summary>
        /// Parse the LET statement
        /// </summary>
        public void Parse(PartsParser p)
        {
            _variableName = p.ReadIdentifier();

            p.ReadToken(TokenType.EQ);

            _newValue = p.ReadExpression();
        }

        public void List(TextWriter output)
        {

        }
    }
}
