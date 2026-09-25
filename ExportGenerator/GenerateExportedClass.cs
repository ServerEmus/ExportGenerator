using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace ExportGenerator;

internal readonly struct GenerateExportedClass
{
	public GenerateExportedClass(string baseName, SourceProductionContext context, StringBuilder sb, MethodExport export)
	{
		BaseName = baseName;
		Context = context;
		SB = sb;
		Method = export;
	}

	public GenerateExportedClass(string baseName, SourceProductionContext context, StringBuilder sb, IMethodSymbol symbol)
	{
		BaseName = baseName;
		Context = context;
		SB = sb;

		symbol.TryGetAttribute("ExportGenCreate", out AttributeData? data);

		if (data == null)
			symbol.TryGetAttribute("ExportGenFree", out data);

		if (data == null)
			symbol.TryGetAttribute("ExportGenVTableCreate", out data);

		if (data == null)
			symbol.TryGetAttribute("ExportGenMethod", out data);

		Method = new()
		{
			MethodSymbol = symbol,
			ExportGenAttribute = data,
		};
	}

	private SourceProductionContext Context { get; }
	private StringBuilder SB { get; }
	private MethodExport Method { get; }
	private string BaseName { get; }

	public void Generate()
	{
		SB.AppendLine();
		SB.Append("\n\t[UnmanagedCallersOnly(EntryPoint = \"");
		WriteExportName();
		WriteCallConvEnum();
		SB.Append("\")]");

		SB.Append("\n\tpublic static ");
		WriteReturnType();
		SB.Append(" ");
		WriteExportName(true);
		SB.Append("(IntPtr pThis");
		WriteAdditionalArguments();
		SB.Append(")");

		SB.AppendLine("\n\t{");

		SB.Append("\n\t\tif (!PointerToCreatedClass.TryGetValue(pThis, out var genClass))");
		SB.Append("\n\t\t\treturn");

		if (!Method.MethodSymbol.ReturnsVoid)
		{
			SB.Append(" default");
		}
		SB.Append(";");

		SB.AppendLine();

		SB.Append("\n\t\t");
		if (!Method.MethodSymbol.ReturnsVoid)
		{
			SB.Append("return ");
		}
		WriteMethodNameCall();
		WriteCallerArg();

		SB.AppendLine("\n\t}");
	}

	public void GenerateCreate(IMethodSymbol? createVTableSymbol)
	{
		if (!Method.MethodSymbol.IsStatic)
		{
			Context.ReportDiagnostic(
				Diagnostic.Create(Diagnostics.MustBeStatic, Method.MethodSymbol.Locations[0], [Method.MethodSymbol.Name])
			);
			return;
		}


		if (createVTableSymbol != null)
		{
			if (!createVTableSymbol.IsStatic)
			{
				Context.ReportDiagnostic(
					Diagnostic.Create(Diagnostics.MustBeStatic, createVTableSymbol.Locations[0], [createVTableSymbol.Name])
				);
				return;
			}

			if (createVTableSymbol.Parameters.Length != (Method.MethodSymbol.Parameters.Length + 1))
			{
				Context.ReportDiagnostic(
					Diagnostic.Create(Diagnostics.CreateVTableParamLength, createVTableSymbol.Locations[0], [
						createVTableSymbol.Name, createVTableSymbol.Parameters.Length, Method.MethodSymbol.Parameters.Length + 1
						])
				);
				return;
			}

			if (createVTableSymbol.Parameters[0].RefKind != RefKind.Ref || !createVTableSymbol.Parameters[0].Type.Equals(Method.MethodSymbol.ReturnType, SymbolEqualityComparer.Default))
			{
				Context.ReportDiagnostic(
					Diagnostic.Create(Diagnostics.CreateVTableParamParamFirst, createVTableSymbol.Locations[0], [
						createVTableSymbol.Name,
						createVTableSymbol.Parameters[0].Type,
						Method.MethodSymbol.ReturnType
						])
				);
				return;
			}
		}

		SB.AppendLine();
		SB.Append("\n\t[UnmanagedCallersOnly(EntryPoint = \"");
		WriteExportName();
		WriteCallConvEnum();
		SB.Append("\")]");

		SB.Append("\n\tpublic static IntPtr ");
		WriteExportName(true);
		SB.Append("(");
		WriteAdditionalArguments(false);
		SB.Append(")");

		SB.AppendLine("\n\t{");

		SB.Append($"\t\t{Method.MethodSymbol.ContainingType.Name} newClass = {Method.MethodSymbol.Name}");
		WriteCallerArg();

		SB.Append("\n\t\tIntPtr vtablePtr = ");

		if (createVTableSymbol == null)
		{
			SB.Append("_vtable.ToIntPtr();");
		}
		else
		{
			SB.Append(createVTableSymbol.Name);
			SB.Append("(ref newClass, ");
			WriteCallerArg(false);
		}

		SB.Append("\n\t\tCustomVTable export = new()");
		SB.Append("\n\t\t{");
		SB.Append("\n\t\t\tVTablePointer = vtablePtr,");
		SB.Append("\n\t\t\tHash = GeneratedHash,");
		SB.Append("\n\t\t};");
		SB.Append("\n\t\tIntPtr pThis = export.ToIntPtr();");
		SB.AppendLine();
		SB.Append("\n\t\tPointerToCreatedClass[pThis] = newClass;");
		SB.Append("\n\t\treturn pThis;");

		SB.AppendLine("\n\t}");
	}

	public void GenerateFree()
	{
		if (!Method.MethodSymbol.IsStatic)
		{
			Context.ReportDiagnostic(
				Diagnostic.Create(Diagnostics.MustBeStatic, Method.MethodSymbol.Locations[0], [Method.MethodSymbol.Name])
			);
			return;
		}

		if (!Method.MethodSymbol.ReturnsVoid)
		{
			Context.ReportDiagnostic(
				Diagnostic.Create(Diagnostics.FreeMustBeVoid, Method.MethodSymbol.Locations[0], [Method.MethodSymbol.Name])
			);
			return;
		}

		if (Method.MethodSymbol.Parameters[0].RefKind != RefKind.Ref || !Method.MethodSymbol.Parameters[0].Type.Equals(Method.MethodSymbol.ContainingType, SymbolEqualityComparer.Default))
		{
			Context.ReportDiagnostic(
				Diagnostic.Create(Diagnostics.CreateVTableParamParamFirst, Method.MethodSymbol.Locations[0], [
					Method.MethodSymbol.Name,
					Method.MethodSymbol.Parameters[0].Type,
					Method.MethodSymbol.ContainingType
					])
			);
			return;
		}

		SB.AppendLine();
		SB.Append("\n\t[UnmanagedCallersOnly(EntryPoint = \"");
		WriteExportName();
		WriteCallConvEnum();
		SB.Append("\")]");

		SB.Append("\n\tpublic static void ");
		WriteExportName(true);
		SB.Append("(IntPtr pThis)");
		SB.AppendLine("\n\t{");

		SB.Append("\t\tMarshal.FreeHGlobal(pThis);");
		SB.Append("\n\t\tif (!PointerToCreatedClass.Remove(pThis, out var exportCustom))");
		SB.Append("\n\t\t\treturn;");

		SB.Append("\n\t\t");
		SB.Append(Method.MethodSymbol.Name);
		SB.Append("(ref exportCustom);");

		SB.AppendLine("\n\t}");
	}

	private readonly bool TryGenMethod(out AttributeData? data)
	{
		data = Method.ExportGenAttribute;
		return data != null;
	}

	public readonly void WriteMethodNameCall()
	{
		if (Method.MethodSymbol.IsStatic)
		{
			SB.Append(Method.MethodSymbol.Name);
			return;
		}


		if (Method.MethodSymbol.MethodKind != MethodKind.ExplicitInterfaceImplementation)
		{
			SB.Append("genClass.");
			SB.Append(Method.MethodSymbol.Name);
			return;
		}

		IMethodSymbol interfaceMethod = Method.MethodSymbol.ExplicitInterfaceImplementations[0];
		string interfaceName = interfaceMethod.ContainingType.Name;
		string methodName = interfaceMethod.Name;

		SB.Append($"(({interfaceName})genClass).{methodName}");
	}

	public readonly void WriteExportName(bool asExportMethod = false)
	{
		string exportName = $"{(asExportMethod ? "EXPORT_" : string.Empty)}{BaseName}_{Method.MethodSymbol.Name}";
		if (!TryGenMethod(out AttributeData? attribute) || attribute == null)
		{
			SB.Append(exportName);
			return;
		}

		if (attribute.ConstructorArguments[0].Value is not string exportNameCtor)
			return;

		if (attribute.ConstructorArguments[2].Value is not bool includeBaseName)
			return;

		if (string.IsNullOrEmpty(exportNameCtor))
		{
			exportNameCtor = exportName;
		}
		else if (includeBaseName)
		{
			exportNameCtor = $"{(asExportMethod ? "EXPORT_" : string.Empty)}{BaseName}_{exportNameCtor}";
		}

		SB.Append(exportNameCtor);
	}

	public readonly void WriteCallConv()
	{
		if (!TryGenMethod(out AttributeData? attribute) || attribute == null)
			return;

		if (attribute.ConstructorArguments[1].Value is not CallingConvention conv)
			return;

		switch (conv)
		{
			case CallingConvention.Cdecl:
				SB.Append("[Cdecl]");
				break;
			case CallingConvention.FastCall:
				SB.Append("[FastCall]");
				break;
			case CallingConvention.StdCall:
				SB.Append("[Stdcall]");
				break;
			case CallingConvention.ThisCall:
				SB.Append("[Thiscall]");
				break;
			case CallingConvention.Winapi:
				SB.Append("[Stdcall]");
				break;
			default:
				break;
		}
	}

	public readonly void WriteArgument()
	{
		string lastReturn = "void";
		if (!Method.MethodSymbol.ReturnsVoid)
		{
			if (!Method.MethodSymbol.ReturnType.IsUnmanagedType)
			{
				Context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ExportMethodReturnNotUnmanaged, Method.MethodSymbol.Locations[0], [Method.MethodSymbol.Name, Method.MethodSymbol.ReturnType]));
				return;
			}

			lastReturn = Method.MethodSymbol.ReturnType.ToString();
		}

		SB.Append("nint, ");
		int len = Method.MethodSymbol.Parameters.Length;
		if (len != 0)
		{
			for (int i = 0; i < len; i++)
			{
				var argType = Method.MethodSymbol.Parameters[i];
				SB.Append(argType.Type.Name);
				SB.Append(", ");
			}
		}

		SB.Append(lastReturn);
	}

	public readonly void WriteReturnType()
	{
		string lastReturn = "void";
		if (!Method.MethodSymbol.ReturnsVoid)
		{
			if (!Method.MethodSymbol.ReturnType.IsUnmanagedType)
			{
				Context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ExportMethodReturnNotUnmanaged, Method.MethodSymbol.Locations[0], [Method.MethodSymbol.Name, Method.MethodSymbol.ReturnType]));
				return;
			}

			lastReturn = Method.MethodSymbol.ReturnType.ToString();
		}

		SB.Append(lastReturn);
	}

	public readonly void WriteAdditionalArguments(bool usePre = true)
	{
		int len = Method.MethodSymbol.Parameters.Length;
		if (len == 0)
			return;

		if (usePre)
			SB.Append(", ");

		for (int i = 0; i < len; i++)
		{
			var argType = Method.MethodSymbol.Parameters[i];
			SB.Append(argType.Type.Name);
			SB.Append($" {argType.Name}");

			if (i + 1 < len)
				SB.Append(", ");
		}
	}

	public readonly void WriteCallerArg(bool writeEntereing = true)
	{
		if (writeEntereing)
			SB.Append("(");

		int len = Method.MethodSymbol.Parameters.Length;
		if (len == 0)
		{
			SB.Append(");");
			return;
		}

		for (int i = 0; i < len; i++)
		{
			var argType = Method.MethodSymbol.Parameters[i];
			if (argType.RefKind == RefKind.Ref)
			{
				SB.Append("ref ");
			}
			if (argType.RefKind == RefKind.In)
			{
				SB.Append("in ");
			}

			SB.Append($"{argType.Name}");

			if (i + 1 < len)
				SB.Append(", ");
		}


		SB.Append(");");
	}

	public readonly void WriteCallConvEnum()
	{
		if (!TryGenMethod(out AttributeData? attribute) || attribute == null)
			return;

		if (attribute.ConstructorArguments[1].Value is not CallingConvention conv)
			return;

		switch (conv)
		{
			case CallingConvention.Cdecl:
				SB.Append(", CallConvs = [typeof(CallConvCdecl)]");
				break;
			case CallingConvention.FastCall:
				SB.Append(", CallConvs = [typeof(CallConvFastCall)]");
				break;
			case CallingConvention.StdCall:
				SB.Append(", CallConvs = [typeof(CallConvStdcall)]");
				SB.Append("[Stdcall]");
				break;
			case CallingConvention.ThisCall:
				SB.Append(", CallConvs = [typeof(CallConvThiscall)]");
				break;
			case CallingConvention.Winapi:
				SB.Append(", CallConvs = [typeof(CallConvStdcall)]");
				break;
			default:
				break;
		}
	}
}
