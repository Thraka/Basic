using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// Assignment statement: varioable-name '=' expression
    /// </summary>
    [BasicStatement("ASSIGN", "Assigns a value to a new or existing variable")]
    public class AssignmentStatement : IStatement
    {
        private string _variableName;
        private Expression _newValue;

        /// <summary>
        /// ctor
        /// </summary>
        public AssignmentStatement(string varName)
        {
            _variableName = varName;
        }

        /// <summary>
        /// Execute the  assignment
        /// </summary>
        public void Execute(ExecutionContext ctx)
        {
            Value newValue = _newValue.Evaluate(ctx);
            ctx.Variables.Set(_variableName, newValue);
        }

        /// <summary>
        /// Parse the LET statement
        /// </summary>
        public void Parse(PartsParser p)
        {
            _newValue = p.ReadExpression();
        }

        public void List(TextWriter output)
        {
            output.Write($"{_variableName}=");
            _newValue.List(output);
        }
    }
}
