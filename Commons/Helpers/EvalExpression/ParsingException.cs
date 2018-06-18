using System;
using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression
{
    /// <summary>
    /// Expression thrown when parsing script or expression
    /// </summary>
    [Serializable]
    public class ParsingException : Exception
    {
        /// Default constructor
        public ParsingException()
        {
        }

        /// Constructor with message
        public ParsingException(string message)
            : base(message)
        {
        }

        /// Constructor with message and inner exception
        public ParsingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// Serialization constructor
        protected ParsingException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}