using System;
using Basic.Execute;
using Basic.Infrastructure;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Basic active");

            ExecutionContext ctx = new ExecutionContext();
            while(true)
            {
                ctx.Execute();

                string input = Console.ReadLine();
                if (input == "quit") break;

                try
                {
                    ctx.ExecuteInteractive(input, ctx);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
