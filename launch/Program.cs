using System;
using System.Diagnostics;
using System.Linq;

namespace launch
{
    class Program
    {
        static void Main(string[] args) {

            if (!args.Any()) {

                Console.WriteLine("Usage: launch program [arguments]");
                return;
            }

            try {
                new Process { StartInfo = new ProcessStartInfo(args[0], string.Join(" ", args.Skip(1))) }.Start();
            }
            catch (Exception x) {
                Console.WriteLine(x.Message);
            }
        }
    }
}
