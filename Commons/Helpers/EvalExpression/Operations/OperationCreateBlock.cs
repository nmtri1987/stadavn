using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Replace top N objects on top of the stack with a single collection object</summary>
    [Serializable]
    public class OperationCreateBlock : IOperation
    {
        readonly int _paramCount;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramCount">Number of objects to read from stack</param>
        public OperationCreateBlock(int paramCount)
        {
            _paramCount=paramCount;
        }

        /// Returns number of entries added to stack by the operation. 
        public int StackBalance
        {
            get { return 1-_paramCount; }
        }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            var o = OperationHelper.PopArray(stack, _paramCount);

            // Find a general object type
            Type parent = null;
            foreach (var o1 in o)
                parent = Utils.CommonBase(o1, parent);
            
            if (parent == null || parent == typeof(object))
                stack.Push(o);
            else
            {
                Array a = (Array)Activator.CreateInstance(parent.MakeArrayType(), o.Length);
                for (int i = 0; i < a.Length; ++i)
                    a.SetValue(o[i], i);
                stack.Push(a);
            }
        }

        /// Return string representation of the expression
        public override string ToString()
        {
            return "block(" + _paramCount + ")";
        }
    }
}