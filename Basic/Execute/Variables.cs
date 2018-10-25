using System;
using System.Collections.Generic;
using Basic.Expressions;

namespace Basic.Execute
{
    /// <summary>
    /// Manages the variables collection
    /// </summary>
    public class Variables
    {
        /// <summary>
        /// Variables, lookup is NOT case-sensitive
        /// </summary>
        private Dictionary<string, Value> _variables = new Dictionary<string, Value>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// ctor
        /// </summary>
        public Variables()
        {
        }

        /// <summary>
        /// Reset everything
        /// </summary>
        public void Reset()
        {
            _variables = new Dictionary<string, Value>();
        }

        /// <summary>
        /// 'LET' : defines a new variable
        /// </summary>
        public void DeclareNew(string name, Value newValue)
        {
            if (_variables.ContainsKey(name))
            {
                throw new Exception($"Variable {name} already defined");
            }

            _variables[name] = newValue;
        }

        /// <summary>
        /// Variable assignment to existing variable.
        /// </summary>
        public void AssignExisting(string name, Value newValue)
        {
            if (!_variables.ContainsKey(name))
            {
                throw new Exception($"Variable {name} is not defined");
            }

            _variables[name] = newValue;
        }

        /// <summary>
        /// Set new value of new or existing variable
        /// </summary>
        public void Set(string name, Value newValue)
        {
            _variables[name] = newValue;
        }

        /// <summary>
        /// Variable GET
        /// </summary>
        public bool TryGetVariable(string name, out Value currentValue)
        {
            return _variables.TryGetValue(name, out currentValue);
        }
    }
}
