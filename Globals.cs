using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public static class Globals
    {
        public static List<string> CachedRoleTalentIds = new List<string>();

        public static string Ascending
        {
            get
            {
                return "Ascending";
            }
        }

        public static string Descending
        {
            get
            {
                return "Descending";
            }
        }
    }
}