namespace game.IL
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using System;
    using System.Linq;
    using System.Xml;

    class GenLinkXml
    {
        /// <summary>
        /// 打包IOS时，防止IL2Cpp剥离需要用到的库
        /// </summary>
        [MenuItem("IL/生成link.xml",false,5)]
        public static void Gen()
        {
            //收集需要输出xml的dll信息
            var assemblies = new List<Assembly>()
            {
                //待填充
            };

            assemblies = assemblies.Distinct().ToList();

            var xml = new XmlDocument();
            var rootElement = xml.CreateElement("linker");
            
            foreach(var ass in assemblies)
            {
                var assElement = xml.CreateElement("assembly");
                assElement.SetAttribute("fullname", ass.GetName().Name);
                var types = ass.GetTypes();
                foreach(var type in types)
                {
                    if (type.FullName.Equals("Win32"))
                        continue;
                    var typeElement = xml.CreateElement("type");
                    typeElement.SetAttribute("fullname", type.FullName);
                    typeElement.SetAttribute("preserve", "all");
                    assElement.AppendChild(typeElement);
                }
                rootElement.AppendChild(assElement);
            }
            xml.AppendChild(rootElement);

            var path = Application.dataPath + "/" + "link.xml";
            if (File.Exists(path))
                File.Delete(path);
            xml.Save(path);
        }
    }
}
