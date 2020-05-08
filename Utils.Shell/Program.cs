using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kw.WinAPI;
using Microsoft.Win32;

using Kw.Common;
using Kw.Common.Containers;

namespace Utils.Shell
{
    public static class KeyboardHelper
    {
        public static Keys VK2Keys(VK vk)
        {
            return (Keys) ((int) vk);
        }

        public static VK Keys2VK(Keys keys)
        {
            switch (keys)
            {
                case Keys.Alt: return VK.MENU;
                case Keys.Control: return VK.LCONTROL;
                case Keys.Shift: return VK.LSHIFT;
                case Keys.KeyCode:
                case Keys.LineFeed:
                case Keys.Modifiers:
                case Keys.None:
                    return (VK) 0;
                default:
                    return (VK) keys;
            }

            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var mtx = Mutex.OpenExisting("DBWinMutex1");

            var vks = EnumHelper.GetValues<VK>().Cast<int>().Distinct().Cast<VK>().OrderBy(k => k.ToString()).ToArray();
            var keys = EnumHelper.GetValues<Keys>().Cast<int>().Distinct().Cast<Keys>().OrderBy(k => k.ToString()).ToArray();

            var commons = vks.Cast<int>().Intersect(keys.Cast<int>()).ToArray();

            Console.WriteLine("Common values");

            var scommons = new StringBuilder();

            foreach (var k in commons)
            {
                Console.WriteLine($"{(VK)k} {(Keys)k}");
                scommons.AppendLine($"{(VK)k} {(Keys)k}");
            }

            var scs = scommons.ToString();

            Console.WriteLine("Specific VK values");

            var vks_e = vks.Cast<int>().Except(commons).Cast<VK>().ToArray();

            foreach (var k in vks_e)
            {
                Console.WriteLine($"{k}");
            }

            Console.WriteLine("Specific Keys values");

            var keys_e = keys.Cast<int>().Except(commons).Cast<Keys>().ToArray();

            foreach (var k in keys_e)
            {
                Console.WriteLine($"{k}");
            }

        }
    }
}
