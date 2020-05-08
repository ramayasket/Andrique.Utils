using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Andrique.Utils.NoOperation
{
    class Program
    {
        static void Main()
        {
            var mtx = new Mutex(false, "zlp");

            Console.ReadLine();
        }
    }
}
