using System;
using System.Collections.Generic;
using System.IO;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression
{
    /// Basic implementation of the IEvaluationContext
    public class BasicEvaluationContext : IEvaluationContext
    {
        private readonly Parser _parser;
        private readonly IEqualityComparer<string> _comparer;
        private readonly Dictionary<string, object> _variables;
        private readonly Dictionary<string, object> _objects;
        private readonly List<string> _namespaces = new List<string> { "System", "System.Text", "System.Collections", "System.Text.RegularExpressions" };
        private readonly List<TypeObjectPair> _nonameObjects=new List<TypeObjectPair>();


        /// Return current parser
        public Parser Parser    {   get { return _parser; } }

        /// Return dictionary of variables (that are referenced with $ prefix, for example $a+$b )
        public Dictionary<string, object> Variables {   get { return _variables; }  }

        /// Return dictionary of objects (that are referenced without $ prefix, for example a+b.Length )
        public Dictionary<string, object> Objects   {   get { return _objects; }   }

        /// Return dictionary of namespaces that are searched when trying to resolve type.
        /// By default System, System.Text, System.Text.RegularExpressions and System.Collections are resolved.
        public List<string> Namespaces  {   get { return _namespaces; } }


        #region ** IEvaluationContext members **

        /// Get external variable (variable specified as $xxx. For example, $x+$y )
        public virtual bool TryGetValue(string name, out object value) { return Variables.TryGetValue(name, out value); }
        
        /// Try to get external object. This is different from variable and not prepended by $. For example, c.WriteLine('hello')
        public virtual bool TryGetExternal(string name, out object value) { return Objects.TryGetValue(name, out value); }
        
        /// Returns true if private members may be accessed
        public virtual bool AccessPrivate { get { return false; } }
        
        /// Call external method
        public virtual object CallExternal(string name, object[] parameters)
        {
            throw new NotImplementedException("Function " + name + " with " + parameters.Length+" parameter(s) is not implemented");
        }

        /// Get list of no-name objects or type to try methods that start with .
        public virtual IEnumerable<TypeObjectPair> GetNonameObjects()
        {
            return _nonameObjects.Count == 0 ? null : _nonameObjects;
        }

        /// Find external type. Returns null if type not found
        public virtual Type FindType(string name)
        {
            Type t=Utils.FindType(name);
            if (t!=null)
                return t;
            foreach(var ns in Namespaces)
            {
                t = Utils.FindType(ns + "." + name);
                if (t!=null)
                    return t;
            }
            return null;
        }
        #endregion


        /// Set variable value
        public virtual void SetVariable(string name, object value)
        {
            Variables[name] = value;
        }

        /// Set object value
        public virtual void SetObject(string name, object value)
        {
            Objects[name] = value;
        }

        /// Add noname object (.X will call value.X instance method)
        public virtual void AddNonameObject(object value)
        {
            _nonameObjects.Add(new TypeObjectPair(value.GetType(), value));
        }

        /// Add noname type (.X will call type.X static method)
        public virtual void AddNonameType(Type type)
        {
            _nonameObjects.Add(new TypeObjectPair(type, null));
        }

        /// Constructor
        public BasicEvaluationContext() : this(new Parser(), StringComparer.OrdinalIgnoreCase )
        {
            _objects.Add("null",null);
            _objects.Add("true", true);
            _objects.Add("false", false);
        }

        /// Constructor
        public BasicEvaluationContext(Parser parser, IEqualityComparer<string> comparer)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (comparer == null) throw new ArgumentNullException("comparer");
            _parser = parser;
            _comparer = comparer;
            _variables = new Dictionary<string, object>(comparer);
            _objects = new Dictionary<string, object>(comparer);
        }
        
        /// Evaluate string and return the result of the evaluation
        public object Eval(string st)
        {
            return Eval<object>(st);
        }

        /// Evaluate string and return the result of the evaluation, casted to the type T
        public T Eval<T>(string st)
        {
            using (var sr = new ParsingReader(new StringReader(st)))
            {
                var o = Eval<T>(sr);
                sr.SkipWhiteSpaceAndComments();
                sr.ReadAndThrowIfNot(-1);
                return o;
            }
        }

        /// Parse stream until the end of the expression, evaluate it, and return result of the evaluation
        public object Eval(ParsingReader sr)
        {
            return Eval<object>(sr);
        }

        /// Parse stream until the end of the expression, evaluate it, and return result of the evaluation converted to type T
        public T Eval<T>(ParsingReader sr)
        {
            var p = Parser.Parse(sr);
            if (p == null)
                return Utils.To<T>(null);
            return Eval<T>(p);
        }

        /// Evaluate the specified operation and return result of the evaluation
        public T Eval<T>(IOperation op)
        {
            Stack<object> st = new Stack<object>();
            op.Eval(this,st);
            return Utils.To<T>(st.Pop());
        }

        /// Evaluate the specified operation and return result of the evaluation converted to type T
        public object Eval(IOperation op)
        {
            return Eval<object>(op);
        }

    }
}