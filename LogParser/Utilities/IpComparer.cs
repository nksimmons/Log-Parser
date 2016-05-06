using System.Collections.Generic;
using System.Linq;

namespace LogParser.Utilities
{
    public class IpComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return a.Split('.').Zip(b.Split('.'),
                                 (x, y) => int.Parse(x).CompareTo(int.Parse(y)))
                             .FirstOrDefault(i => i != 0);
        }
    }
}
