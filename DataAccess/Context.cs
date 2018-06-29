using System;
using System.Collections.Generic;

namespace DataAccess
{
    public static class Context
    {
        public static ConnectionType ConnectionType { get; set; }

        public static Dictionary<Type, dynamic> TypeMapper = new Dictionary<Type, dynamic>();
    }
}
