using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Pack
{
    public class PackReader
    {
        private const string LibraryPath = "pack-shared.dll";
        
        [DllImport(LibraryPath)] protected static extern PackResult createPackReader(string filePath, ref IntPtr packReader);
        [DllImport(LibraryPath)] protected static extern void destroyPackReader(IntPtr packReader);
        [DllImport(LibraryPath)] protected static extern ulong getPackItemCount(IntPtr packReader);
        [DllImport(LibraryPath)] protected static extern bool getPackItemIndex(IntPtr packReader, string path, ref ulong index);
        [DllImport(LibraryPath)] protected static extern uint getPackItemDataSize(IntPtr packReader, ulong index);
        [DllImport(LibraryPath)] protected static extern string getPackItemPath(IntPtr packReader, ulong index);
        [DllImport(LibraryPath)] protected static extern PackResult readPackItemData(IntPtr packReader, ulong index, ref uint size, ref IntPtr data);
        [DllImport(LibraryPath)] protected static extern void freePackReaderBuffers(IntPtr packReader);
        [DllImport(LibraryPath)] protected static extern PackResult unpackFiles(string filePath, ref ulong fileCount, bool printProgress);

        protected IntPtr Handle;

        public ulong ItemCount => getPackItemCount(Handle);

        public PackReader(string filePath)
        {
            var handle = IntPtr.Zero;
            var result = createPackReader(filePath, ref handle);

            if (result != PackResult.Success)
                throw new Exception(result.ToString());

            Handle = handle;
        }
        ~PackReader()
        {
            destroyPackReader(Handle);
        }

        public bool GetItemIndex(string path, ref ulong index)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            return getPackItemIndex(Handle, path, ref index);
        }
        public uint GetItemDataSize(ulong index)
        {
            if (index >= ItemCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return getPackItemDataSize(Handle, index);
        }
        public string GetItemPath(ulong index)
        {
            if (index >= ItemCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return getPackItemPath(Handle, index);
        }
        
        public PackResult ReadItemData(ulong index, ref uint size, ref IntPtr data)
        {
            if (index >= ItemCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return readPackItemData(Handle, index, ref size, ref data);
        }
        public PackResult ReadItemData(ulong index, ref byte[] data)
        {
            if (index >= ItemCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var size = uint.MinValue;
            var buffer = IntPtr.Zero;

            var result = readPackItemData(Handle, index, ref size, ref buffer);

            if (result != PackResult.Success)
                return result;

            if (size > int.MaxValue)
                throw new ApplicationException("Unsupported item data size");
            
            data = new byte[size];
            Marshal.Copy(buffer, data, 0, (int)size);
            return PackResult.Success;
        }
        public PackResult ReadItemData(ulong index, ref string data)
        {
            byte[] buffer = null;
            var result = ReadItemData(index, ref buffer);

            if (result != PackResult.Success)
                return result;

            data = Encoding.UTF8.GetString(buffer);
            return PackResult.Success;
        }

        public void FreeBuffers()
        {
            freePackReaderBuffers(Handle);
        }

        public static PackResult UnpackFiles(string filePath, ref ulong fileCount, bool printProgress)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            return unpackFiles(filePath, ref fileCount, printProgress);
        }
    }
}
