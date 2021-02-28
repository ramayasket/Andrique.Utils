using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;
using Kw.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clipcopy
{
    public enum KeyMove
    {
        Up,
        Down
    }

    internal abstract class Daemon
    {
        #region Events
        protected enum KeyAction
        {
            Release,
            Swallow,
            Collect,
        }

        protected class KeyEvent
        {
            public KeyMove Move { get; }
            public Keys KeyCode { get; }

            public KeyEvent(Keys key, KeyMove move)
            {
                KeyCode = key;
                Move = move;
            }

            public override string ToString()
            {
                return $"{Move}.{KeyCode}";
            }
        }

        protected class InternalHandling : IDisposable
        {
            public static bool Ignore { get; private set; }

            public InternalHandling()
            {
                Ignore = true;
            }

            public void Dispose()
            {
                Ignore = false;
            }
        }
        #endregion

        #region Instance infrastructure
        private static readonly Type Type = typeof(Daemon);
        private static readonly Assembly Module = Type.Assembly;
        private static readonly Type[] Concretes = Module.GetTypes().Where(t => !t.IsAbstract && t.Is(Type)).ToArray();

        private static readonly Daemon[] Daemons = Concretes
                .Select(t => t.GetConstructor(new Type[0])?.Invoke(new object[0]))
                .OfType<Daemon>()
                .ToArray();
        #endregion

        //
        //    Instance handling
        //
        protected abstract KeyEvent[] Sequence { get; }
        protected abstract KeyAction Match();

        private int CurrentIndex = 0;

        private KeyAction ProcessInstance(KeyEvent @event)
        {
            KeyAction action = KeyAction.Release;

            var compare = Sequence[CurrentIndex];

            //Console.WriteLine($"Enter: {CurrentIndex}: {compare}");

            if (compare.KeyCode == @event.KeyCode && compare.Move == @event.Move)
            {
                CurrentIndex ++;

                //Console.WriteLine($"Match: {CurrentIndex}: {@event}");

                if (CurrentIndex == Sequence.Length)
                {
                    CurrentIndex = 0;
                    //Console.WriteLine($"Reset: {CurrentIndex}: {@event}");
                    action = Match();
                }
                else
                {
                    action = KeyAction.Collect;
                }
            }
            else
            {
                CurrentIndex = 0;
                //Console.WriteLine($"Mismatch: {CurrentIndex}: {@event}");
            }

            //Console.WriteLine($"Exit: {CurrentIndex}");

            return action;
        }

        //
        //    Static handling
        //
        protected static readonly List<KeyEvent> _events = new List<KeyEvent>();

        private static KeyAction ProcessEvent(KeyEvent @event)
        {
            var action = KeyAction.Release;

            foreach (var daemon in Daemons)
            {
                action = (KeyAction)Math.Max((int)daemon.ProcessInstance(@event), (int)action);
            }

            return action;
        }

        private static void EmulateEvent(KeyEvent @event)
        {
            var vk = KeyboardHelper.Keys2VK(@event.KeyCode);

            if (0 == vk)
                return;

            if (KeyMove.Down == @event.Move)
            {
                User.KeyboardDown(vk);
            }
            else
            {
                User.KeyboardUp(vk);
            }
        }

        private static void PostProcess(KeyAction action, KeyEventArgs args, KeyEvent @event)
        {
            Debug.WriteLine($"-->{action}");

            if (action > KeyAction.Release)
            {
                args.Handled = true;

                Debug.WriteLine("Handled");

                if (KeyAction.Collect == action)
                {
                    _events.Add(@event);
                }
            }
            else
            {
                foreach (var keyEvent in _events)
                {
                    
                }
            }
        }

        public static void OnKeyUp(object sender, KeyEventArgs args)
        {
            var @event = new KeyEvent(args.KeyCode, KeyMove.Up);
            Debug.WriteLine($"{KeyMove.Up}.{args.KeyCode}");
            var action = ProcessEvent(@event);

            PostProcess(action, args, @event);
        }

        public static void OnKeyDown(object sender, KeyEventArgs args)
        {
            var @event = new KeyEvent(args.KeyCode, KeyMove.Down);
            Debug.WriteLine($"{KeyMove.Down}.{args.KeyCode}");
            var action = ProcessEvent(@event);

            PostProcess(action, args, @event);
        }
    }

    //internal class StartDaemon : Daemon
    //{
    //    protected override KeyEvent[] Sequence => new []
    //    {
    //        new KeyEvent(Keys.LWin, KeyMove.Down),
    //        new KeyEvent(Keys.LMenu, KeyMove.Down),
    //        new KeyEvent(Keys.Return, KeyMove.Down),

    //        new KeyEvent(Keys.LWin, KeyMove.Up),
    //        new KeyEvent(Keys.LMenu, KeyMove.Up),
    //        new KeyEvent(Keys.Return, KeyMove.Up),
    //    };
    //}

    //internal class WinampDaemon : Daemon
    //{
    //    protected override KeyEvent[] Sequence => new[]
    //    {
    //        new KeyEvent(Keys.LControlKey, KeyMove.Down),
    //        new KeyEvent(Keys.M, KeyMove.Down),

    //        new KeyEvent(Keys.LControlKey, KeyMove.Up),
    //        new KeyEvent(Keys.M, KeyMove.Up),
    //    };

    //    protected override KeyAction Match()
    //    {
    //        return KeyAction.Swallow;
    //    }
    //}

    internal class abcDaemon : Daemon
    {
        protected override KeyEvent[] Sequence => new[]
        {
            new KeyEvent(Keys.A, KeyMove.Down),
            new KeyEvent(Keys.A, KeyMove.Up),
            new KeyEvent(Keys.B, KeyMove.Down),
            new KeyEvent(Keys.B, KeyMove.Up),
            new KeyEvent(Keys.C, KeyMove.Down),
            new KeyEvent(Keys.C, KeyMove.Up),
        };

        protected override KeyAction Match()
        {
            return KeyAction.Release;
        }
    }
}
