using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Parser;
using Basic.Statements;
using Basic.Infrastructure;

namespace Basic.Execute
{
    using ForPosition = Tuple<ForStatement, ExecutionUnit.ExecPosition>;

    public class ExecutionUnit
    {
        /// <summary>
        /// current execution position.
        /// struct to make copying easier
        /// </summary>
        public struct ExecPosition
        {
            public int LineIndex { get; set; }
            public int StatementIndex { get; set; }

            public void SetNewLine(int l)
            {
                LineIndex = l;
                StatementIndex = 0;
            }
        }

        /// <summary>
        /// Current program
        /// </summary>
        private ProgramList _programList = new ProgramList();
          
        /// <summary>
        /// Program counter
        /// </summary>
        private ExecPosition _pc = new ExecPosition();
        private bool _isRunning = false;

        /// <summary>
        /// for gosub/return : stack with return addresses
        /// </summary>
        private Stack<ExecPosition> _returnStack = new Stack<ExecPosition>();

        /// <summary>
        /// For next stack.
        /// </summary>
        private Stack<ForPosition> _forNextStack = new Stack<ForPosition>();

        /// <summary>
        /// ctor
        /// </summary>
        internal ExecutionUnit()
        {
            Reset();
        }

        /// <summary>
        /// Get lines
        /// </summary>
        public IEnumerable<ProgramLine> LineRange(int startLine, int endLine)
        {
            return _programList.Lines.Where(l => l.Number >= startLine && l.Number <= endLine);
        }

        /// <summary>
        /// Activates a new program
        /// </summary>
        internal void LoadNewProgram(ProgramList newProgram)
        {
            _programList = newProgram;
            Reset();
        }

        /// <summary>
        /// Get next statement to execute
        /// </summary>
        internal IStatement Step(out int lineNumber)
        {
            lineNumber = 0;
            if (_pc.LineIndex >= _programList.Count)
            {
                _isRunning = false;
            }
            if (!_isRunning) return null;
 
            var line = _programList[_pc.LineIndex];
            var stat = line.Statements[_pc.StatementIndex];
            lineNumber = line.Number;

            // increment PC, ready to execute next statement
            _pc.StatementIndex++;
            if (_pc.StatementIndex >= line.Statements.Count)
            {
               _pc.SetNewLine(_pc.LineIndex + 1);

                //TODO: problem whenever Statements.Count == 0
            }

            return stat;
        }

        /// <summary>
        /// Reset flow-control
        /// </summary>
        internal void Reset()
        {
            _pc.LineIndex = 0;
            _pc.StatementIndex = 0;
            _isRunning = false;
            _returnStack.Clear();
            _forNextStack.Clear();
        }

        /// <summary>
        /// Adds the program line.
        /// </summary>
        internal void AddProgramLine(int lineNumber, StatementList statements)
        {
            _programList.Insert(lineNumber, statements);
        }

        /// <summary>
        /// Starts the program.
        /// </summary>
        internal void StartProgram(int? startLine)
        {
            Reset();

            int startIndex = startLine.HasValue ? _programList.IndexOf(startLine.Value) : 0;
            _pc = new ExecPosition() { LineIndex = startIndex, StatementIndex = 0 };
            _isRunning = (startIndex < _programList.Count);
        }

        /// <summary>
        /// Goto specified line, go back to current position on 'return'
        /// </summary>
        internal void Gosub(int lineNumber)
        {
            _returnStack.Push(_pc);

            _pc = new ExecPosition();
            Goto(lineNumber);
        }

        /// <summary>
        /// resume from calling position after GOSUB
        /// </summary>
        internal void Return()
        {
            if (!_returnStack.TryPop(out ExecPosition returnPos))
            {
                throw new BasicRuntimeException("Cannot return: no gosub active");
            }

            _pc = returnPos;
        }

        /// <summary>
        /// Execute from specified line
        /// </summary>
        internal void Goto(int lineNumber)
        {
            _pc.SetNewLine(_programList.IndexOf(lineNumber));
        }

        /// <summary>
        /// Begin of for loop
        /// </summary>
        internal void StartFor(ForStatement forStatement)
        {
            var x = Tuple.Create(forStatement, _pc);
            _forNextStack.Push(x);
        }

        /// <summary>
        /// end-of-statement: increment and loop back 
        /// </summary>
        internal void Next()
        {
            if (!_forNextStack.TryPeek(out var x))
            {
                throw new BasicRuntimeException("Next: FOR statement not available");
            }

            if (x.Item1.TryNext())
            {
                _pc = x.Item2;
            }
            else
            {
                _forNextStack.Pop();
            }
        }

        /// <summary>
        /// Flowcontrol: end execution
        /// </summary>
        internal void End()
        {
            _isRunning = false;
        }
    }
}
