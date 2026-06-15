function Get-DocxText {
    param($Path)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($Path)
    $docXml = $zip.GetEntry("word/document.xml")
    $reader = New-Object System.IO.StreamReader($docXml.Open())
    $xmlStr = $reader.ReadToEnd()
    $reader.Close()
    $zip.Dispose()
    # Replace </w:p> with newline to preserve paragraph breaks
    $xmlStr = $xmlStr -replace '<w:p\b[^>]*>', "`n"
    $xmlStr = $xmlStr -replace '<[^>]+>', ''
    return $xmlStr.Trim()
}

$lore = Get-DocxText "d:\GitHub\silver-eagle\SE - LORE.docx"
$lore | Out-File "d:\GitHub\silver-eagle\lore.txt" -Encoding UTF8

$sectors = Get-DocxText "d:\GitHub\silver-eagle\Sectors.docx"
$sectors | Out-File "d:\GitHub\silver-eagle\sectors.txt" -Encoding UTF8

$gdd = Get-DocxText "d:\GitHub\silver-eagle\GDD SilverEagle.docx"
$gdd | Out-File "d:\GitHub\silver-eagle\gdd.txt" -Encoding UTF8
