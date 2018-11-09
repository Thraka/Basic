using System;
using System.Collections.Generic;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;

namespace Basic.Functions
{
    /// <summary>
    /// Evaluate a BASIC function
    /// </summary>
    public delegate Value FunEvaluator(ExecutionContext ctx, List<Value> paramValues);


    public class UserFunction
    {
        public string FunctionName { get; private set;}
        public List<string> FormalParameters { get; private set;}
        public Expression Value { get; private set;}

        public UserFunction(string functionName, List<string> formalParameters, Expression value)
        {
            FunctionName = functionName;
            FormalParameters = formalParameters;
            Value = value;
        }
    }

}
