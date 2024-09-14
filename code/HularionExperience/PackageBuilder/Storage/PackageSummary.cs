#region License
/*
MIT License

Copyright (c) 2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Package;
using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HularionExperience.PackageBuilder.Storage
{
    public class PackageSummary
    {

        public string PackageKey { get; set; }

        public string Version { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductKey { get; set; }

        public string Brand { get; set; }

        public string License { get; set; }

        public bool LicenseAgreementIsRequired { get; set; } = false;

        public List<PackageLink> Links { get; set; } = new List<PackageLink>();

        public List<PackageContributor> Contributors { get; set; } = new List<PackageContributor>();

        public List<string> Tags { get; set; } = new List<string>();

        public List<ApplicationSummary> Applications { get; set; } = new List<ApplicationSummary>();

        public List<PackageDependency> PackageDependencies { get; set; } = new List<PackageDependency>();


        public PackageSummary()
        {
            
        }

        public PackageSummary(HularionProject project)
        {
            PackageKey = project.Key;
            Name = project.Name;
            Version = project.Version;
            ProductKey = project.ProductKey;
            Brand = project.Brand;
            Description = project.Description;
            License = project.License;
            LicenseAgreementIsRequired = project.LicenseAgreementIsRequired;
            Links = project.Links.Select(x => new PackageLink(x)).ToList();
            Contributors = project.Collaborators.Select(x => new PackageContributor(x)).ToList();
            Tags = project.Tags.ToList();
            Applications = project.Applications.Select(x => new ApplicationSummary(x)).ToList();
            PackageDependencies = project.PackageImports.Select(x => new PackageDependency(x)).ToList();
        }

    }
}
