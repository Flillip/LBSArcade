$testPath = ".\test_env"

# if folder already exists delete it.
if (Test-Path -Path $testPath) {
    Remove-Item -Path $testPath -Recurse
}

# make the directory
md $testPath

# copy all the files
Get-Item -Path ".\LBSArcade\bin\Debug\net6.0\*" | Copy-Item -Destination $testPath -Recurse
Get-Item -Path ".\OSHelper\bin\Debug\net6.0\*"  | Copy-Item -Destination $testPath

$batPath = "$($testPath)\start.bat"

# create a new bat file and fill it with a script
New-Item -Path $batPath
Set-Content $batPath "@echo OFF`r`nstart `"`" OSHelper.exe"
