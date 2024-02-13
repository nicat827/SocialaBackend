
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Common
{
    public static  class DataHelper
    {
        public static string Capitalize(this string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
        }
    }
}
