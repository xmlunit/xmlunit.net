/*
  This file is licensed to You under the Apache License, Version 2.0
  (the "License"); you may not use this file except in compliance with
  the License.  You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(true)]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion(Org.XmlUnit.XmlUnitVersion.AssemblyVersion)]
[assembly: AssemblyInformationalVersion(Org.XmlUnit.XmlUnitVersion.Version)]
[assembly: AssemblyCompany("XMLUnit Contributors")]

namespace Org.XmlUnit
{
    internal static class XmlUnitVersion
    {
        internal const string ApiVersion = "2.1.1";
        internal const string AssemblyVersion = ApiVersion + ".80";
        internal const string Version = ApiVersion + "-alpha-01";
    } 
}