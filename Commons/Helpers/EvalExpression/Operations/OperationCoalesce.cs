using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Conditional operator. Get the top value from stack and execute the expression if the value is not null</summary>
    [Serializable]
    public class OperationCoalesce : IOperation
    {
        readonly IOperation _ifNull;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ifNull">Expression to execute if the condition is null</param>
        public OperationCoalesce(IOperation ifNull)
        {
            _ifNull = ifNull;
        }

        /// Returns number of entries added to stack by the operation. 
        public int StackBalance
        {
            get
            {
                return  1 - _ifNull.StackBalance;
            }
        }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            var p = stack.Pop();
            if (p == null)
                _ifNull.Eval(context, stack);
            else
                stack.Push(p);
        }
    }
}