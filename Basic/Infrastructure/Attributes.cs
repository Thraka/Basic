using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Basic.Execute;
using Basic.Expressions;
using Basic.Functions;
using Basic.Statements;

namespace Basic.Infrastructure
{
    /// <summary>
    /// base
    /// </summary>
    public class BasicAttribute : Attribute
    {
        /// <summary>
        /// Short ID used in code
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Description, informational only.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public BasicAttribute(string id, string desc)
        {
            ID = id;
            Description = desc;
        }
    }

    /// <summary>
    /// Marks a class as (implementing) a statement
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class BasicStatementAttribute : BasicAttribute
    {
        /// <summary>
        /// When true, the command is allowed inside a program
        /// when false, only allowed are immediate statement.
        /// </summary>
        public bool AllowInProgram { get; set; } = true;

        /// <summary>
        /// ctor
        /// </summary>
        public BasicStatementAttribute(string id, string desc)
            : base(id, desc)
        {
        }
    }

    /// <summary>
    /// Marks a method as (implementing) a function
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class BasicFunctionAttribute : BasicAttribute
    {
        /// <summary>
        /// Minimal and maximal number of parameters
        /// </summary>
        public int MinNrParameters { get; set; } = 0;
        public int MaxNrParameters { get; set; } = Int32.MaxValue;

        /// <summary>
        /// ctor
        /// </summary>
        public BasicFunctionAttribute(string id, string desc)
            : base(id, desc)
        {
        }
    }

    /// <summary>
    /// Describes a statement class.
    /// Really: a tuple of (Type, BasicStatementAttribute)
    /// </summary>
    public class StatementTypeInfo
    {
        private BasicStatementAttribute _attribute;
        private Type _type;

        public string ID { get { return _attribute.ID; } }
        public string Description { get { return _attribute.Description; } }

        /// <summary>
        /// ctor
        /// </summary>
        public StatementTypeInfo(Type type, IEnumerable<BasicStatementAttribute> attribs)
        {
            _attribute = attribs.Single();
            _type = type;
           
            if (!IsValidID(_attribute.ID))
            {
                throw new Exception($"'{ID}' is not a valid ID");               
            }

            if (!typeof(IStatement).IsAssignableFrom(type))
            {
                throw new Exception($"'{ID}' does not implement IStatement");
            }
        }

        /// <summary>
        /// True when it is a valid ID
        /// </summary>
        internal static bool IsValidID(string id)
        {
            return  !string.IsNullOrEmpty(id) &&
                      char.IsLetter(id[0]) &&
                      id.All(ch => char.IsLetterOrDigit(ch) || ch == '$');
        }

        internal IStatement CreateInstance()
        {
            return (IStatement)Activator.CreateInstance(_type);
        }
    }

    /// <summary>
    /// describes a class implementing a BASIC function
    /// </summary>
    public class FunctionTypeInfo
    {
        private BasicFunctionAttribute _attribute;
        private MethodInfo _method;

        public string ID { get { return _attribute.ID; } }
        public string Description { get { return _attribute.Description; } }
        public int MinNrParameters { get; private set; }
        public int MaxNrParameters { get; private set; }
        public FunEvaluator CallMethod { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public FunctionTypeInfo(MethodInfo method, IEnumerable<BasicFunctionAttribute> attribs)
        {
            _method = method;
            _attribute = attribs.Single();
            MinNrParameters = _attribute.MinNrParameters; 
            MaxNrParameters = _attribute.MaxNrParameters;

            if (!StatementTypeInfo.IsValidID(_attribute.ID))
            {
                throw new Exception($"'{ID}' is not a valid ID");
            }

            var caller = Delegate.CreateDelegate(typeof(FunEvaluator), _method, false);
            if (caller != null)
            {
                CallMethod = (FunEvaluator)caller;
            }
            else 
            {
                //TODO: dit is een tikje convoluted. Nu ook kennis Value en ExecutionContext nodig :-(
                var callerN1 = Delegate.CreateDelegate(typeof(FunEvaluatorN1), _method, false);
                if (callerN1 == null)
                {
                    throw new Exception($"'{ID}' is not a FunEvaluator delegate");
                }

                CallMethod = (ExecutionContext ctx, List<Value> paramValues) =>
                {
                    var number = paramValues[0].GetRequiredNumber();

                    return Value.CreateNumber(((FunEvaluatorN1)callerN1)(number));
                };

                // Have tested compatibility with FunEvaluatorN1, safe to set min/max
                MinNrParameters = 1;
                MaxNrParameters = 1;
            }
        }
    }

    /// <summary>
    /// Helper: finds methods and classes decorated with function- or statement attribute
    /// </summary>
    public static class TypeFinder
    {
        /// <summary>
        /// finds all classes that are statements
        /// </summary>
        public static List<StatementTypeInfo> FindStatements()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var statTypes =
                from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(BasicStatementAttribute), true)
                where attributes != null && attributes.Length > 0
                select new StatementTypeInfo(t, attributes.Cast<BasicStatementAttribute>() );
            
            return statTypes.ToList();
        }

        /// <summary>
        /// Finds all BASIC functions.
        /// </summary>
        public static List<FunctionTypeInfo> FindFunctions()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var functionMethods =
                   (from t in assembly.GetTypes()
                    from m in t.GetMethods(BindingFlags.Static|BindingFlags.Public)
                    let attributes = m.GetCustomAttributes(typeof(BasicFunctionAttribute), true)
                    where attributes != null && attributes.Length > 0
                    select new FunctionTypeInfo(m, attributes.Cast<BasicFunctionAttribute>())).ToList();

            return functionMethods;
        }
    }
}
