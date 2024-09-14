#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.Project.Concept
{
    public class Subscriptioneference
    {
        public string Event { get; set; }

        public string Method { get; set; }



        public static IEnumerable<Subscriptioneference> Parse(string serialized)
        {
            var result = new List<Subscriptioneference>();
            if (String.IsNullOrWhiteSpace(serialized)) { return result; }
            var maps = serialized.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (var map in maps)
            {
                if (map.Length < 1) { continue; }
                if (map.Contains("=>"))
                {
                    var subscription = new Subscriptioneference();
                    result.Add(subscription);
                    var splits = map.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Length < 2) { continue; }
                    subscription.Event = splits[0].Trim();
                    subscription.Method = splits[1].Trim();
                }
                else if (!string.IsNullOrWhiteSpace(map))
                {
                    var subscription = new Subscriptioneference();
                    result.Add(subscription);
                    subscription.Event = map;
                    subscription.Method = map;
                }
            }
            return result;
        }
    }
}