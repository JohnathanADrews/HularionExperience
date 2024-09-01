#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience
{
    public class HularionExperienceKeyword
    {

        public string Name { get; set; }
        public string Value { get; set; }


        public static HularionExperienceKeyword Hularion = new() { Name = nameof(Hularion), Value = "hularion" };
        public static HularionExperienceKeyword BaseRoute = new() { Name = nameof(BaseRoute), Value = "hularion" };
        public static HularionExperienceKeyword HXProjectName = new() { Name = nameof(HXProjectName), Value = ".hxproject" };
        public static HularionExperienceKeyword HXPackageExtension = new() { Name = nameof(HXPackageExtension), Value = "hxpackage" };
        public static HularionExperienceKeyword HXPartialExtension = new() { Name = nameof(HXPartialExtension), Value = "hxpartial" };
        public static HularionExperienceKeyword HXClientPartialExtension = new() { Name = nameof(HXClientPartialExtension), Value = "client" };
        public static HularionExperienceKeyword HXServerPartialExtension = new() { Name = nameof(HXServerPartialExtension), Value = "server" };
        public static HularionExperienceKeyword HXSummaryPartialExtension = new() { Name = nameof(HXSummaryPartialExtension), Value = "summary" };
        public static HularionExperienceKeyword HXPackagesDirectory = new() { Name = nameof(HXPackagesDirectory), Value = "packages" };
        public static HularionExperienceKeyword HXServerPackagesDirectory = new() { Name = nameof(HXServerPackagesDirectory), Value = "server" };
        public static HularionExperienceKeyword HXPartialPackagesDirectory = new() { Name = nameof(HXPartialPackagesDirectory), Value = "partial" };
        public static HularionExperienceKeyword HXSourcePackagesDirectory = new() { Name = nameof(HXSourcePackagesDirectory), Value = "source" };
        public static HularionExperienceKeyword HXLatestPackageIndicator = new() { Name = nameof(HXLatestPackageIndicator), Value = "latest" };
        public static HularionExperienceKeyword HXProjectPackageVersionIndicator = new() { Name = nameof(HXProjectPackageVersionIndicator), Value = "project" };
        public static HularionExperienceKeyword HXSystemPackageIndicator = new() { Name = nameof(HXSystemPackageIndicator), Value = "system" };
        //public static HularionExperienceKeyword ClientBaseRoute = new HularionExperienceKeyword() { Name = nameof(ClientBaseRoute), Value = String.Format("{0}/client", BaseRoute.Value) };


        public static string MakePackageVersionKey(string packageKey, string packageVersion)
        {
            packageKey = String.Format("{0}", packageKey).Trim();
            packageVersion = String.Format("{0}", packageVersion).Trim();
            return String.Format("{0}@{1}", packageKey, packageVersion);
        }

        public static string MakePackageFilename(string packageKey, string packageVersion)
        {
            return String.Format("{0}.{1}", MakePackageVersionKey(packageKey, packageVersion), HularionExperienceKeyword.HXPackageExtension.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }

}
