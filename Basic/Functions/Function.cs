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
}
