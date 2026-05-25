using Microsoft.CodeAnalysis;

namespace ExportGenerator;

public class MethodExport
{
	public IMethodSymbol MethodSymbol = null!;
	public AttributeData? Attribute;
}
