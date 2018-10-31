using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basic.Infrastructure;
using Basic.Statements;

namespace Basic.Execute
{
    /// <summary>
    /// small holder class
    /// </summary>
    public class ProgramLine
    {
        public int Number { get; private set; }
        public StatementList Statements { get; private set; }

        public ProgramLine(int newNum, StatementList newStatements)
        {
            Number = newNum;
            Statements = newStatements;
        }
    }

    public class ProgramList
    {
        /// <summary>
        /// all lines in the program, sorted on Line.Number
        /// </summary>
        private List<ProgramLine> _lines = new List<ProgramLine>();

        public int Count { get { return _lines.Count; } }

        /// <summary>
        /// Indexer into _lines
        /// </summary>
        internal ProgramLine this[int index]
        {
            get { return _lines[index]; }
        }

        public IEnumerable<ProgramLine> Lines 
        {
            get { return _lines; }    
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ProgramList()
        {
        }

        public void Insert(int lineNumber, StatementList newStatements)
        {
            var newLine = new ProgramLine(lineNumber, newStatements);

            int lineIndex = 0;
            while (lineIndex < _lines.Count && _lines[lineIndex].Number < lineNumber)
            {
                ++lineIndex;
            }

            if (lineIndex >= _lines.Count)
            {
                _lines.Add(newLine);
            }
            else if (lineNumber == _lines[lineIndex].Number)
            {
                // replacement of existing line
                _lines[lineIndex] = newLine;
            }
            else
            {
                _lines.Insert(lineIndex, newLine);
            }
        }

        /// <summary>
        /// Load program from file
        /// </summary>
        public static bool TryLoad(string fileName, out ProgramList newProgram, out List<string> errorMessages)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            using (var inp = File.OpenText(fileName))
            {
                return TryLoadFromStream(inp, baseName, out newProgram, out errorMessages);
            }
        }

        public static bool TryLoadFromString(string programText, out ProgramList newProgram, out List<string> errorMessages)
        {
            using(var inp = new StringReader(programText))
            {
                return TryLoadFromStream(inp, "<string>", out newProgram, out errorMessages);
            }
        }

        private static bool TryLoadFromStream(TextReader inp, string baseName, out ProgramList newProgram, out List<string> errorMessages)
        {
            const int MaxErrors = 10;

            newProgram = new ProgramList();
            errorMessages = new List<string>();
                              
            var parser = new Parser.StatementParser(inp);

            //while (!inp.EndOfStream)
            while (inp.Peek() >= 0)
            {
                try
                {
                    var statements = parser.ParseLine();

                    if (parser.IsProgramLine)
                    {
                        newProgram.Insert(parser.LineNumber, statements);
                    }
                    else
                    {
                        throw new Exception("Immediate command in file");
                    }
                }
                catch (BasicSyntaxException synEx)
                {
                    var loc = baseName + synEx.Location;
                    errorMessages.Add($"{loc} : {synEx.Message}");
                    if (errorMessages.Count > MaxErrors)
                    {
                        errorMessages.Add("(Too many errors, quiting)");
                        break;
                    }
                }
            }

            return (errorMessages.Count == 0);
        }

        /// <summary>
        /// Get required line
        /// </summary>
        internal int IndexOf(int lineNumber)
        {
            var line = _lines.Select((programLine, idx) => new { programLine.Number, idx }).Where(ln => ln.Number == lineNumber).FirstOrDefault();

            return line?.idx ?? throw new BasicRuntimeException($"Cannot find program line {lineNumber}");
        }
    }
}
