using System;
using System.IO;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("INPUT", "Read a number from console")]
    public class InputStatement : IStatement
    {
        private string _varName;

        public void Execute(ExecutionContext ctx)
        {
            var line = Console.ReadLine();

            if (_varName.EndsWith('$'))
            {
                ctx.Variables.Set(_varName, Value.CreateString(line));
            }
            else
            {
                if (Numbers.TryStringToNumber(line.Trim(), out double number))
                {
                    throw new BasicRuntimeException($"INPUT: not an number, line='{line}'");
                }

                ctx.Variables.Set(_varName, Value.CreateNumber(number));
             }
        }

        public void List(TextWriter output)
        {
            output.Write($"INPUT {_varName}");
        }

        public void Parse(PartsParser p)
        {
            _varName = p.ReadIdentifier();
        }
    }
}
