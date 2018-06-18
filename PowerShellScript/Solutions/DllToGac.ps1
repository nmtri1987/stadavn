#read files into array?
function Add-GacItem([string]$path) {
  Begin {
    $gacutil="$env:ProgramFiles\Microsoft SDKs\Windows\v7.0A\bin\gacutil.exe"

    function AddGacItemImpl([string]$path) {
      "& $gacutil /nologo /i $path"
    }
  }
  Process {
    if ($_) { AddGacItemImpl $_ }
  }
  End {
    if ($path) { AddGacItemImpl $path }
  }
}


foreach ($file in Get-ChildItem -Filter "*.dll" )
{ 
  Add-GacItem $file.Name
}

#Get-Content .\dlls.txt | Split-String | Add-GacItem