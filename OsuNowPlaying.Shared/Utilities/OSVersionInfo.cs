using System.Runtime.InteropServices;

namespace OsuNowPlaying.Shared.Utilities
{
	public class OSVersionInfo
	{
		// https://pinvoke.net/default.aspx/ntdll/RtlGetVersion.html

		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int RtlGetVersion(ref OSVERSIONINFOEXW versionInfo);

		private enum PRODUCT_TYPE
		{
			VER_NT_WORKSTATION = 0x0000001,
			VER_NT_DOMAIN_CONTROLLER = 0x0000002,
			VER_NT_SERVER = 0x0000003
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct OSVERSIONINFOEXW
		{
			public int dwOSVersionInfoSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;

			public uint wServicePackMajor;
			public uint wServicePackMinor;
			public uint wSuiteMask;
			public PRODUCT_TYPE wProductType;
			public byte wReserved;
		}

		private static OSVERSIONINFOEXW _versionInfo;
		private static bool _isLoaded;

		private static OSVERSIONINFOEXW VersionInfo
		{
			get
			{
				if (!_isLoaded)
				{
					_versionInfo = new OSVERSIONINFOEXW
					{
						dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEXW))
					};

					RtlGetVersion(ref _versionInfo);
					_isLoaded = true;
				}

				return _versionInfo;
			}
		}

		public static int MajorVersion => VersionInfo.dwMajorVersion;
		public static int MinorVersion => VersionInfo.dwMinorVersion;
	}
}