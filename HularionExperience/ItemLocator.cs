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
    public class ItemLocator<T>
    {
        private Type type = typeof(T);

        private Dictionary<PropertyInfo, ItemLocationIndicator> properties = new Dictionary<PropertyInfo, ItemLocationIndicator>();

        private List<ItemLocationIndicator> sorted = new List<ItemLocationIndicator>();

        private List<T> items = new List<T>();

        private long definitvePriority = (1 << 32);

        public void AddIndicator(ItemLocationIndicator indicator)
        {
            if (type.GetProperties().Where(x => x == indicator.Property).Count() <= 0) { throw new ArgumentException(String.Format("The property {0} provided by the indicator is not a member of type {1}.", indicator.Property.Name, type.FullName)); }
            if (properties.ContainsKey(indicator.Property)) { properties.Remove(indicator.Property); }
            properties.Add(indicator.Property, indicator);
            sorted = properties.Values.OrderByDescending(x => (indicator.IsDefinitive ? definitvePriority : 0) + x.Priority).ToList();
        }

        public void AddIndicators(params ItemLocationIndicator[] indicators)
        {
            foreach (var indicator in indicators) { AddIndicator(indicator); }
        }

        public void AddItem(T item)
        {
            items.Add(item);
        }

        public void AddItems(params T[] items)
        {
            AddItems(items.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer">Returns true iff the item must be removed.</param>
        public void RemoveItems(Func<T, bool> comparer)
        {
            items = items.Where(x => !comparer(x)).ToList();
        }

        public void AddItems(IEnumerable<T> items)
        {
            foreach (var item in items) { AddItem(item); }
        }

        public T GetMatch(IEnumerable<ItemLocationValue> values)
        {
            return GetMatches(values).FirstOrDefault();
        }

        public T[] GetMatches(IEnumerable<ItemLocationValue> values, bool highestPriorityOnly = false)
        {
            var valueMap = values.ToDictionary(x => x.Property, y => y);
            var results = new Dictionary<T, int>();
            foreach (var locationIndicator in sorted)
            {
                if (!valueMap.ContainsKey(locationIndicator.Property)) { continue; }
                var value = valueMap[locationIndicator.Property];
                foreach (var item in items)
                {
                    if (!locationIndicator.Comparer(value, item)) { continue; }
                    if (locationIndicator.IsDefinitive) { return new T[] { item }; }
                    if (!results.ContainsKey(item)) { results.Add(item, 0); }
                    results[item] = results[item] + locationIndicator.Priority;
                }
            }
            if (highestPriorityOnly)
            {
                var max = results.Max(x => x.Value);
                return results.Where(x => x.Value == max).OrderBy(x => x.Value).Select(x => x.Key).ToArray();
            }
            return results.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
        }

        public IEnumerable<T> GetAllItems()
        {
            return items.ToList();
        }
    }

    public class ItemLocationIndicator
    {
        public PropertyInfo Property { get; set; }

        public bool IsDefinitive { get; set; } = false;

        public int Priority { get; set; } = 0;

        public Func<ItemLocationValue, object, bool> Comparer { get; set; } = new Func<ItemLocationValue, object, bool>((x, y) => false);
    }

    public class ItemLocationValue
    {
        public PropertyInfo Property { get; set; }

        public object Value { get; set; }
    }



}
