using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Functions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("DEF", "Define a user function")]
    public class DefStatement : IStatement
    {
        private UserFunction _userFn;

        public void Execute(ExecutionContext ctx)
        {
            ctx.UserFunctions.Add(_userFn);
        }

        public void List(TextWriter output)
        {
            output.Write($"DEF FN{_userFn.FunctionName}(");

            var lastParam = _userFn.FormalParameters.LastOrDefault();
            foreach(var varName in _userFn.FormalParameters.)
            {
                output.Write($"{varName}");
                if (varName != lastParam) output.Write(",");
            }
            output.Write(")=");
            _userFn.Value.List(output);
        }

        public void Parse(PartsParser p)
        {
            var functionName = p.ReadUserFunctionName();

            var paramNames = p.ReadFormalParameterList();

            var functionValue = p.ReadAssignment();

            _userFn = new UserFunction(functionName, paramNames, functionName);
        }
    }
}
