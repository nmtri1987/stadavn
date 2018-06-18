using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    /// <summary>
    /// Push value into stack
    /// </summary>
    [Serializable]
    public class OperationPush : IOperation
    {
        readonly object _value;

        /// Constructor
        public OperationPush(object value)  {   _value = value;}

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack) { stack.Push(_value); }

        /// Returns an number of entries added to stack by the operation. Returns 1, as value is pushed into stack
        public int StackBalance { get { return 1; } }

        /// Returns a <see cref="T:System.String"/> that represents the current object.
        public override string ToString() { return "push(" + _value + ")"; }
    }
}