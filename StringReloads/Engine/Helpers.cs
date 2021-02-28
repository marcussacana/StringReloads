using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Engine
{
    static class Helpers
    {
        internal static T TryGet<T>(Func<T> Action)
        {
            try
            {
                return Action();
            }
            catch {
                return default;
            }
        }
    }
}
