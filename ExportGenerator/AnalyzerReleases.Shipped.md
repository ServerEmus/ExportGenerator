; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
EXGEN001  | ExportGen |  Error | ExportGen VTable off with no VTable creation
EXGEN002  | ExportGenMethod |  Error | Export Method returns type must be unmanaged
EXGEN003  | ExportGenMethod |  Error | Export Method needs attribute
EXGEN004  | ExportGen |  Error | Method must be static
EXGEN005  | ExportGenVTable |  Error | CreateVTable method parameter is not the same as the Create Method parameters
EXGEN006  | ExportGenVTable |  Error | CreateVTable method must have the first parameter as ref class
EXGEN007  | ExportGenFree |  Error | Free Method must be void
EXGEN008  | ExportGenFree |  Error | Free method must have the first parameter as ref class