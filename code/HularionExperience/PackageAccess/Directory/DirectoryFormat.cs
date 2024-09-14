#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageAccess.Directory
{

    public class DirectoryFormat
    {

        public string BaseDirectory { get; set; }

        public DirectoryFormat()
        {
                
        }

        public DirectoryFormat(string baseDirectory)
        {
            this.BaseDirectory = baseDirectory;
        }

        public string FormatVersion(string version)
        {
            return string.Format("{0}", version).Trim().ToLower();
        }

        public string GetPackageDirectory(string packageKey, string version)
        {
            return string.Format(@"{0}\{1}\{2}\{3}", BaseDirectory, HularionExperienceKeyword.HXPackagesDirectory.Value, packageKey, version);
        }

        public string GetPartialDirectory(string packageKey, string version)
        {
            return string.Format(@"{0}\{1}", GetPackageDirectory(packageKey, version), HularionExperienceKeyword.HXPartialPackagesDirectory.Value);
        }

        public string GetClientPartialFilename(string packageKey, string version)
        {
            return string.Format(@"{0}\{1}.{2}", GetPartialDirectory(packageKey, version), HularionExperienceKeyword.HXClientPartialExtension.Value, HularionExperienceKeyword.HXPartialExtension.Value);
        }

        public string GetServerPartialFilename(string packageKey, string version)
        {
            return string.Format(@"{0}\{1}.{2}", GetPartialDirectory(packageKey, version), HularionExperienceKeyword.HXServerPartialExtension.Value, HularionExperienceKeyword.HXPartialExtension.Value);
        }

        public string GetServerFilesDirectory(string packageKey, string version)
        {
            return string.Format(@"{0}\{1}", GetPackageDirectory(packageKey, version), HularionExperienceKeyword.HXServerPackagesDirectory.Value);
        }
    }
}
