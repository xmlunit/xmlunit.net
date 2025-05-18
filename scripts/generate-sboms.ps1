# Requires -Version 7

param(
    [Parameter(Mandatory = $true)]
    [string]
    $Version
)

function Generate-SBom {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $Version,

        [Parameter(Mandatory = $true)]
        [string]
        $Path,

        [Parameter(Mandatory = $true)]
        [string]
        $Name,

        [switch]
        $Json,

        [switch]
        $PostProcess
    )

    $targetDir = Join-Path -Path build -ChildPath cyclonedx
    if (-not(Test-Path $targetDir -PathType Container)) {
        New-Item -Path $targetDir -ItemType Directory
    }
    $targetDir = Resolve-Path -Path $targetDir
    $fileName = $JSon ? "$Name.cylconedx.json" : "$Name.cylconedx.xml"
    $fullFileName = Join-Path -Path $targetDir -ChildPath $fileName

    $Args = @(
        "./src/main/$Path/$Name.csproj"
        "-ipr"
        "-o"
        "$targetDir"
        "-sv"
        "$Version"
        "-st"
        "library"
        "-sn"
        "$Name"
        "--set-nuget-purl"
        "-imp"
        "./src/shared/cyclonedx-metadata.xml"
        "-fn"
        "$fileName"
    )

    Write-Output "Generating $fullFileName"
    if ($Json) {
        dotnet-CycloneDX @Args -j
    } else {
        dotnet-CycloneDX @Args
    }

    if ($PostProcess) {
        Write-Output "Post-Processing $fullFileName"
        if ($Json) {
            $body = Get-Content $fullFileName -Raw | ConvertFrom-Json -Depth 32
            $body.dependencies |
              ForEach-Object {
                  if ($_.ref -eq "xmlunit-core@1.0.0") {
                      $_.ref = "pkg:nuget/XMLUnit.Core@$Version"
                  }
                  if ($_.dependsOn -and $_.dependsOn.Contains("xmlunit-core@1.0.0")) {
                      $_.dependsOn = $_.dependsOn.Replace("xmlunit-core@1.0.0",
                                                          "pkg:nuget/XMLUnit.Core@$Version")
                  }
              }
            $body.components |
              ForEach-Object {
                  if ($_.name -eq "xmlunit-core") {
                      $_.name = "XMLUnit.Core"
                      $_."bom-ref" = "pkg:nuget/XMLUnit.Core@$Version"
                      $_.version = "$Version"
                  }
              }
            $body | ConvertTo-Json -Depth 32 | Set-Content $fullFileName
        } else {
            $body = [xml](Get-Content $fullFileName)
            $body.bom.dependencies.dependency |
              ForEach-Object {
                  if ($_.HasAttribute("ref") -and $_.Attributes["ref"].Value -eq "xmlunit-core@1.0.0") {
                      $_.Attributes["ref"].Value = "pkg:nuget/XMLUnit.Core@$Version"
                  }
                  if ($_.dependency) {
                      $_.dependency |
                        ForEach-Object {
                            if ($_.HasAttribute("ref") -and $_.Attributes["ref"].Value -eq "xmlunit-core@1.0.0") {
                                $_.Attributes["ref"].Value = "pkg:nuget/XMLUnit.Core@$Version"
                            }
                        }
                  }
              }
            $body.bom.components.component |
              ForEach-Object {
                  if ($_.HasAttribute("bom-ref") -and $_.Attributes["bom-ref"].Value -eq "xmlunit-core@1.0.0") {
                      $_.Attributes["bom-ref"].Value = "pkg:nuget/XMLUnit.Core@$Version"
                      $_.name = "XMLUnit.Core"
                      $_.version = "$Version"
                  }
              }
            $body.Save($fullFileName)
        }            
    }
}

function Generate-SBoms {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $Version,

        [Parameter(Mandatory = $true)]
        [string]
        $Path,

        [Parameter(Mandatory = $true)]
        [string]
        $Name,

        [switch]
        $PostProcess
    )

    Generate-SBom -Version $Version -Path $Path -Name $Name -PostProcess:$PostProcess
    Generate-SBom -Version $Version -Path $Path -Name $Name -Json -PostProcess:$PostProcess
}


function Generate-NUnit2-Constraints-SBom {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $Version,

        [switch]
        $Json
    )

    $Name = "XMLUnit.NUnit2.Constraints"
    $targetDir = Join-Path -Path build -ChildPath cyclonedx
    if (-not(Test-Path $targetDir -PathType Container)) {
        New-Item -Path $targetDir -ItemType Directory
    }
    $targetDir = Resolve-Path -Path $targetDir
    $fileName = $JSon ? "$Name.cylconedx.json" : "$Name.cylconedx.xml"
    $fullFileName = Join-Path -Path $targetDir -ChildPath $fileName

    $Args = @(
        "./src/main/net-constraints-nunit2/NetFramework/$Name.NetFramework.csproj"
        "-o"
        "$targetDir"
        "-sv"
        "$Version"
        "-st"
        "library"
        "-sn"
        "$Name"
        "--set-nuget-purl"
        "-imp"
        "./src/shared/cyclonedx-metadata.xml"
        "-fn"
        "$fileName"
    )

    Write-Output "Generating $fullFileName"
    if ($Json) {
        dotnet-CycloneDX @Args -j
    } else {
        dotnet-CycloneDX @Args
    }

    Write-Output "Post-Processing $fullFileName"
    if ($Json) {
        $body = Get-Content $fullFileName -Raw | ConvertFrom-Json -Depth 32
        $coreReference =@"
    {
        "ref": "pkg:nuget/XMLUnit.Core@$Version"
    }
"@
        $body.dependencies += (ConvertFrom-Json -InputObject $coreReference)
        $coreComponent =@"
    {
      "type": "library",
      "bom-ref": "pkg:nuget/XMLUnit.Core@$Version",
      "name": "XMLUnit.Core",
      "version": "$Version",
      "scope": "required"
    }
"@
        $body.components += (ConvertFrom-Json -InputObject $coreComponent)
        $body.dependencies |
          ForEach-Object {
              if ($_.ref -eq "pkg:nuget/XMLUnit.NUnit2.Constraints@$Version") {
                  $_.dependsOn += "pkg:nuget/XMLUnit.Core@$Version"
              }
          }
        $body | ConvertTo-Json -Depth 32 | Set-Content $fullFileName
    } else {
        $body = [xml](Get-Content $fullFileName)
        $coreReference = $body.CreateElement("dependency", $body.bom.NamespaceURI)
        $coreReference.SetAttribute("ref", "pkg:nuget/XMLUnit.Core@$Version")
        $body.bom.dependencies.AppendChild($coreReference)
        $coreComponent = $body.CreateElement("component", $body.bom.NamespaceURI)
        $coreComponent.SetAttribute("type", "library")
        $coreComponent.SetAttribute("bom-ref", "pkg:nuget/XMLUnit.Core@$Version")
        $name = $body.CreateElement("name", $body.bom.NamespaceURI)
        $name.InnerText = "XMLUnit.Core"
        $coreComponent.AppendChild($name)
        $versionElement = $body.CreateElement("version", $body.bom.NamespaceURI)
        $versionElement.InnerText = "$Version"
        $coreComponent.AppendChild($versionElement)
        $scope = $body.CreateElement("scope", $body.bom.NamespaceURI)
        $scope.InnerText = "required"
        $coreComponent.AppendChild($scope)
        $body.bom.components.AppendChild($coreComponent)
        $body.bom.dependencies.dependency |
          ForEach-Object {
              if ($_.HasAttribute("ref") -and $_.Attributes["ref"].Value -eq "pkg:nuget/XMLUnit.NUnit2.Constraints@$Version") {
                  $_.AppendChild($coreReference.Clone())
              }
          }
        $body.Save($fullFileName)
    }
}

Generate-SBoms -Path net-core -Name XMLUnit.Core -Version $Version
Generate-SBoms -Path net-constraints-nunit3 -Name XMLUnit.NUnit3.Constraints -Version $Version -PostProcess
Generate-SBoms -Path net-constraints-nunit4 -Name XMLUnit.NUnit4.Constraints -Version $Version -PostProcess
Generate-SBoms -Path net-placeholders -Name XMLUnit.Placeholders -Version $Version -PostProcess
Generate-NUnit2-Constraints-SBom -Version $Version
Generate-NUnit2-Constraints-SBom -Version $Version -JSon
