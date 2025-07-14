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

using OStorageTools.Native;

namespace OStorageTools.Ole
{
    public class OleStorage : IDisposable
    {

        //const IID IID_IStorage is defined as 0000000B-0000-0000-C000-000000000046
        private const long STG_E_FILEALREADYEXISTS = 0x80030050L;
        private const int _WriteFlags = (int)(STGMFlags.STGM_READWRITE | STGMFlags.STGM_SHARE_EXCLUSIVE);
        private const int _ReadFlags = (int)(STGMFlags.STGM_READ | STGMFlags.STGM_SHARE_EXCLUSIVE);
        private int _DefaultFlags = (int)(STGMFlags.STGM_SHARE_EXCLUSIVE);
        private IStorage _Storage;
        private string _Name;
        private bool _bDisposableStrategy;

        // constructors ...
        private OleStorage(IStorage storage, string name, int Flags)
        {
            if (storage == null)
                throw new ArgumentNullException("storage");

            _DefaultFlags = Flags;
            _Storage = storage;
            _Name = name;
            _bDisposableStrategy = 0 != ((_DefaultFlags & (int)STGMFlags.STGM_READWRITE) | (_DefaultFlags & (int)STGMFlags.STGM_WRITE));
        }

        // destructor ...
        ~OleStorage()
        {
            Dispose(false);
        }

        // private methods...    
        private void Dispose(bool isDisposing)
        {
            if (_Storage == null)
                return;
            if (isDisposing)
            {
                _Storage.Commit(0);
            }
            Marshal.ReleaseComObject(_Storage);
            _Storage = null;
        }

        // public methods...
        /// <summary>
        /// Disposes the storage ...
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                GC.SuppressFinalize(this);
                Dispose(_bDisposableStrategy);
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
                return null;
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
                UInt32 result = NativeMethods.StgCreateStorageEx(path, nFlags, 0, 0, IntPtr.Zero, IntPtr.Zero, ref iiref_storage, out storage);
                if (result != 0)
                    return null;
            }
            else
            {
                int result = NativeMethods.StgOpenStorage(path, null, nFlags, IntPtr.Zero, 0, out storage);
                if (result != 0)
                    return null;
            }
            return new OleStorage(storage, "Root", nFlags);
        }

        public static bool IsAStorageFile(string path)
        {
            return (NativeMethods.StgIsStorageFile(path) == 0);
        }

        /// <summary>
        /// Open new instance of the ole storage.
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public OleStorage OpenStorage(string name)
        {
            if (_Storage == null)
            {
                return null;
            }

            IStorage storage;
            int result = _Storage.OpenStorage(name, null, _DefaultFlags, null, 0, out storage);
            if (result != 0)
                return null;
            return new OleStorage(storage, name, _DefaultFlags);
        }

        /// <summary>
        /// Creates new instance of the ole storage.
        /// </summary>
        /// <param name="path">The path of the file to create storage for</param>
        public OleStorage CreateStorage(string name)
        {
            if (_Storage == null)
            {
                return null;
            }

            IStorage storage;
            // Check if it already exist
            int nFlags = _DefaultFlags;
            nFlags &= ~((int)STGMFlags.STGM_CREATE);
            int result = 0;
            try
            {
                result = _Storage.OpenStorage(name, null, nFlags, null, 0, out storage);
                if (result == 0) // si oK
                    return new OleStorage(storage, name, _DefaultFlags);
            }
            catch (COMException excom)
            {
                // analyze ErrorCode
                result = excom.ErrorCode;
                string sMsg = String.Format("OleStorage COMException Exception {0}", excom);
            }
            catch (System.Exception ex)
            {
                string sMsg = String.Format("OleStorage CreateStream Exception {0}", ex);
                //MessageBox.Show(sMsg, "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (result != 0)
            {
                // it doesnot exist create it
                nFlags |= (int)(STGMFlags.STGM_CREATE);
                result = _Storage.CreateStorage(name, nFlags, 0, 0, out storage);
                if (result == 0) // si oK
                    return new OleStorage(storage, name, _DefaultFlags);

            }
            return null;
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
                int result = _Storage.OpenStream(name, IntPtr.Zero, _DefaultFlags, 0, out stream);
                if (result != 0)
                    return null;
            }
            catch (System.Exception e)
            {
                string sMsg = String.Format("OleStream OpenStream Exception {0}", e);
                //MessageBox.Show(sMsg, "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return new OleStream(stream, name);
        }

        /// <summary>
        /// Create stream with the given name.
        /// </summary>
        /// <param name="name">The name of the stream to open.</param>
        public OleStream CreateStream(string name)
        {
            System.Runtime.InteropServices.ComTypes.IStream stream;
            try
            {
                int result = _Storage.CreateStream(name, _DefaultFlags, 0, 0, out stream);
                if (result != 0)
                    return null;
            }
            catch (System.Exception e)
            {
                string sMsg = String.Format("OleStream CreateStream Exception {0}", e);
                //MessageBox.Show(sMsg, "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return new OleStream(stream, name);
        }

        /// <summary>
        /// Reads data from the specified stream.
        /// </summary>
        /// <param name="name">The name of the stream to read.</param>
        public byte[] ReadStream(string name)
        {
            OleStream stream = OpenStream(name);
            try
            {
                return stream.ReadToEnd();
            }
            finally
            {
                stream.Close();
            }
        }


        // public properties...
        public string Name
        {
            get
            {
                return _Name;
            }
        }
        public bool IsDisposed
        {
            get
            {
                return _Storage == null;
            }
        }
    }
}