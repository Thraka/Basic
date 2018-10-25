using System;
using System.Globalization;

namespace Basic.Expressions
{
    public enum ValueType
    {
        Unknown,
        Number,
        String,
    };

    /// <summary>
    /// A Value, result of a calculation
    /// </summary>
    public class Value
    {
        public static Value Default = new Value()
        {
            _valueType = ValueType.Unknown,
            StringValue = String.Empty
        };

        private ValueType _valueType = ValueType.Unknown;

        public string StringValue { get; private set; } = String.Empty;
        public double NumberValue { get; private set; }

        public bool IsNumber
        {
            get { return _valueType == ValueType.Number; }
        }

        public bool IsString
        {
            get { return _valueType == ValueType.String; }
        }

        public static Value CreateNumber(double number)
        {
            return new Value() { _valueType = ValueType.Number, NumberValue = number };
        }

        public static Value CreateNumber(string number)
        {
            return new Value() { _valueType = ValueType.Number, NumberValue = Numbers.StringToNumber(number) };
        }

        public static Value CreateString(string text)
        {
            return new Value() { _valueType = ValueType.String, StringValue = text ?? string.Empty };
        }

        /// <summary>
        /// returns value as a string for "LIST"
        /// </summary>
        public string ConvertForList()
        {
            if (_valueType == ValueType.String)
            {
                return '"' + StringValue.Replace("\"", "\\\"") + '"';
            }
            else
            {
                return ConvertToString();
            }
        }

        /// <summary>
        /// Convert any value to string to use, example: to print
        /// </summary>
        public string ConvertToString()
        {
            switch (_valueType)
            {
                case ValueType.String:
                    return StringValue;
                case ValueType.Number:
                    return Numbers.NumberToString(NumberValue);
                case ValueType.Unknown:
                    return string.Empty;
                default:
                    throw new Exception($"Cannot convert type '{_valueType}' to string");
            }
        }

        /// <summary>
        /// Interpret a value as boolean
        /// </summary>
        public bool AsBoolean()
        {
            switch (_valueType)
            {
                case ValueType.String:
                    return StringValue != String.Empty;
                case ValueType.Number:
                    return !Numbers.IsZero(NumberValue);
                case ValueType.Unknown:
                    return false;
                default:
                    throw new Exception($"Cannot convert type '{_valueType}' to string");
            }          
        }

        /// <summary>
        /// A string-value is required, get it
        /// </summary>
        public string GetRequiredString()
        {
            if (_valueType != ValueType.String)
            {
                throw new Exception($"Value '{NumberValue}' : string required");
            }

            return StringValue ?? String.Empty;
        }

        /// <summary>
        /// A numeric value is required
        /// </summary>
        public double GetRequiredNumber()
        {
            if (_valueType != ValueType.Number)
            {
                throw new Exception($"Value '{StringValue}' : number required");
            }

            return NumberValue;             
        }

        /// <summary>
        /// An integer numeric number is required
        /// </summary>
        public int GetRequiredInt()
        {
            return (int) Math.Round(GetRequiredNumber());
        }
    }
}