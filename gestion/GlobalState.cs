using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gestion
{
    public static class GlobalState
    {
        private static string _authToken;
        public static string AuthToken
        {
            get { return _authToken; }
            set { _authToken = value; }
        }
    }
}
