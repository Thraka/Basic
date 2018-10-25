using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Basic.Execute;
using Basic.Parser;

namespace Basic.Statements
{
    /// <summary>
    /// IStatement
    /// </summary>
    public interface IStatement
    {
        void Execute(ExecutionContext ctx);
        void Parse(PartsParser p);
        void List(TextWriter output);
    }

    /// <summary>
    /// Simple list of statements
    /// </summary>
    public class StatementList : IEnumerable<IStatement>
    {
        private List<IStatement> _statements = new List<IStatement>();

        public int Count
        {
            get { return _statements.Count; }
        }

        public IStatement this[int index]
        {
            get { return _statements[index]; }
        }

        /// <summary>
        /// Add new statement
        /// </summary>
        public void Add(IStatement newStatement)
        {
            _statements.Add(newStatement);
        }

        public IEnumerator<IStatement> GetEnumerator()
        {
            return ((IEnumerable<IStatement>)_statements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IStatement>)_statements).GetEnumerator();
        }
    }
}
