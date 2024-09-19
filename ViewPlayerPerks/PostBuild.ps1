###
### Void Crew Modding PreBuild Script
### For use with VoidManager
###
### Written by Dragon of VoidCrewModdingTeam.
### Modified by: Mest
###
### Script Version 1.0.7
###
###
### This script was created for auto-generation/fill of release files for Void Crew mods.
###
###

param ($OutputDir, $SolutionDir)
$ConfigFilePath = "$PSScriptRoot\ReleaseFiles\ReleaseConfig.config"


### Simple INI reader. Credit to https://devblogs.microsoft.com/scripting/use-powershell-to-work-with-any-ini-file/
function Get-IniContent ($FilePath)
{
    $ini = @{}
    switch -regex -File $FilePath
    {
        “^\[(.+)\]” # Section
        {
            $section = $matches[1]
            $ini[$section] = @{}
            $CommentCount = 0
        }
        “^(;.*)$” # Comment
        {
            $value = $matches[1]
            $CommentCount = $CommentCount + 1
            $name = “Comment” + $CommentCount
            $ini[$section][$name] = $value
        }
        “(.+?)\s*=(.*)” # Key
        {
            $name,$value = $matches[1..2]
            $ini[$section][$name] = $value
        }
    }
    return $ini
}

$ConfigData = Get-IniContent($ConfigFilePath)



### Input Vars
# The name of the mod, used for AutoGUID, FileName, and anything else which needs a file-friendly name. Leave blank to use existing data.
$PluginName = $ConfigData["ReleaseProperties"]["PluginName"]

# The current version of the mod. Used for file version, BepinPlugin, and Thunderstore manifest.
$PluginVersion = $ConfigData["ReleaseProperties"]["PluginVersion"]

# Build Zip File
$BuildZip = $ConfigData["PrebuildExecParams"]["BuildZip"] -eq "True"


Write-Output "Starting Zip Operation"

$CSProjDir = (@(Get-ChildItem -Path ($PSScriptRoot + "\*.csproj"))[0])
$CSProjXML = [xml](Get-Content -Path $CSProjDir.FullName)

# Set AssemblyName
$AssemblyNameXMLNode = $CSProjXML.SelectSingleNode("//Project/PropertyGroup/AssemblyName")

# Autofill PluginName
if(-Not $PluginName) { $PluginName = $AssemblyNameXMLNode.InnerText }

## Zip files
if ($BuildZip -and $PluginVersion)
{
    Write-Output "Building Zips..."
    [System.IO.Directory]::CreateDirectory("$OutputDir\Releases")
    Compress-Archive -Path "$OutputDir\README.md", "$OutputDir\CHANGELOG.md", "$OutputDir\manifest.json", "$OutputDir$PluginName.dll"   -DestinationPath "$OutputDir\Releases\$PluginName-$PluginVersion.zip" -Force
    if(Test-Path "$OutputDir\Releases\icon.png")
    {
        Compress-Archive -Path "$OutputDir\icon.png" -DestinationPath "$OutputDir\Releases\$PluginName-$PluginVersion.zip" -Update
    }
}

Write-Host "Zip Operation Complete!"