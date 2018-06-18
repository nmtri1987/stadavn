using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression
{

    /// Pair of type and object
    [Serializable]
    public class TypeObjectPair
    {
        /// Object type
        public Type Type { get; private set; }

        /// Optional object (may be null)
        public object Object { get; private set; }


        ///Constructor
        public TypeObjectPair(Type type, object o)
        {
            Type = type;
            Object = o;
        }
    }

    
    /// Evaluation context, providing interface to the external resources
    public interface IEvaluationContext
    {
        /// Get external variable (variable specified as $xxx. For example, $x+$y )
        bool    TryGetValue(string name, out object value);

        /// Find external type. Returns null if type not found
        Type    FindType(string name);

        /// Call external method
        object  CallExternal(string name, object[] parameters);

        /// Try to get external object. This is different from variable and not prepended by $. For example, c.WriteLine('hello')
        bool    TryGetExternal(string name, out object value);

        /// Get list of no-name objects or type to try methods that start with .
        IEnumerable<TypeObjectPair> GetNonameObjects();

        /// Returns true if private members may be accessed
        bool    AccessPrivate { get; }
    }

    /// Compiled script expression
    public interface IOperation
    {
        /// <summary>
        /// Evaluate the operation against stack
        /// </summary>
        /// <param name="context">Evaluation context</param>
        /// <param name="stack">Stack</param>
        void Eval(IEvaluationContext context, Stack<object> stack);

        /// <summary>
        /// Returns an number of entries added to stack by the operation. For example, result for Push would be 1, and for Pop would be -1.
        /// </summary>
        int  StackBalance { get; }
    }


    
}
