#region Copyright (c) 2006-2008 Cellbi
/*
Cellbi Software Component Product
Copyright (c) 2006-2008 Cellbi
www.cellbi.com

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

	1.	Redistributions of source code must retain the above copyright notice,
			this list of conditions and the following disclaimer.

	2.	Redistributions in binary form must reproduce the above copyright notice,
			this list of conditions and the following disclaimer in the documentation
			and/or other materials provided with the distribution.

	3.	The names of the authors may not be used to endorse or promote products derived
			from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED “AS IS” AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL CELLBI
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Runtime.InteropServices;
using UnitySC.PM.DMT.Service.DMTCalTransform.Native;

namespace UnitySC.PM.DMT.Service.DMTCalTransform.Ole
{
    internal class OleStream : IDisposable
    {
        private System.Runtime.InteropServices.ComTypes.IStream _stream;
        public string Name { get; private set; }
        public bool IsDisposed => (_stream == null);

        // constructors...
        public OleStream(System.Runtime.InteropServices.ComTypes.IStream stream, string name)
        {
            _stream = stream;
            Name = name;
        }

        // public methods...
        /// <summary>
        /// Disposes the stream ...
        /// </summary>
        public void Dispose()
        {
            if (_stream == null)
            {
                _stream.Commit(0);
                Marshal.ReleaseComObject(_stream);
                _stream = null;
            }
        }

        /// <summary>
        /// Reads all stream data.
        /// </summary>
        public byte[] ReadToEnd()
        {
            System.Runtime.InteropServices.ComTypes.STATSTG stat;
            _stream.Stat(out stat, 0);
            long size = stat.cbSize;

            byte[] buffer = new byte[size];
            _stream.Read(buffer, (int)size, IntPtr.Zero);
            return buffer;
        }

        /// <summary>
        /// Writes the specified bytes into the stream.
        /// </summary>
        /// <param name="data">The data to write.</param>
        public void Write(byte[] data)
        {
            _stream.Write(data, data.Length, IntPtr.Zero);
        }

        public void WriteInt(int nVal)
        {
            Write(BitConverter.GetBytes(nVal));
        }

        public void WriteFloat(float fVal)
        {
            Write(BitConverter.GetBytes(fVal));
        }

        public void WriteDouble(double dVal)
        {
            Write(BitConverter.GetBytes(dVal));
        }

        public void WriteUInt32(uint uVal)
        {
            Write(BitConverter.GetBytes(uVal));
        }

        public byte[] ReadBuffer(int nSize)
        {
            byte[] buffer = new byte[nSize];
            _stream.Read(buffer, (int)nSize, IntPtr.Zero);
            return buffer;
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadBuffer(sizeof(int)), 0);
        }
        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBuffer(sizeof(float)), 0);
        }
        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBuffer(sizeof(double)), 0);
        }
        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBuffer(sizeof(uint)), 0);
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

    }
}
