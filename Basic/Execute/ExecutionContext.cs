using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basic.Expressions;
using Basic.Infrastructure;
using Basic.Statements;

namespace Basic.Execute
{
    /// <summary>
    /// bag of properties required during execution
    /// </summary>
    public class ExecutionContext
    {
        public Variables Variables { get; private set; } = new Variables();

        public ExecutionUnit ExecutionUnit { get; private set; } = new ExecutionUnit();

        public TextWriter Output { get; private set; } = Console.Out;

        /// <summary>
        /// Standard BASIC directory
        /// </summary>
        private string _defaultDirectory = "/Users/lsluis/Projects/Basic/Basic/Examples/";

        /// <summary>
        /// ctor
        /// </summary>
        public ExecutionContext()
        {
        }

        /// <summary>
        /// For list statement: get range of programn-lines
        /// </summary>
        public IEnumerable<ProgramLine> LineRange(int startLine, int endLine)
        {
            return ExecutionUnit.LineRange(startLine, endLine);
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        public void Reset()
        {
            Variables.Reset();
            ExecutionUnit.Reset();
        }

        /// <summary>
        /// Runs the program
        /// </summary>
        internal void StartProgram(int? startLine)
        {
            Reset();
            ExecutionUnit.StartProgram(startLine);
        }

        /// <summary>
        /// Loads a new program
        /// </summary>
        internal void LoadNewProgram(ProgramList newProgram)
        {
            ExecutionUnit.LoadNewProgram(newProgram);
        }

        /// <summary>
        /// Sets output-stream to console
        /// </summary>
        public void SetOutputToConsole()
        {
            Output = Console.Out;
        }

        /// <summary>
        /// Keep running the program until there is nothing more to run
        /// </summary>
        public void Execute()
        {
            while (true)
            {
                var statToExecute = ExecutionUnit.Step(out int lineNumber);
                if (statToExecute == null) break;

                if (!TryExecStatement(statToExecute, lineNumber))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Executes the statements.
        /// </summary>
        private void ExecuteStatements(StatementList statements)
        {
            foreach(var stat in statements)
            {
                if (!TryExecStatement(stat, 0))
                {
                    break;
                }
            }
        }

        private bool TryExecStatement(IStatement statement, int lineNumber)
        {
            try
            {
                //if (true)   //TODO log execution
                //{
                //    Output.Write($"EXEC {lineNumber,5} ");
                //    statement.List(Output);
                //    Output.WriteLine();
                //}

                statement.Execute(this);
                return true;
            }   
            catch(BasicRuntimeException runEx)
            {
                // TODO: does not include linenumber
                Console.WriteLine($"Line {lineNumber} : {runEx.Message}");
                return false;
            }
        }

        /// <summary>
        /// Execute line entered from console
        /// </summary>
        public void ExecuteInteractive(string line, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(line)) return;

            var parser = new Parser.StatementParser(line);
            var statements = parser.ParseLine();

            if (parser.IsProgramLine)
            {
                ExecutionUnit.AddProgramLine(parser.LineNumber, statements);
            }
            else
            {
                ExecuteStatements(statements);
            }
        }

        public string FullFilePath(string baseName)
        {
            return Path.ChangeExtension(Path.Combine(_defaultDirectory, baseName), ".bas");  
        }
    }
}
