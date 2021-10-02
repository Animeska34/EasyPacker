using System;
using System.Runtime.InteropServices;

namespace Pack
{
    public static class Common
    {
        private const string LibraryPath = "pack-shared.dll";
        
        [DllImport(LibraryPath)] private static extern void getPackLibraryVersion(ref byte majorVersion, ref byte minorVersion, ref byte patchVersion);
        [DllImport(LibraryPath)] private static extern PackResult getPackInfo(string filePath, ref byte majorVersion, ref byte minorVersion, ref byte patchVersion, ref bool isLittleEndian, ref ulong itemCount);

        public static void GetPackLibraryVersion(ref byte majorVersion, ref byte minorVersion, ref byte patchVersion)
        {
            getPackLibraryVersion(ref majorVersion, ref minorVersion, ref patchVersion);
        }
        public static PackResult GetPackInfo(string filePath, ref byte majorVersion, ref byte minorVersion, ref byte patchVersion, ref bool isLittleEndian, ref ulong itemCount)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            return getPackInfo(filePath, ref majorVersion, ref minorVersion, ref patchVersion, ref isLittleEndian, ref itemCount);
        }
    }
}
