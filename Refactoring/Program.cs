using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    using NFluent;

    using NUnit.Framework;

    class Program
    {
        static void Main(string[] args)
        {
        }

        [Test]
        // 1 line: int (discarded)
        // 2 line: int (stored as 'amount')
        // 3 linE: twelve ints (non null)
        public void ExploratryTests()
        {
            var result = Inv3st_Plan.Input(
@"4
12
1 1 3 4 5 6 7 8 9 10 11 12
"
);
            Check.That(result.Output()).IsEqualTo("Case #1: 1 12 132$");


            result = Inv3st_Plan.Input(
@"4
12
1 1 1 1 1 1 1 1 1 1 1 1
1
1 1 1 1 1 1 1 1 1 1 1 1"
);
            Check.That(result.Output()).IsEqualTo("Case #1: IMPOSSIBLE\nCase #2: IMPOSSIBLE");

            result = Inv3st_Plan.Input(
        @"4
100
1 1 1 1 1 1 1 1 1 1 1 13
"
);
            // looks like: (maxprice -minprice *(int(amount/minprice))
            // neg value ignored
            Check.That(result.Output()).IsEqualTo("Case #1: 1 12 1200$");
            Check.That(Inv3st_Plan.Input(@"4
11
10 2 3 4 5 6 7 8 9 10 10 2
").Output()).IsEqualTo("Case #1: 2 12 40$");

        }
    }
}
