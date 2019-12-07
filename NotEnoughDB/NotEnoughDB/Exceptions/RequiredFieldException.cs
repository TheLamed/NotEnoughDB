using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughDB.Exceptions
{
    [System.Serializable]
    public class RequiredFieldException : Exception
    {
        public string Field { get; protected set; }

        public RequiredFieldException() { }
        public RequiredFieldException(string field) : base(field + " is required")
        {
            Field = field;
        }
        public RequiredFieldException(string field, string message) : base(message)
        {
            Field = field;
        }
        public RequiredFieldException(string message, Exception inner) : base(message, inner) { }
        protected RequiredFieldException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
