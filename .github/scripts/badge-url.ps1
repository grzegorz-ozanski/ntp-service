# This script generates a badge URL for unit test results from a .trx file.
param (
  [Parameter(Position=0, Mandatory=$true)][string]$TrxPath
)

[xml]$xml = Get-Content $TrxPath
$namespaceManager = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
$namespaceManager.AddNamespace("t", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")

$counters = $xml.SelectSingleNode("//t:TestRun/t:ResultSummary/t:Counters", $namespaceManager)
$color = if ([int]$counters.failed -eq 0) { "brightgreen" } else { "red" }

"https://img.shields.io/badge/Unit%20tests-Passed%3A$($counters.passed)%20Failed%3A$($counters.failed)-${color}"
