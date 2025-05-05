# Enable error handling
$ErrorActionPreference = "Stop"

# Parameters
$ProjectsToTestPattern = "*Tests/*.csproj"
$ExcludePattern = "*Integrat*Test*"
$TestResultsPattern = "**/TestResults/**/coverage.cobertura.xml"
$ReportDir = "tests/coverlet/reports"
$HtmlReportDir = "tests/reports"
$Threshold = 80  # Minimum coverage threshold you want

# Clean previous report folders
$FullReportDir = Join-Path $PWD $ReportDir
$FullHtmlReportDir = Join-Path $PWD $HtmlReportDir

if (Test-Path $FullReportDir) {
    Write-Host "Cleaning previous reports at $FullReportDir..."
    Remove-Item $FullReportDir -Recurse -Force
}
New-Item -ItemType Directory -Path $FullReportDir | Out-Null

if (Test-Path $FullHtmlReportDir) {
    Write-Host "Cleaning previous HTML reports at $FullHtmlReportDir..."
    Remove-Item $FullHtmlReportDir -Recurse -Force
}
New-Item -ItemType Directory -Path $FullHtmlReportDir | Out-Null

# Install ReportGenerator if not installed
if (-not (Test-Path ".tools/reportgenerator.exe")) {
    Write-Host "Installing ReportGenerator tool..."
    dotnet tool update --tool-path .tools dotnet-reportgenerator-globaltool --ignore-failed-sources
}

# Find test projects
Write-Host "Searching for test projects matching pattern: $ProjectsToTestPattern"
$TestProjects = Get-ChildItem -Recurse -Filter *.csproj |
    Where-Object {
        $_.FullName -like "*Tests*" -and
        $_.FullName -notlike $ExcludePattern
    }

if ($TestProjects.Count -eq 0) {
    Write-Error "No test projects found matching the pattern."
    exit 1
}

# Run tests with code coverage
foreach ($project in $TestProjects) {
    Write-Host "Running tests for project: $($project.FullName)"
    dotnet test $project.FullName  --settings tests/coverage.runsettings
}

# Generate merged code coverage reports
Write-Host "Generating merged Cobertura report..."
./.tools/reportgenerator -reports:"$TestResultsPattern" -targetdir:"$FullReportDir" -reporttypes:"Cobertura" "-assemblyfilters:-FSharp.Core;-Dapper*"

# Generate HTML reports
Write-Host "Generating HTML reports..."
./.tools/reportgenerator -reports:"$TestResultsPattern" -targetdir:"$FullHtmlReportDir" -reporttypes:"HTMLInline;HTMLChart"

# Also generate another Cobertura XML for other uses
Write-Host "Generating secondary Cobertura reports..."
./.tools/reportgenerator -reports:"$TestResultsPattern" -targetdir:"$FullHtmlReportDir" -reporttypes:"Cobertura"

Write-Host "✅ Done. Reports generated in '$HtmlReportDir' and '$ReportDir'."

# ======== Check and display total code coverage ==========

# Path to the merged Cobertura file
$MergedCoberturaReportPath = Join-Path $FullReportDir "Cobertura.xml"

if (-not (Test-Path $MergedCoberturaReportPath)) {
    Write-Error "Merged Cobertura report not found for coverage calculation."
    exit 1
}

# Load merged XML and extract line-rate
[xml]$CoverageXml = Get-Content $MergedCoberturaReportPath
[decimal]$Coverage = [decimal]$CoverageXml.coverage.'line-rate' * 100

# Compare against threshold
if ($Coverage -lt $Threshold) {
    Write-Error "Coverage ($Coverage%) is less than required threshold ($Threshold%)"
    exit 1
}
else {
    Write-Host ""
    Write-Host "TOTAL COVERAGE: $Coverage%" -ForegroundColor Green
    Write-Host ""
}
