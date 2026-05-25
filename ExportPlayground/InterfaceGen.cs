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

public partial struct StructTest : ITestV1
{
	public int Version() => throw new NotImplementedException();

	public void Test()
	{
		throw new NotImplementedException();
	}

	public static StructTest CreateSturct()
	{
		return new();
	}
}

[ExportGen("ITest")]
public partial class Test(int @version) : ITestV1, ITestV2
{
	private readonly int _version = @version;

	public int Version() => _version;

	public void Test2()
	{
		Console.WriteLine("test2 call");
	}

	void ITestV1.Test()
	{
		Console.WriteLine("v1 test call");
	}

	void ITestV2.Test()
	{
		Console.WriteLine("v2 test call");
	}

	[ExportGenCreate("Create")]
	public static Test CreateTest(int version)
	{
		return new Test(version);
	}

	[ExportGenVTableCreate(nameof(CreateTest))]
	public static IntPtr CreateVTable(int version)
	{
		switch (version)
		{
			default:
				break;
			case 1:
				return IntPtr.Zero;
		}
		return IntPtr.Zero;
	}
}

#if false

public partial class Test
{
	public const int GeneratedHash = 254345;

	public enum ExportCustomFunctions
	{
		Version,
		ITestV1_Test,
		ITestV2_Test,
		Test2,
		MAX
	}

	private static readonly Dictionary<IntPtr, Test> PointerToCreatedClass = [];
	private static readonly nint[] FunctionPointers = new nint[(int)ExportCustomFunctions.MAX];

	static Test()
	{
		unsafe
		{
			// FunctionPointers[(int)ExportCustomFunctions.Version] = (nint)(delegate* unmanaged /*[Cdecl] */<nint, void>)&EXPORT_Start;
		}
	}

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