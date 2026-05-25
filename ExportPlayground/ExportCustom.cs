using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ExportPlayground;

// Generate export with all public class, prefixed as "ExportCustom_"
[ExportGen] 
public partial class ExportCustom
{
	private bool isInitialied = false;
	private int runTime = 0;
	public int MagicId { get; init; }
	public void Start()
	{
		if (isInitialied)
		{
			//Console.WriteLine("Already initialized!");
			return;
		}

		isInitialied = true;
		runTime = 0;
	}

	public void Stop()
	{
		if (!isInitialied)
			return;

		isInitialied = false;
		runTime = 0;
	}

	public void Run()
	{
		if (!isInitialied)
			return;

		runTime++;
	}

	public int GetRunTime()
	{
		return runTime;
	}

	public int GetId()
	{
		if (!isInitialied || runTime == 0)
			return 0;

		return Random.Shared.Next(0, runTime + 1);
	}

	public int GetMagic()
	{
		return MagicId;
	}

	[ExportGenIgnore]
	public void IgnoreMe()
	{

	}

	public static void StaticTest()
	{

	}

	[ExportGenCreate]
	public static ExportCustom CreateExportCustom(int magic)
	{
		return new()
		{ 
			MagicId = magic,
		};
	}

	[ExportGenFree]
	public static void DestroyExportCustom(ref ExportCustom? exportCustom)
	{
		exportCustom?.Stop();
		exportCustom = null;
	}
}

// This should be what generated
#if false
public partial class ExportCustom
{
	public const int GeneratedHash = 346513646;

	// VTable since this is required for us.
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ExportCustomVTable
	{
		public nint Start;
		public nint Stop;
		public nint Run;
		public nint GetRunTime;
		public nint GetId;
		public nint GetMagic;
		public nint StaticTest;
	}

	public enum ExportCustomFunctions
	{
		Start,
		Stop,
		Run,
		GetRunTime,
		GetId,
		GetMagic,
		StaticTest,
		MAX
	}

	private static readonly Dictionary<IntPtr, ExportCustom> PointerToCreatedClass = [];
	private static readonly nint[] FunctionPointers = new nint[(int)ExportCustomFunctions.MAX];
	private static ExportCustomVTable _vtable;

	static ExportCustom()
	{
		unsafe
		{
			FunctionPointers[(int)ExportCustomFunctions.Start] = (nint)(delegate* unmanaged /*[Cdecl] */<nint, void>)&EXPORT_Start;
		}

		_vtable = new()
		{
			Start = FunctionPointers[(int)ExportCustomFunctions.Start]
		};
	}

	[UnmanagedCallersOnly(EntryPoint = "ExportCustom_Create", CallConvs = [typeof(CallConvCdecl)])]
	public static IntPtr EXPORT_ExportCustom_Create(int magic)
	{
		// alloc new class
		ExportCustom newClass = CreateExportCustom(magic);

		// create vtable and make it a pointer.
		CustomVTable export = new()
		{
			VTablePointer = _vtable.ToIntPtr(),
			Hash = GeneratedHash,
		};
		IntPtr pThis = export.ToIntPtr();

		PointerToCreatedClass[pThis] = newClass;
		//Console.WriteLine("Create with: {0}. HashCode: {1}, PThis: {2}", magic, hashcode, pThis);
		return pThis;
	}

	[UnmanagedCallersOnly(EntryPoint = "ExportCustom_Free", CallConvs = [typeof(CallConvCdecl)])]
	public static void EXPORT_ExportCustom_Free(IntPtr pThis)
	{
		//Console.WriteLine("Free called with pointer: {0}", pThis);
		if (!PointerToCreatedClass.TryGetValue(pThis, out ExportCustom? exportCustom))
		{
			//Console.WriteLine("Class could not found!");
			return;
		}

		DestroyExportCustom(ref exportCustom);

		PointerToCreatedClass.Remove(pThis);
		Marshal.FreeHGlobal(pThis);
	}


	[UnmanagedCallersOnly(EntryPoint = "ExportCustom_Start")]
	public static void EXPORT_Start(IntPtr pThis)
	{
		//Console.WriteLine("Start called with pointer: {0}", pThis);
		if (!PointerToCreatedClass.TryGetValue(pThis, out ExportCustom? exportCustom))
			return;

		exportCustom.Start();
	}
}
#endif