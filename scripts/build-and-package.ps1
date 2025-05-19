# Requires -Version 7

param(
    [Parameter(Mandatory = $true)]
    [string]
    $Version
)

function BuildAll {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $Version,

        [Parameter(Mandatory = $true)]
        [string]
        $Configuration
    )

    msbuild /p:Configuration=$Configuration XMLUnit.NET.NetFramework.sln
    msbuild /p:Configuration=$Configuration src/main/net-constraints-nunit4/NetFramework/XMLUnit.NUnit4.Constraints.NetFramework.csproj

    dotnet build XMLUnit.NET.sln -c $Configuration
    dotnet build src/main/net-constraints-nunit4/XMLUnit.NUnit4.Constraints.csproj -c $Configuration
}

function BuildNugetPackage {
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $Version,

        [Parameter(Mandatory = $true)]
        [string]
        $Spec
    )

    nuget pack $Spec -Symbols -OutputDirectory build\ -Properties "version=$Version"
}

$env:DOTNET_ROLL_FORWARD = "Major"

dotnet restore XMLUnit.NET.sln

BuildAll -Version $Version -Configuration Debug

dotnet test src/tests/net-core/XMLUnit.Core.Tests.csproj
dotnet test src/tests/net-constraints-nunit3/XMLUnit.NUnit3.Constraints.Test.csproj
dotnet test src/tests/net-placeholders/XMLUnit.Placeholders.Tests.csproj
dotnet test src/tests/net-constraints-nunit4/XMLUnit.NUnit4.Constraints.Test.csproj

BuildAll -Version $Version -Configuration Release

BuildNugetPackage -Version $Version -Spec src\main\net-core\XMLUnit.Core.nuspec
BuildNugetPackage -Version $Version -Spec src\main\net-constraints-nunit2\XMLUnit.NUnit2.Constraints.nuspec
BuildNugetPackage -Version $Version -Spec src\main\net-constraints-nunit3\XMLUnit.NUnit3.Constraints.nuspec
BuildNugetPackage -Version $Version -Spec src\main\net-constraints-nunit4\XMLUnit.NUnit4.Constraints.nuspec
BuildNugetPackage -Version $Version -Spec src\main\net-placeholders\XMLUnit.Placeholders.nuspec
