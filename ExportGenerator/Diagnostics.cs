using Microsoft.CodeAnalysis;

namespace ExportGenerator;

public static class Diagnostics
{
	public static readonly DiagnosticDescriptor ExportGenVTableOffNoCustomVTable = new(
		id: "EXGEN001",
		title: "ExportGen VTable off with no VTable creation",
		messageFormat: "Export object '{0}' has VTable generation off, set it on or add ExportGenVTableCreate to custom method!",
		category: "ExportGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ExportMethodReturnNotUnmanaged = new(
		id: "EXGEN002",
		title: "Export Method returns type must be unmanaged",
		messageFormat: "Export method '{0}' has return type {1}, it should be unmanaged type!",
		category: "ExportGenMethod",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ExportMethodInterfaceImpNeedAttribute = new(
		id: "EXGEN003",
		title: "Export Method needs attribute",
		messageFormat: "Export method '{0}' must have a ExportGenMethodAttribute!",
		category: "ExportGenMethod",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor MustBeStatic = new(
		id: "EXGEN004",
		title: "Method must be static!",
		messageFormat: "Method '{0}' must be static!",
		category: "ExportGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor CreateVTableParamLength = new(
		id: "EXGEN005",
		title: "CreateVTable method parameter is not the same as the Create Method parameters",
		messageFormat: "CreateVTable method '{0}' parameter is not the same as the Create Method parameters ({1} , {2})",
		category: "ExportGenVTable",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor CreateVTableParamParamFirst = new(
		id: "EXGEN006",
		title: "CreateVTable method must have the first parameter as ref class",
		messageFormat: "CreateVTable method '{0}' must have the first parameter as ref class! ({1} , {2})",
		category: "ExportGenVTable",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor FreeMustBeVoid = new(
		id: "EXGEN007",
		title: "Free Method must be void",
		messageFormat: "Free Method '{0}' must be void!",
		category: "ExportGenFree",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor FreeMustHaveRefType = new(
		id: "EXGEN008",
		title: "Free method must have the first parameter as ref class",
		messageFormat: "Free method '{0}' must have the first parameter as ref class)! ({1} , {2})",
		category: "ExportGenFree",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);
}

