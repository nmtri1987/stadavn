using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Pop a value from stack</summary>
    [Serializable]
    public class OperationPop : IOperation
    {
        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack) { stack.Pop(); }

        /// Returns an number of entries added to stack by the operation. Returns -1, as value is removed from stack
        public int StackBalance { get { return -1; } }

        /// Returns a <see cref="T:System.String"/> that represents the current object.
        public override string ToString() { return "pop()"; }
    }
}