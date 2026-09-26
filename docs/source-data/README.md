# Local source data

Run `tools/extract_workbook.py` with Python and `openpyxl` to create the ignored reference JSON. Set `ConnectionStrings__SqlServer`, then run `dotnet run --project tools/KarimDoors.WorkbookImport` from the repository root. The importer applies migrations, validates historical prices and is safe to rerun.

Redcon D29 and D029 are one model, stored as `RED-D29-1050`. FD60 and FD90 models remain inactive because their detailed source sheets are absent. Imported prices are historical references.

Place the original pricing/breakdown spreadsheets in this folder on your development machine.

Excel files in this directory are intentionally ignored by Git because the GitHub repository is public and the source files may contain confidential business pricing information.
