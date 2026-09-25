using Microsoft.CodeAnalysis;

namespace ExportGenerator;

public class MethodExport
{
	public IMethodSymbol MethodSymbol = null!;
	public AttributeData? ExportGenAttribute;

	public string EnumName(string @namespace)
	{
		string genMethodName = MethodSymbol.Name;
		if (ExportGenAttribute != null &&
			ExportGenAttribute.ConstructorArguments[0].Value is string str &&
			!string.IsNullOrEmpty(str))
		{
			genMethodName = str;
		}

		if (genMethodName.Contains(@namespace + "."))
			genMethodName = genMethodName.Replace(@namespace + ".", string.Empty);

		if (genMethodName.Contains("."))
			genMethodName = genMethodName.Replace(".", "_");

		return genMethodName;
	}
}
