using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
           
namespace Basic.Functions
{
    /// <summary>
    /// Simplified version of FunEvaluator : for BASIC functions that take a number
    /// and return a number, without needing access to the ExecutionContext
    /// Min- and MaxParameters must be 1
    /// </summary>
    public delegate double FunEvaluatorN1(double input);

   /// <summary>
    /// Numeric functions
    /// </summary>
    public static class NumericFunctions
    {
        [BasicFunction("ABS", "Absolute value")]
        public static double Abs(double input)
        {
            return Math.Abs(input);
        }

        [BasicFunction("ATN", "arctangent")]
        public static double Atn(double input)
        {
            return Math.Atan(input);
        }

        [BasicFunction("COS", "Cosine")]
        public static double Cos(double input)
        {
            return Math.Cos(input);
        }

        [BasicFunction("EXP", "transcendental number e raised to the power of the value")]
        public static double Exp(double input)
        {
            return Math.Exp(input);
        }

        [BasicFunction("INT", "Rounds a number")]
        public static double Int(double input)
        {
            return Math.Round(input);
        }

        [BasicFunction("LOG", "Logarithm")]
        public static double Log(double input)
        {
            return Math.Log(input);
        }

        [BasicFunction("SIN", "Sine")]
        public static double Sin(double input)
        {
            return Math.Sin(input);
        }

        [BasicFunction("SQR", "square root")]
        public static double Sqr(double input)
        {
            return Math.Sqrt(input);
        }

        [BasicFunction("TAN", "Tangens")]
        public static double Tan(double input)
        {
            return Math.Tan(input);
        }

        [BasicFunction("SGN", "Sign of parameter")]
        public static double Sgn(double input)
        {
            if (Numbers.IsZero(input))
            {
                return 0;
            }
            else
            {
                return (input < 0) ? -1 : 1;
            }
        }

        //TODO: should probably be part of the Exec-Context
        private static Random _rand = new Random();

        [BasicFunction("RND", "Random", MinNrParameters = 0, MaxNrParameters = 1)]
        public static Value Rnd(ExecutionContext ctx, List<Value> paramValues)
        {
            var limit = 1.0;
            if (paramValues.Count == 1)
            {
                limit = paramValues[0].GetRequiredNumber();
            }

            double randValue = 0;
            if (Numbers.IsEqual(limit, 1.0))
            {
                randValue = _rand.NextDouble();
            }
            else
            {
                randValue = _rand.Next((int)randValue);
            }

            return Value.CreateNumber(randValue);
        }
    }
}
