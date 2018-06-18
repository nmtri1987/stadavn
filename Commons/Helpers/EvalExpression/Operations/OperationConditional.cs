using System;
using System.Collections.Generic;
using System.Text;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Conditional operator. Get the top value from stack, convert it to bool, and execute one of the expressions</summary>
    [Serializable]
    public class OperationConditional : IOperation
    {
        readonly IOperation _ifTrue;
        readonly IOperation _ifFalse;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ifTrue">Expression to execute if the condition is true</param>
        /// <param name="ifFalse">Expression to execute if the condition is false</param>
        public OperationConditional(IOperation ifTrue, IOperation ifFalse)
        {
            _ifTrue = ifTrue;
            _ifFalse = ifFalse;
        }

        /// Returns number of entries added to stack by the operation. 
        public int StackBalance
        {
            get
            {
                return 1-Math.Max(_ifTrue.StackBalance,_ifFalse.StackBalance);
            }
        }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            var cond = Utils.To<bool>(stack.Pop());
            if (cond)
                _ifTrue.Eval(context,stack);
            else
                _ifFalse.Eval(context, stack);
        }

        /// Returns a string representation of the current object
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("conditional(");
            sb.Append(_ifTrue);
            sb.Append(", ");
            sb.Append(_ifFalse);
            sb.Append(")");
            return sb.ToString();
        }

    }

}