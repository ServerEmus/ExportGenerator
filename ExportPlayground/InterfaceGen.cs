using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ExportPlayground;

[ExportGenVTable]
internal interface ITestV1
{
	public int Version();

	public void Test();
}

[ExportGenVTable]
internal interface ITestV2
{
	public int Version();
	public void Test();

	public void Test2();
}

[ExportGen("ITest", exportPublicMethods: true, useVTable: false)]
public partial class Test(int @version) : ITestV1, ITestV2
{
	private readonly int _version = @version;

	public int Version() => _version;

	public void Test2()
	{
		Console.WriteLine("test2 call");
	}

	[ExportGenMethod("Test")]
	void ITestV1.Test()
	{
		Console.WriteLine("v1 test call");
	}

	[ExportGenMethod("TestV2")]
	void ITestV2.Test()
	{
		Console.WriteLine("v2 test call");
	}

	public void ArgTest(int arg1)
	{

	}

	public void Arg2(int arg1, int arg2, int arg3)
	{

	}

	[ExportGenCreate]
	public static Test CreateTest(int version)
	{
		return new Test(version);
	}

	[ExportGenVTableCreate]
	public static unsafe IntPtr CreateVTable(ref Test test, int version)
	{
		switch (version)
		{
			default:
				break;
			case 1:
				return new VTables.ITestV1_VTable()
				{ 
					Version = (nint)(delegate* unmanaged<nint, int>)&EXPORT_ITest_Version,
					Test = (nint)(delegate* unmanaged<nint, void>)&EXPORT_ITest_Test,
				}.ToIntPtr();
			case 2:
				return new VTables.ITestV2_VTable()
				{
					Version = (nint)(delegate* unmanaged<nint, int>)&EXPORT_ITest_Version,
					Test = (nint)(delegate* unmanaged<nint, void>)&EXPORT_ITest_TestV2,
					Test2 = (nint)(delegate* unmanaged<nint, void>)&EXPORT_ITest_Test2,
				}.ToIntPtr();
		}
		return IntPtr.Zero;
	}

	[ExportGenFree]
	public static void Free(ref Test test)
	{

	}

	[ExportGenIgnore]
	public void Balls()
	{

	}
}

#if false

public partial class Test
{
	public const int GeneratedHash = 254345;

	private static readonly Dictionary<IntPtr, Test> PointerToCreatedClass = [];

	[UnmanagedCallersOnly(EntryPoint = "Test_Create", CallConvs = [typeof(CallConvCdecl)])]
	public static IntPtr EXPORT_Test_Create(int version)
	{
		// alloc new class
		Test newClass = CreateTest(version);
		IntPtr vtable = CreateVTable(version);

		// create vtable and make it a pointer.
		CustomVTable export = new()
		{
			VTablePointer = vtable,
			Hash = GeneratedHash,
		};


		return export.ToIntPtr();
	}
}

public static partial class VTables
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ITestV1_VTable
	{
		public nint Version;
		public nint Test;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ITestV2_VTable
	{
		public nint Version;
		public nint Test;
		public nint Test2;
	}
}

#endif