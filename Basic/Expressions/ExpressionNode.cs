using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basic.Execute;
using Basic.Functions;
using Basic.Infrastructure;
using Basic.Parser;

namespace Basic.Expressions
{
    public interface IExpressionNode
    {
        Value Evaluate(ExecutionContext ctx);
        void List(TextWriter output);
    }

    /// <summary>
    /// Literal value
    /// </summary>
    internal class LiteralNode : IExpressionNode
    {
        private Value _literalValue;

        internal LiteralNode(Value newType)
        {
            _literalValue = newType;    
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            return _literalValue;
        }

        public void List(TextWriter output)
        {
            output.Write(_literalValue.ConvertForList());
        }
    }

    /// <summary>
    /// Reference to a variable
    /// </summary>
    internal class VariableNode : IExpressionNode
    {
        private string _variableName;

        internal VariableNode(string newVarName)
        {
            _variableName = newVarName;
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            if (!ctx.Variables.TryGetVariable(_variableName, out var value))
            {
                throw new Exception($"Unknown variable '{value}'");    
            }

            return value;
        } 

        public void List(TextWriter output)
        {
            output.Write(_variableName);
        }
    }

    internal class NumericUnaryExpression : IExpressionNode
    {
        private IExpressionNode _subExpression;

        internal NumericUnaryExpression(TokenType type, IExpressionNode subTree)
        {
            _subExpression = subTree;
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            Value subValue = _subExpression.Evaluate(ctx);
            if (!subValue.IsNumber)
            {
                throw new Exception("Unary minus expects numeric value");   
            }

            return Value.CreateNumber(-subValue.NumberValue);
        }

        public void List(TextWriter output)
        {
            output.Write('-');
        }
    }

    /// <summary>
    /// Base class for binary operators. Handles only storage..
    /// </summary>
    internal class BaseBinaryExpression
    {
        protected TokenType _tokenType;
        protected IExpressionNode _leftExpression;
        protected IExpressionNode _rightExpression;

        /// <summary>
        /// ctor
        /// </summary>
        protected BaseBinaryExpression(TokenType newType, IExpressionNode leftNode, IExpressionNode rightNode)
        {
            _tokenType = newType;
            _leftExpression = leftNode;
            _rightExpression = rightNode;
        }

        public void List(TextWriter output)
        {
            _leftExpression.List(output);
            output.Write(_tokenType);
            _rightExpression.List(output);
        }
    }

    /// <summary>
    /// Standard binary operator on numbers : both operands MUST be numbers
    /// </summary>
    internal class NumericBinaryExpression : BaseBinaryExpression, IExpressionNode
    {
        internal NumericBinaryExpression(TokenType newType, IExpressionNode leftNode, IExpressionNode rightNode)
            : base(newType, leftNode, rightNode)
        {            
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            var leftValue = _leftExpression.Evaluate(ctx).GetRequiredNumber();
            var rightValue = _rightExpression.Evaluate(ctx).GetRequiredNumber();
 
            return EvaluateNumericBinary(_tokenType, leftValue, rightValue);
        }

        /// <summary>
        /// Evaluates binary operator where both operands are numbers
        /// </summary>
        internal static Value EvaluateNumericBinary(TokenType tokenType, double leftValue, double rightValue)
        {
            double result = 0;
            bool isSameValues = Numbers.IsEqual(leftValue, rightValue);

            switch (tokenType)
            {
                case TokenType.Plus:
                    result = leftValue + rightValue;
                    break;                    
                case TokenType.Minus:
                    result = leftValue - rightValue;
                    break;
                case TokenType.Mul:
                    result = leftValue * rightValue;
                    break;
                case TokenType.Div:
                    if (Numbers.IsZero(rightValue))
                    {
                        throw new Exception("Division by zero");
                    }
                    result = leftValue / rightValue;
                    break;

                // Relational operators
                case TokenType.Less:
                    result = Numbers.Bool2Number(leftValue < rightValue);
                    break;
                case TokenType.LessEQ:
                    result = Numbers.Bool2Number(leftValue < rightValue || isSameValues);
                    break;
                case TokenType.EQ:
                    result = Numbers.Bool2Number(isSameValues);
                    break;
                case TokenType.NotEQ:
                    result = Numbers.Bool2Number(!isSameValues);
                    break;
                case TokenType.Great:
                    result = Numbers.Bool2Number(leftValue > rightValue);
                    break;
                case TokenType.GreatEQ:
                    result = Numbers.Bool2Number(leftValue > rightValue || isSameValues);
                    break;

                default:
                    throw new Exception($"Unknown binary operator '{tokenType}");
            }

            return Value.CreateNumber(result);
        }
    }

    /// <summary>
    /// binary operations on either string or number.
    /// if both operands are number, process as standard numeric binary operand
    /// otherwise: make sure both operands are string by converting them, then evaluate string binary operation
    /// </summary>
    internal class MixedBinaryExpression : BaseBinaryExpression, IExpressionNode
    {
        internal MixedBinaryExpression(TokenType newType, IExpressionNode leftNode, IExpressionNode rightNode)
            : base(newType, leftNode, rightNode)
        {

        }

        public Value Evaluate(ExecutionContext ctx)
        {
            var leftValue = _leftExpression.Evaluate(ctx);
            var rightValue = _rightExpression.Evaluate(ctx);

            // Both numeric
            if (rightValue.IsNumber && leftValue.IsNumber)
            {
                return NumericBinaryExpression.EvaluateNumericBinary(_tokenType, leftValue.NumberValue, rightValue.NumberValue);
            }

            string leftString = leftValue.ConvertToString();
            string rightString = rightValue.ConvertToString();

            return EvaluateStringBinary(_tokenType, leftString, rightString);
        }

        /// <summary>
        /// Evaluates binary operator where both operands are numbers
        /// </summary>
        private static Value EvaluateStringBinary(TokenType tokenType, string leftValue, string rightValue)
        {
            if (tokenType == TokenType.Plus)
            {
                // string concatenation
                return Value.CreateString(leftValue + rightValue);
            }
            else
            {
                int lrOrder = String.Compare(leftValue, rightValue);
                bool result = false;
                switch (tokenType)
                {
                    case TokenType.Less:
                        result = (lrOrder < 0);
                        break;
                    case TokenType.LessEQ:
                        result = (lrOrder < 0) || (lrOrder == 0);
                        break;
                    case TokenType.EQ:
                        result = (lrOrder == 0);                           
                        break;
                    case TokenType.NotEQ:
                        result = (lrOrder != 0);
                        break;
                    case TokenType.Great:
                        result = (lrOrder > 0);
                        break;
                    case TokenType.GreatEQ:
                        result = (lrOrder >= 0);
                        break;

                    default:
                        // this is really parser error
                        throw new Exception($"Unknown binary operator '{tokenType}");
                }

                return Value.CreateNumber(Numbers.Bool2Number(result));
            }
        }
    }

    /// <summary>
    /// Short-circuit binary operator for 'AND' and 'OR'
    /// </summary>
    internal class ShortCircuitBinaryExpression : BaseBinaryExpression, IExpressionNode
    {
        internal ShortCircuitBinaryExpression(TokenType newType, IExpressionNode leftNode, IExpressionNode rightNode)
            : base(newType, leftNode, rightNode)
        {

        }

        public Value Evaluate(ExecutionContext ctx)
        {
            // eval left-side and see if this determines value of whole expression
            var leftValue = _leftExpression.Evaluate(ctx);
            var leftBool = leftValue.AsBoolean();
            if (ShouldShortCircuit(_tokenType, leftValue.AsBoolean()))
            {
                // it does -> stop without evaluating left side
                return Value.CreateNumber(Numbers.Bool2Number(leftBool));
            }

            // left-side determines final value
            var rightValue = _rightExpression.Evaluate(ctx);
            return Value.CreateNumber(Numbers.Bool2Number(rightValue.AsBoolean()));
        }

        /// <summary>
        /// True when expression should be short-circuited
        /// </summary>
        private static bool ShouldShortCircuit(TokenType oper, bool value)
        {
            return  (oper == TokenType.And && !value) ||
                    (oper == TokenType.Or && value);
        }
    }

    /// <summary>
    /// Function call
    /// </summary>
    internal class FunctionExpression : IExpressionNode
    {
        private FunctionTypeInfo _function;
        private List<IExpressionNode> _params;

        /// <summary>
        /// ctor
        /// </summary>
        internal FunctionExpression(FunctionTypeInfo func, List<IExpressionNode> parameters)
        {
            _function = func;
            _params = parameters;
        }

        public Value Evaluate(ExecutionContext ctx)
        {
            var paramValues = _params.Select(p => p.Evaluate(ctx)).ToList();

            return _function.CallMethod(ctx, paramValues);
        }

        public void List(TextWriter output)
        {
            output.Write($"{_function.ID}(");

            foreach(var p in _params)
            {
                p.List(output);
                if (p != _params.LastOrDefault())
                {
                    output.Write(",");
                }
            }

            output.Write(")");
        }
    }
}