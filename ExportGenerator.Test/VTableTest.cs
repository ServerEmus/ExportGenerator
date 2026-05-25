using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ExportGenerator.Test;

public class VTableTest
{
	[Fact]
	public void GeneratorExpected()
	{
		CSharpGeneratorDriver driver = 
		CSharpGeneratorDriver.Create([
			new AttributesGenerator(),
			new ExportGenVTableGenerator(),
		]);

		Compilation compilation = CSharpCompilation.Create(nameof(VTableTest),
			[CSharpSyntaxTree.ParseText(VTableTestConst.SourceText, cancellationToken: TestContext.Current.CancellationToken)],
			[
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
			]);

		// Run generators and retrieve all results.
		GeneratorDriverRunResult runResult = driver.RunGenerators(compilation, TestContext.Current.CancellationToken).GetRunResult();

#if DEBUG
		Directory.CreateDirectory("Generated");
		foreach (var item in runResult.GeneratedTrees)
		{
			string Input = item.GetText(TestContext.Current.CancellationToken).ToString();
			var filepath = item.FilePath.Split('\\');
			var path = filepath[filepath.Length - 1];


			File.WriteAllText($"Generated/{(path)}.generated.txt", Input);
		}
#endif


		foreach (var expectedPairs in VTableTestConst.ExpectedDictFiles)
		{
			SyntaxTree generatedFileSyntax = runResult.GeneratedTrees.Single(t =>
			{
				var filepath = t.FilePath.Split('\\');
				var path = filepath[filepath.Length - 1];
				return path == expectedPairs.Key;
			});
			string Input = generatedFileSyntax.GetText(TestContext.Current.CancellationToken).ToString().Replace("    ", "\t");
			string expected = expectedPairs.Value.Replace("    ", "\t");
#if DEBUG
			File.WriteAllText($"Generated/{expectedPairs.Key}.input.txt", Input);
			File.WriteAllText($"Generated/{expectedPairs.Key}.expected.txt", expected);
#endif
			Assert.Equal(expected, Input, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
		}

	}
}
