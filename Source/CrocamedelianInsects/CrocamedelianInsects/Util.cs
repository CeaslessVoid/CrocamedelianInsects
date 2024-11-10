using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianInsects
{
    internal static class Util
    {
        public static void Msg(object o)
        {
            Log.Message("[CR Insects] " + ((o != null) ? o.ToString() : null));
        }

    }

}
