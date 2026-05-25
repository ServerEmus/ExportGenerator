using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Xml.Linq;

namespace ExportGenerator;

internal static class Helpers
{    
	// stable hashcode, 1:1 of Mirror's implementation.
	public static int GetStableHashCode(this string text)
	{
		unchecked
		{
			int hash = 23;
			foreach (char c in text)
				hash = hash * 31 + c;

			return hash;
		}
	}



	// gets the namespace from the symbol.
	public static string GetNameSpace(INamedTypeSymbol symbol)
	{
		List<string> names = [];
		INamespaceSymbol namespaceSymbol = symbol.ContainingNamespace;
		while (namespaceSymbol != null)
		{
			if (namespaceSymbol.Name.Contains("<global namespace>"))
				break;

			names.Add(namespaceSymbol.Name);
			namespaceSymbol = namespaceSymbol.ContainingNamespace;
		}

		// reverting so it will be Main.Sub instead of Sub.Main
		names.Reverse();

		string namespaceStr = string.Empty;

		if (names.Count != 0)
			namespaceStr = string.Join(".", names);

		if (names.Count > 1)
			namespaceStr = namespaceStr.Substring(1);

		return namespaceStr;
	}

	public static bool TryGetAttribute(this ISymbol symbol, string attributeName, out AttributeData? data)
	{
		data = symbol.GetAttributes().FirstOrDefault(attrib => attrib.AttributeClass?.Name == attributeName);
		return data != null;
	}

	public static bool HastAttribute(this ISymbol symbol, string attributeName)
	{
		return symbol.GetAttributes().Any(x => x.AttributeClass?.Name == attributeName);
	}

	public static void GenerateVTable(this INamedTypeSymbol symbol, List<MethodExport> methods, StringBuilder sb)
	{
		List<string> methodNames = [];
		methodNames.Clear();

		string @namespace = GetNameSpace(symbol);

		foreach (var method in methods)
		{
			string genMethodName = method.MethodSymbol.Name;
			if (method.Attribute != null &&
				method.Attribute.ConstructorArguments[0].Value is string str &&
				!string.IsNullOrEmpty(str))
			{
				genMethodName = str;
			}


			if (genMethodName.Contains(@namespace + "."))
				genMethodName = genMethodName.Replace(@namespace + ".", string.Empty);

			if (genMethodName.Contains("."))
				genMethodName = genMethodName.Replace(".", "_");

			methodNames.Add(genMethodName);
		}

		sb.AppendLine(
			$$"""
						[StructLayout(LayoutKind.Sequential, Pack = 8)]
						public struct {{symbol.Name}}_Table
						{
					{{string.Join("\n",
					methodNames.Select(s => string.Format("\t\tpublic nint {0};", s))
				)}}
						}
					""");
	}


}
