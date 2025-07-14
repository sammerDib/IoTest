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
    internal class OleStorage : IDisposable
    {
        //const IID IID_IStorage is defined as 0000000B-0000-0000-C000-000000000046
        private const long STG_E_FILEALREADYEXISTS = 0x80030050L;
        private const int _WriteFlags = (int)(STGMFlags.STGM_READWRITE | STGMFlags.STGM_SHARE_EXCLUSIVE) ;
        private const int _ReadFlags = (int)(STGMFlags.STGM_READ | STGMFlags.STGM_SHARE_EXCLUSIVE);
        private int _flags = (int)(STGMFlags.STGM_SHARE_EXCLUSIVE);

        private IStorage _storage;
        public string Name { get; private set; }
        public bool IsDisposed => (_storage == null);

        // constructors ...
        private OleStorage(IStorage storage, string name, int flags)
        {
            if (storage == null)
                throw new ArgumentNullException("storage");

            _flags = flags;
            _storage = storage;
            Name = name;
        }

        // destructor ...
        ~OleStorage()
        {
            Dispose();
        }

        // public methods...
        /// <summary>
        /// Disposes the storage ...
        /// </summary>
        public void Dispose()
        {
            if (_storage != null)
            {
                if (((_flags & (int)STGMFlags.STGM_READWRITE) | (_flags & (int)STGMFlags.STGM_WRITE)) != 0)
                    _storage.Commit(0);
                Marshal.ReleaseComObject(_storage);
                _storage = null;
            }
        }

        /// <summary>
        /// Creates new instance of the ole storage. (Read Only access)
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public static OleStorage CreateInstance(string path)
        {
            IStorage storage;
            int result = NativeMethods.StgOpenStorage(path, null, _ReadFlags, IntPtr.Zero, 0, out storage);
            if (result != 0)
                throw new ApplicationException("Failed to open " + path);
            return new OleStorage(storage, "Root", _ReadFlags);
        }

        /// <summary>
        /// Creates new Writable instance of the ole storage. (Read & Write access)
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public static OleStorage CreateWritableInstance(string path)
        {
            IStorage storage;
            int nFlags = _WriteFlags;
            if (!System.IO.File.Exists(path))
            {
                nFlags |= (int)(STGMFlags.STGM_CREATE);
                System.Guid iiref_storage = new System.Guid(Guids.IStorage);
                int result = NativeMethods.StgCreateStorageEx(path, nFlags, 0, 0, IntPtr.Zero, IntPtr.Zero, ref iiref_storage, out storage);
                if (result != 0)
                    throw new ApplicationException("Failed to create " + path);
            }
            else
            {
                int result = NativeMethods.StgOpenStorage(path, null, nFlags, IntPtr.Zero, 0, out storage);
                if (result != 0)
                    throw new ApplicationException("Failed to open "+ path);
            }
            return new OleStorage(storage, "Root", nFlags);
        }

        /// <summary>
        /// Open new instance of the ole storage.
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public OleStorage OpenStorage(string name)
        {
            if (_storage == null)
            {
                return null;
            }

            IStorage storage;
            int result = _storage.OpenStorage(name, null, _flags, null, 0, out storage);
            if (result != 0)
                throw new ApplicationException("Failed to open " + name);
            return new OleStorage(storage, name, _flags);
        }

        /// <summary>
        /// Creates new instance of the ole storage.
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public OleStorage CreateStorage(string name)
        {
            if (_storage == null)
            {
                return null;
            }

            IStorage storage;
            // Check if it already exist
            int nFlags = _flags;
            nFlags &= ~((int)STGMFlags.STGM_CREATE);
            int result = 0;
            try
            {
                result = _storage.OpenStorage(name, null, nFlags, null, 0, out storage);
                if (result == 0) // si oK
                    return new OleStorage(storage, name, _flags);
            }
            catch (COMException excom)
            {
                // analyze ErrorCode
                result = excom.ErrorCode;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Failed to open " + name, ex);
            }

            if (result != 0)
            {
                // it doesnot exist create it
                nFlags |= (int)(STGMFlags.STGM_CREATE);
                result = _storage.CreateStorage(name, nFlags, 0, 0, out storage);
                if (result == 0) // si oK
                    return new OleStorage(storage, name, _flags);

            }
            throw new ApplicationException("Failed to open " + name);
        }

        /// <summary>
        /// Closes the storage.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Opens stream with the given name.
        /// </summary>
        /// <param name="name">The name of the stream to open.</param>
        public OleStream OpenStream(string name)
        {
            System.Runtime.InteropServices.ComTypes.IStream stream;
            try
            {
                int result = _storage.OpenStream(name, IntPtr.Zero, _flags, 0, out stream);
                if (result == 0)
                    return new OleStream(stream, name);
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Failed to open stream " + name + " in file " + Name, e);
            }
            throw new ApplicationException("Failed to open stream " + name + " in file " + Name);
        }

        /// <summary>
        /// Create stream with the given name.
        /// </summary>
        /// <param name="name">The name of the stream to open.</param>
        public OleStream CreateStream(string name)
        {
            System.Runtime.InteropServices.ComTypes.IStream stream;
            int result;
            try
            {
                result = _storage.CreateStream(name, _flags, 0, 0, out stream);
                if (result == 0)
                    return new OleStream(stream, name);
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Failed to create stream " + name + " in file " + Name, e);
            }
            throw new ApplicationException("Failed to create stream " + name + " in file " + Name);
        }

        /// <summary>
        /// Reads data from the specified stream.
        /// </summary>
        /// <param name="name">The name of the stream to read.</param>
        public byte[] ReadStream(string name)
        {
            using (OleStream stream = OpenStream(name))
                return stream.ReadToEnd();
        }
    }
}
