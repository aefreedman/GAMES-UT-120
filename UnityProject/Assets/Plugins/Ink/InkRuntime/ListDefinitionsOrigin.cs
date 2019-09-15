using System.Collections.Generic;

namespace Ink.Runtime
{
    internal class ListDefinitionsOrigin
    {
        public List<ListDefinition> lists
        {
            get
            {
                var listOfLists = new List<ListDefinition>();
                foreach (var namedList in _lists)
                {
                    listOfLists.Add(namedList.Value);
                }

                return listOfLists;
            }
        }

        public ListDefinitionsOrigin(List<ListDefinition> lists)
        {
            _lists = new Dictionary<string, ListDefinition>();
            _allUnambiguousListValueCache = new Dictionary<string, ListValue>();

            foreach (var list in lists)
            {
                _lists[list.name] = list;

                foreach (var itemWithValue in list.items)
                {
                    var item = itemWithValue.Key;
                    var val = itemWithValue.Value;
                    var listValue = new ListValue(item, val);

                    // May be ambiguous, but compiler should've caught that,
                    // so we may be doing some replacement here, but that's okay.
                    _allUnambiguousListValueCache[item.itemName] = listValue;
                    _allUnambiguousListValueCache[item.fullName] = listValue;
                }
            }
        }

        public bool TryListGetDefinition(string name, out ListDefinition def)
        {
            return _lists.TryGetValue(name, out def);
        }

        public ListValue FindSingleItemListWithName(string name)
        {
            ListValue val = null;
            _allUnambiguousListValueCache.TryGetValue(name, out val);
            return val;
        }

        Dictionary<string, ListDefinition> _lists;
        Dictionary<string, ListValue> _allUnambiguousListValueCache;
    }
}