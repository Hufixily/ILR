using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using Object = UnityEngine.Object;

using UnityEditor;

namespace game.IL.Editor
{
    public partial class GUIHelp
    {
        public static string StringPopupT<T>(string label, string selected, List<T> displayedOptions, Func<T, string> fun, string searchName="", params GUILayoutOption[] options)
        {
            List<string> ds = new List<string>();
            string searchLow = string.IsNullOrEmpty(searchName) ? "" : searchName.ToLower();
            for (int i = 0; i < displayedOptions.Count; ++i)
            {
                string item = fun(displayedOptions[i]);
                if (!string.IsNullOrEmpty(searchName) && selected != item && !item.ToLower().Contains(searchLow))
                    continue;
                ds.Add(item);
            }

            return StringPopup(label, selected, ds, options);
        }

        public static string StringPopup(string selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            return StringPopup(null, selected, displayedOptions, options);
        }

        public static string StringPopup(string label, string selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            int index = StringPopup(label, displayedOptions.IndexOf(selected), displayedOptions, options);
            return displayedOptions.Count == 0 ? selected : displayedOptions[index];
        }

        public static int StringPopup(string label, int selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            if (selected < 0 || selected >= displayedOptions.Count)
                selected = 0;

            List<int> valueList = new List<int>();
            for (int i = 0; i < displayedOptions.Count; ++i)
                valueList.Add(i);

            if (!string.IsNullOrEmpty(label))
                selected = EditorGUILayout.IntPopup(label, selected, displayedOptions.ToArray(), valueList.ToArray(), options);
            else
                selected = EditorGUILayout.IntPopup(selected, displayedOptions.ToArray(), valueList.ToArray(), options);

            return selected;
        }

        public static T StringPopup<T>(string label, T selected, List<T> displayedOptions, System.Func<T, string> fun, params GUILayoutOption[] options)
        {
            if (!displayedOptions.Contains(selected) && displayedOptions.Count != 0)
                selected = displayedOptions[0];

            List<int> valueList = new List<int>();
            List<string> keyList = new List<string>();
            for (int i = 0; i < displayedOptions.Count; ++i)
            {
                valueList.Add(i);
                keyList.Add(fun(displayedOptions[i]));
            }

            int selectedindex = displayedOptions.IndexOf(selected);
            if (!string.IsNullOrEmpty(label))
                selectedindex = EditorGUILayout.IntPopup(label, selectedindex, keyList.ToArray(), valueList.ToArray(), options);
            else
                selectedindex = EditorGUILayout.IntPopup(selectedindex, keyList.ToArray(), valueList.ToArray(), options);

            if (selectedindex == -1)
                return selected;

            return displayedOptions[selectedindex];
        }

        public static K MapStringPopup<K, V>(
            string label,
            K selectedValue,
            IDictionary<K, V> maps,
            Action<List<KeyValuePair<K, V>>> begin,
            Func<KeyValuePair<K, V>, string> keyfun,
            Action onintpopbegin,
            Action<K> onintpopend,
            Action<KeyValuePair<K, V>> onselect,
            params GUILayoutOption[] options)
        {
            List<KeyValuePair<K, V>> ls = new List<KeyValuePair<K, V>>();
            foreach (KeyValuePair<K, V> itor in maps)
                ls.Add(itor);

            if (begin != null)
                begin(ls);

            List<string> values = new List<string>();
            List<int> intV = new List<int>();
            int current = -1;

            foreach (KeyValuePair<K, V> itor in ls)
            {
                string key = keyfun(itor);
                values.Add(key);
                intV.Add(intV.Count);
                if (itor.Key.Equals(selectedValue))
                    current = values.Count - 1;
            }

            if (current == -1)
                current = 0;

            if (onintpopbegin != null)
                onintpopbegin();

            if (string.IsNullOrEmpty(label))
                current = EditorGUILayout.IntPopup(current, values.ToArray(), intV.ToArray(), options);
            else
                current = EditorGUILayout.IntPopup(label, current, values.ToArray(), intV.ToArray(), options);

            if (values.Count > current)
            {
                if (onintpopend != null)
                    onintpopend(ls[current].Key);

                if (onselect != null)
                {
                    onselect(ls[current]);
                }

                return ls[current].Key;
            }

            if (onintpopend != null)
                onintpopend(default(K));

            return default(K);
        }

        public static string MapStringPopup<T>(string label, string selectedValue, IDictionary<string, T> maps, System.Action<List<KeyValuePair<string, T>>> begin, Func<KeyValuePair<string, T>, string> fun, Action<KeyValuePair<string, T>> onselect, params GUILayoutOption[] options)
        {
            List<string> values = new List<string>();
            List<KeyValuePair<string, T>> vvs = new List<KeyValuePair<string, T>>();
            List<int> intV = new List<int>();
            Dictionary<string, string> showToKeys = new Dictionary<string, string>();
            string show = "";
            int current = -1;

            List<KeyValuePair<string, T>> ls = new List<KeyValuePair<string, T>>();
            foreach (KeyValuePair<string, T> itor in maps)
                ls.Add(itor);

            if (begin != null)
                begin(ls);

            foreach (KeyValuePair<string, T> itor in ls)
            {
                if (fun == null)
                    show = itor.Key;
                else
                    show = fun(itor);

                showToKeys[show] = itor.Key;

                if (itor.Key == selectedValue)
                    current = values.Count;

                values.Add(show);
                intV.Add(intV.Count);

                vvs.Add(itor);
            }

            if (current == -1)
                current = 0;

            if (string.IsNullOrEmpty(label))
                current = EditorGUILayout.IntPopup(current, values.ToArray(), intV.ToArray(), options);
            else
                current = EditorGUILayout.IntPopup(label, current, values.ToArray(), intV.ToArray(), options);

            if (values.Count > current)
            {
                if (onselect != null)
                {
                    onselect(vvs[current]);
                }

                return showToKeys[values[current]];
            }

            return null;
        }
    }
}