$files = git diff --name-only --diff-filter=ACMRT | Where-Object { $_ -match '\.cs$' }
if ($files.Count -gt 0) {
    dotnet format --include @files -v diagnostic
} else {
    Write-Host "No C# files to format."
}
