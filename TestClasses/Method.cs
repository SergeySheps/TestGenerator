using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assebly_Browser_Library.Models
{
    public class Method
    {
        public string Name { get; set; }

        public string Signature { get; set; }

        public Method(MethodInfo method)
        {
            Signature = "public ";

            if (method.IsStatic)
            {
                Signature += "static ";
            }
            else if (method.IsVirtual)
            {
                Signature += "virtual ";
            }

            Signature += method + ";";
        }
    }
}
