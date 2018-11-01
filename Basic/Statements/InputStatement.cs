using System;
using System.IO;
using System.Collections.Generic;
using Basic.Execute;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Statements
{
    [BasicStatement("INPUT", "Read a number from console")]
    public class InputStatement : IStatement
    {
        private string _optionalCaption = string.Empty;
        private List<string> _variables = new List<string>();


        public void Execute(ExecutionContext ctx)
        {
            var line = Console.ReadLine();

            throw new Exception("Dit werkt niet..");
        }

        public void List(TextWriter output)
        {
            output.Write($"INPUT");
        }

        public void Parse(PartsParser p)
        {
            if (p.TokenIsOfType(TokenType.String))
            {
                _optionalCaption = p.ReadStringLiteral();

                if (!p.TokenIsOfType(TokenType.Comma))
                {
                    throw new BasicSyntaxException("',' expected after INOUT prompt");
                }
                p.NextToken();
            }

            while (p.TokenIsOfType(TokenType.Identifier))
            {
                _variables.Add(p.ReadIdentifier());
                
                if (p.TokenIsOfType(TokenType.Comma))
                {
                    p.NextToken();
                }
            }
        }
    }
}
