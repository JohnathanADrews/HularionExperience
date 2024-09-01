#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

//using HularionExperience.Definition.PackageMeta;
using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientPresenterSet
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string CssStyles { get; set; }

        public Dictionary<string, ClientPresenter> Presenters { get; set; } = new Dictionary<string, ClientPresenter>();

        public List<ClientFrameReference> ScriptImportFrames { get; set; } = new List<ClientFrameReference>();

        public List<ClientScriptReference> ScriptReferences { get; set; } = new List<ClientScriptReference>();

        public List<ClientFrameReference> PresenterImportFrames { get; set; } = new List<ClientFrameReference>();

        public List<ClientPresenterFrameHandle> PresenterReferences { get; set; } = new List<ClientPresenterFrameHandle>();

        public List<ClientStyle> Styles { get; set; } = new List<ClientStyle>();

        public Dictionary<string, Dictionary<string, HashSet<string>>> StyleCategories { get; set; } = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        public HashSet<string> PersistentStyles { get; set; } = new HashSet<string>();

        public Dictionary<string, string> StyleCategoryDefaults { get; set; } = new Dictionary<string, string>();

        public List<ClientCommunicator> Communicators { get; set; } = new List<ClientCommunicator>();

        public ClientPresenterSet()
        {

        }

        public ClientPresenterSet(PresenterSet set, HularionProject package)
        {
            Name = set.Name;
            Presenters = set.Presenters.ToDictionary(x=>x.Name, x=>new ClientPresenter(x));
            if(Presenters.Count == 0) { return; }

            foreach (var presenter in set.Presenters)
            {
                foreach(var style in presenter.Styles)
                {
                    Styles.Add(new ClientStyle(style));
                    if (style.CategoryKey != null && style.CategoryValue != null)
                    {
                        if (!StyleCategories.ContainsKey(style.CategoryKey)) { StyleCategories.Add(style.CategoryKey, new Dictionary<string, HashSet<string>>()); }
                        if (!StyleCategories[style.CategoryKey].ContainsKey(style.CategoryValue)) { StyleCategories[style.CategoryKey].Add(style.CategoryValue, new HashSet<string>()); }
                        StyleCategories[style.CategoryKey][style.CategoryValue].Add(style.Key);
                    }
                    else { PersistentStyles.Add(style.Key); }
                }
            }

            var configuration = package.PresenterConfigurations.Where(x => x.PresenterSet == set.Name).FirstOrDefault();
            if(configuration != null)
            {
                ScriptImportFrames = configuration.ScriptImports.Where(x => x.Type == FrameReferenceType.Script).Select(x => new ClientFrameReference(x)).ToList();
                ScriptReferences = configuration.ScriptFrameHandles.Select(x => new ClientScriptReference(x)).ToList();
                PresenterImportFrames = configuration.PresenterSetImports.Where(x => x.Type == FrameReferenceType.Presenter).Select(x => new ClientFrameReference(x)).ToList();
                PresenterReferences = configuration.PresenterFrameHandles.Select(x => new ClientPresenterFrameHandle(x)).ToList();
                StyleCategoryDefaults = configuration.StyleCategoryDefaults.ToDictionary(x=>x.Key, x=>x.Value);
                CssStyles = configuration.CssStyles;
                //collaborators = new HashSet<string>(configuration.Collaborators);
                Communicators = configuration.CommunicatorHandles.Select(x => new ClientCommunicator() { Handle = x }).ToList();
            }

        }



        //public ClientPresenterSetMeta CreatePresenterSetMeta()
        //{
        //    var meta = new ClientPresenterSetMeta()
        //    {
        //        name = name,
        //        description = description
        //    };
        //    return meta;
        //}

    }
}
