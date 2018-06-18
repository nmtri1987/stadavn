using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Cast the object on top of the stack to the given type, then push it back</summary>
    [Serializable]
    public class OperationIs : IOperation
    {
        readonly string _typeName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeName">Type name. May contain ? and/or suffix. For example, int[] or int? or int or System.Int32</param>
        public OperationIs(string typeName)
        {
            _typeName = typeName;
        }

        /// Returns an number of entries added to stack by the operation. 0 in this case
        public int StackBalance { get { return 0; } }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            var p = stack.Pop();
            Type t = OperationHelper.ResolveType(context, _typeName);
            if (t == null)
                throw new TypeLoadException("Failed to resolve type '" + _typeName + "'");
            if (p==null)
                stack.Push(false);
            else
                stack.Push(t.IsAssignableFrom(p.GetType()));
        }

        /// Returns a <see cref="T:System.String"/> that represents the current object.
        public override string ToString()
        {
            return "is(" + _typeName + ")";
        }
    }
}