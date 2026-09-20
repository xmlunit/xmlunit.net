# Requires -Version 7

# This file is licensed to You under the Apache License, Version 2.0
# (the "License"); you may not use this file except in compliance with
# the License.  You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

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

    dotnet build XMLUnit.NET.sln -c $Configuration -p:ContinuousIntegrationBuild=true
    dotnet build src/main/net-constraints-nunit4/XMLUnit.NUnit4.Constraints.csproj -c $Configuration -p:ContinuousIntegrationBuild=true
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
