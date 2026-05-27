using System.Text;

namespace ExportGenerator;

internal class GenerateExportedClass(StringBuilder sb, MethodExport export)
{
	private StringBuilder SB { get; } = sb;
	private MethodExport Method { get; } = export;


	public void WriteCallConv()
	{
		// Method.Attribute
	}
}
