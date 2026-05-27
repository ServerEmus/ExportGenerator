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
}
