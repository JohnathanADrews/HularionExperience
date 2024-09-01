#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Project.Concept;
using System.Collections.Generic;
using System.Linq;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientScriptSet
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<ClientScript> Scripts { get; set; } = new List<ClientScript>();

        public List<ClientFrameReference> ScriptImportFrames { get; set; } = new List<ClientFrameReference>();

        public List<ClientScriptFrameHandle> ScriptFrameHandles { get; set; } = new List<ClientScriptFrameHandle>();

        public List<ClientFrameReference> PresenterImportFrames { get; set; } = new List<ClientFrameReference>();

        public List<ClientPresenterFrameHandle> PresenterReferences { get; set; } = new List<ClientPresenterFrameHandle>();

        public List<ClientAttachObjectIndicator> AttachIndicators { get; set; } = new List<ClientAttachObjectIndicator>();

        public List<ClientFrameReceiver> FrameReceivers { get; set; } = new List<ClientFrameReceiver>();

        public List<ClientCommunicator> Communicators { get; set; } = new List<ClientCommunicator>();

        public ClientScriptSet()
        {

        }

        public ClientScriptSet(ScriptSet set, HularionProject package)
        {
            Key = set.Key;
            Name = set.Name;
            Scripts = set.Scripts.Select(x => new ClientScript(x)).ToList();
            var configuration = package.ScriptConfigurations.Where(x => x.ScriptSet == set.Name).FirstOrDefault();
            if(configuration == null) { return; }
            ScriptImportFrames = configuration.Frames.Where(x=>x.Type == FrameReferenceType.Script).Select(x => new ClientFrameReference(x)).ToList();
            ScriptFrameHandles = configuration.ScriptFrameHandles.Select(x => new ClientScriptFrameHandle(x)).ToList();
            AttachIndicators = configuration.AttachIndicators.Select(x => new ClientAttachObjectIndicator(x)).ToList();
            PresenterImportFrames = configuration.PresenterSetImports.Select(x => new ClientFrameReference(x)).ToList();
            PresenterReferences = configuration.PresenterFrameHandles.Select(x => new ClientPresenterFrameHandle(x)).ToList();
            FrameReceivers = configuration.ReceivedFrames.Select(x => new ClientFrameReceiver(x)).ToList();
            //collaborators = new HashSet<string>(configuration.Collaborators);
            Communicators = configuration.CommunicatorHandles.Select(x => new ClientCommunicator() { Handle = x }).ToList();
        }

        //public ClientScriptSetMeta CreateScriptSetMeta()
        //{
        //    var meta = new ClientScriptSetMeta()
        //    {
        //        name = name,
        //        description = description
        //    };
        //    return meta;
        //}
    }
}
