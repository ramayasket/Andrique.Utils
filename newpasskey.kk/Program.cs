using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Andrique.Utils;

namespace Andrique.Utils.NewPasskey
{
    class Program : ConsoleUtilityBase
    {
        const int PASSKEY_LENGTH = 32;

        static int Main(string[] args)
        {
            return new Program().Run(args);
        }

        Stream _torrentFile;
        int _torrentFileLength;
        string _oldPassKey, _newPassKey;
        byte[] _buffer;

        public override int RequiredArgumentCount
        {
            get { return 3; }
        }

        public override int UtilityRun(string[] args)
        {
            var filePath = args[0];

            _oldPassKey = args[1];
            _newPassKey = args[2];

            using (_torrentFile = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                _torrentFileLength = Convert.ToInt32(_torrentFile.Length);

                _buffer = new byte[_torrentFileLength];

                var bytesRead = _torrentFile.Read(_buffer, 0, _torrentFileLength);

                Debug.Assert(_torrentFileLength == bytesRead);

                var passKeyIx = FindPassKey();

                if (-1 != passKeyIx)
                {
                    var passKeyBuffer = Encoding.ASCII.GetBytes(_newPassKey);

                    for (int i = 0; i < PASSKEY_LENGTH; i++)
                    {
                        _buffer[passKeyIx + i] = passKeyBuffer[i];
                    }

                    _torrentFile.Seek(0, SeekOrigin.Begin);
                    _torrentFile.Write(_buffer, 0, _torrentFileLength);
                }
            }

            return 0;
        }

        int FindPassKey()
        {
            int ix = 0;
            int ixLimit = _torrentFileLength - _oldPassKey.Length;

            var passKeyBuffer = Encoding.ASCII.GetBytes(_oldPassKey);

            bool found = false;

            while (ix <= ixLimit)
            {
                found = true;

                for (int i = 0; i < PASSKEY_LENGTH; i++)
                {
                    if (_buffer[ix + i] != passKeyBuffer[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return ix;
                }

                ix++;
            }

            return -1;
        }
    }
}
