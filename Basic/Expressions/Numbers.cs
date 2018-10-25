using System;
using System.Globalization;

namespace Basic.Expressions
{
    /// <summary>
    /// Numeric conversion
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// A very small value; used for equality testing
        /// </summary>
        private const double Epsilon = 0.000001;

        /// <summary>
        /// Basic expects '.' as decimal point
        /// </summary>
        private static NumberFormatInfo _dotNumberLocale;

        static Numbers()
        {
            _dotNumberLocale = new NumberFormatInfo();
            _dotNumberLocale.NumberDecimalSeparator = ".";
            _dotNumberLocale.NumberGroupSeparator = ",";
        }

        /// <summary>
        /// True when two doubles are considered 'the same'
        /// </summary>
        public static bool IsEqual(double val1, double val2)
        {
            return Math.Abs(val1 - val2) < Epsilon;
        }

        public static bool IsZero(double val1)
        {
            return Math.Abs(val1) < Epsilon;
        }

        /// <summary>
        /// string-number to double, using BASIC format
        /// </summary>
        public static bool TryStringToNumber(string numberCandidate, out double value)
        {
            return double.TryParse(numberCandidate, NumberStyles.Float, _dotNumberLocale, out  value);
        }

        public static double StringToNumber(string numberCandidate)
        {
            return TryStringToNumber(numberCandidate, out double value) ? value : 0;
        }

        /// <summary>
        /// double to string, using BASIC format
        /// </summary>
        internal static string NumberToString(double numberValue)
        {
            return numberValue.ToString(_dotNumberLocale);
        }

        public static double Bool2Number(bool isTrue)
        {
            return isTrue ? 1 : 0;
        }
    }
}
