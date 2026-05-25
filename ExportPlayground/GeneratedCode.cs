#if false

using System.Runtime.InteropServices;

namespace ExportPlayground;

/// <summary>
/// Indicate the target should generate a dllexport entry point.
/// </summary>
/// <param name="exportBaseName">The base name to export this class.</param>
/// <param name="convention">The calling convention to use.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class ExportGen(string exportBaseName = "", CallingConvention convention = CallingConvention.Winapi) : Attribute
{
	public string ExportBaseName { get; } = exportBaseName;
	public CallingConvention Convention { get; } = convention;
}

/// <summary>
/// Indicate the target has this method as constructor.
/// </summary>
/// <param name="exportName">The export name of this method.</param>
/// <param name="convention">The calling convention to use.</param>
/// <param name="includeBaseName">Should use the class base name with this class.</param>
/// <remarks>
/// ExportName and ExportBaseName will look like: ExportBaseName_ExportName.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ExportGenCreate(string exportName = "Create", CallingConvention convention = CallingConvention.Winapi, bool includeBaseName = true, string exportBaseName = "") : ExportGenMethod(exportName, convention, includeBaseName)
{
	public string ExportBaseName { get; } = exportBaseName;
}

/// <summary>
/// Indicate the target has this method as free (clearing the memory).
/// </summary>
/// <param name="exportName">The export name of this method.</param>
/// <param name="convention">The calling convention to use.</param>
/// <param name="includeBaseName">Should use the class base name with this class.</param>
/// <remarks>
/// ExportName and ExportBaseName will look like: ExportBaseName_ExportName.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ExportGenFree(string exportName = "Free", CallingConvention convention = CallingConvention.Winapi, bool includeBaseName = true, string exportBaseName = "") : ExportGenMethod(exportName, convention, includeBaseName)
{
	public string ExportBaseName { get; } = exportBaseName;
}

/// <summary>
/// Indicate the method should generate a dllexport entry point.
/// </summary>
/// <param name="exportName">The export name of this method.</param>
/// <param name="convention">The calling convention to use.</param>
/// <param name="includeBaseName">Should use the class base name with this class.</param>
/// <remarks>
/// ExportName and ExportBaseName will look like: ExportBaseName_ExportName.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ExportGenMethod(string exportName, CallingConvention convention = CallingConvention.Winapi, bool includeBaseName = true) : Attribute
{
	public string ExportName { get; } = exportName;
	public CallingConvention Convention { get; } = convention;
	public bool IncludeBaseName { get; } = includeBaseName;
}

/// <summary>
/// Indicate the method should be ignored when generation happened.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ExportGenIgnore() : Attribute;

/// <summary>
/// Indicate the target should generate a vtable.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class ExportGenVTable(string exportBaseName = "") : Attribute
{
	public string ExportBaseName { get; } = exportBaseName;
}

/// <summary>
/// Indicate the target should generate a vtable.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ExportGenVTableCreate(string createMethodName = "", params string[] vtables) : Attribute;

/// <summary>
/// Custom virtual table.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct CustomVTable
{
	/// <summary>
	/// The pointer to the real vtable.
	/// </summary>
	public nint VTablePointer;

	/// <summary>
	/// Hash of the generated object.
	/// </summary>
	public int Hash;
}

internal static class Pointers
{
	/// <summary>
	/// Allocate <typeparamref name="T"/> and parse the structure to the pointer.
	/// </summary>
	/// <typeparam name="T">Any unmanaged type.</typeparam>
	/// <param name="unmanaged_t">The unmanaged object.</param>
	/// <returns>The pointer to the object.</returns>
	public static unsafe nint ToIntPtr<T>(this T unmanaged_t) where T : unmanaged
	{
		nint allocatedPointer = Marshal.AllocHGlobal(sizeof(T));
		T* typeTPointer = (T*)allocatedPointer;
		*typeTPointer = unmanaged_t;
		return allocatedPointer;
	}
}
#endif