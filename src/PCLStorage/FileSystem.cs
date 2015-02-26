using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCLStorage
{
    /// <summary>
    /// Provides access to an implementation of <see cref="IFileSystem"/> for the current platform
    /// </summary>
    public static class FileSystem
    {
#if WINDOWS_PHONE_7
        static IFileSystem _fileSystem;
#else
        static Lazy<IFileSystem> _fileSystem = new Lazy<IFileSystem>(() => CreateFileSystem(), System.Threading.LazyThreadSafetyMode.PublicationOnly); 
#endif

        /// <summary>
        /// The implementation of <see cref="IFileSystem"/> for the current platform
        /// </summary>
        public static IFileSystem Current
        {
            get
            {
#if WINDOWS_PHONE_7
                IFileSystem ret = _fileSystem ?? (_fileSystem = CreateFileSystem());
#else
                IFileSystem ret = _fileSystem.Value;
#endif
                if (ret == null)
                {
                    throw FileSystem.NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IFileSystem CreateFileSystem()
        {
#if NETFX_CORE || (WINDOWS_PHONE && !WINDOWS_PHONE_7)
			return new WinRTFileSystem();
#elif SILVERLIGHT
			return new IsoStoreFileSystem();
#elif FILE_SYSTEM
            return new DesktopFileSystem();
#else
            return null;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the PCLStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
