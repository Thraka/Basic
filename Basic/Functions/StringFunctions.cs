using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;

namespace Basic.Functions
{
    /// <summary>
    /// Basic stringfunctions
    /// </summary>
    public static class StringFunctions
    {
        [BasicFunction("ASC", "Ascii value of first char in input", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Asc(ExecutionContext ctx, List<Value> paramValues)
        {
            string text = paramValues[0].GetRequiredString();

            if (string.IsNullOrEmpty(text))
            {
                throw new BasicRuntimeException("ASC: no string given");

            }

            byte[] asciiValue = Encoding.ASCII.GetBytes(text.Substring(0, 1));
            return Value.CreateNumber(asciiValue[0]);
        }

        [BasicFunction("CHR$", "Convert ascii value to char", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Chr(ExecutionContext ctx, List<Value> paramValues)
        {
            int asciiVal = paramValues[0].GetRequiredInt();
            char c = (char)asciiVal;
            return Value.CreateString(new string(c, 1));
        }

        [BasicFunction("LEFT$", "returns left <max-chars> character from start of the string", MinNrParameters = 2, MaxNrParameters = 2)]
        public static Value Left(ExecutionContext ctx, List<Value> paramValues)
        {
            string text = paramValues[0].GetRequiredString();
            int maxLen = paramValues[1].GetRequiredInt();

            if (maxLen > text.Length)
            {
                maxLen = text.Length;
            }
            if (maxLen < 0)
            {
                throw new BasicRuntimeException("Expected non-negative length for `LEFT' function");
            }
            return Value.CreateString(text.Substring(0, maxLen));
        }
 
        [BasicFunction("LEN", "returns length of the string", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Len(ExecutionContext ctx, List<Value> paramValues)
        {
            string text = paramValues[0].GetRequiredString();

            return Value.CreateNumber(text.Length);
        }

        [BasicFunction("MID$", "Mid", MinNrParameters = 2, MaxNrParameters = 3)]
        public static Value Mid(ExecutionContext ctx, List<Value> paramValues)
        {
            string text = paramValues[0].GetRequiredString();
            int index = paramValues[1].GetRequiredInt();
            int count = (paramValues.Count == 3) ? paramValues[2].GetRequiredInt() : text.Length;

            if (index < 1)
            {
                throw new BasicRuntimeException("MID: start position is invalid");
            }
            if (count < 0)
            {
                throw new BasicRuntimeException("MID: count is invalid");
            }

            index--;    // 1-based -> 0-based
            if (index >= text.Length)
            {
                return Value.CreateString(string.Empty);
            }
            if (count > text.Length - index)
            {
                count = text.Length - index;
            }

            return Value.CreateString(text.Substring(index, count));
        }
  
        [BasicFunction("RIGHT$", "Right", MinNrParameters = 2, MaxNrParameters = 2)]
        public static Value Right(ExecutionContext ctx, List<Value> paramValues)
        {
            string text = paramValues[0].GetRequiredString();
            int count = paramValues[1].GetRequiredInt();

            if (count < 0)
            {
                throw new BasicRuntimeException("RIGHT: count is invalid");
            }
            if (count > text.Length)
            {
                count = text.Length;
            }
            return Value.CreateString(text.Substring(text.Length - count, count));
        }

        [BasicFunction("SPC", "String of spaces", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Spc(ExecutionContext ctx, List<Value> paramValues)
        {
            var count = paramValues[0].GetRequiredInt();
            if (count < 0)
            {
                throw new BasicRuntimeException($"SPC() : only non-negative numbers");
            }

            return Value.CreateString(new string(' ', count));
        }

        [BasicFunction("STR", "float as a string", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Str(ExecutionContext ctx, List<Value> paramValues)
        {
            var value = paramValues[0].GetRequiredNumber();

            return Value.CreateString(Numbers.NumberToString(value));
        }

        [BasicFunction("TAB", "Repeats spaces", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Tab(ExecutionContext ctx, List<Value> paramValues)
        {
            var count = paramValues[0].GetRequiredInt();
            if (count < 0)
            {
                throw new BasicRuntimeException($"TAB() : only non-negative numbers");
            }

            return Value.CreateString(new string(' ', count));
        }

        [BasicFunction("VAL", "string to double", MinNrParameters = 1, MaxNrParameters = 1)]
        public static Value Val(ExecutionContext ctx, List<Value> paramValues)
        {
            var text = paramValues[0].GetRequiredString();

            return Value.CreateNumber(Numbers.StringToNumber(text));
        }
    }
}
