using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using QuickGraph;
using QuickGraph.Serialization;

namespace XpressoHackaton
{
	public static class InputLoader
	{
        private static Dictionary<string, Module> _modules;
        private static Dictionary<string, XOVertex> _classes;
        private static Dictionary<string, XOEdge> _relationships;
        private static Dictionary<string, string> _moduleColorMap;
        private static Dictionary<string, int> _moduleIdx;

        static InputLoader()
		{
            using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("XpressoHackaton.Resources.modules.txt")) 
			using (StreamReader sr = new StreamReader(stream))
			{
				_modules = sr
					.ReadToEnd ()
					.Split (new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Split('\t'))
					.ToDictionary(
						a => a[0], 
                        a => new Module(a[0])
                        {
                            Name = a[1],
                            Product = a[2],
                            ProductArea = a[3]
                        });
			}

            _moduleColorMap = new Dictionary<string, string>();
            _moduleIdx = new Dictionary<string, int>();

            int idx = 0;
            foreach (var module in _modules.Keys.OrderBy(s => s))
            {
                _moduleColorMap[module] = ColorHelper.Colors[idx];
                _moduleIdx[module] = idx;
                idx++;
            }

            using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("XpressoHackaton.Resources.classes.txt")) 
			using (StreamReader sr = new StreamReader(stream))
			{
				_classes = sr
					.ReadToEnd ()
					.Split (new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Split('\t'))
					.ToDictionary(
						a => a[0], 
						a => new XOVertex(a[0]) 
						{
							Name = a[1],
							InstancesCount = int.Parse(a[2]),
							ModuleId = a[3],
                            ModuleName = _modules[a[3]].Name,
                            Product = _modules[a[3]].Product,
                            ProductArea = _modules[a[3]].ProductArea,
							MethodsCount = int.Parse(a[4]),
							AttributesCount = int.Parse(a[5]),
                            AccessModifier = a[6],
                            ClassSpec = a[7],
                            Superclasses = a[8].Trim('"').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                            CreationDate = BuildTimeInterval(a[9]),
                            Color = _moduleColorMap[a[3]],
                            IndexForEventLayout = _moduleIdx[a[3]]
						});
			}

            using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("XpressoHackaton.Resources.relationships.txt")) 
			using (StreamReader sr = new StreamReader(stream))
			{
				_relationships = sr
					.ReadToEnd ()
					.Split (new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Split('\t'))
                    .Where(a => a[1] != a[3]) //TODO
					.ToDictionary(
						a => a[0], 
						a => new XOEdge(a[0], _classes[a[1]], _classes[a[3]])
						{
							Name = a[2],
                            Weight = BuildEdgeWeight(_classes[a[1]], _classes[a[3]]),
                            EdgeType = BuildEdgeRelationshipType(_classes[a[1]], _classes[a[3]]),
                            Color = BuildEdgeRelationshipColor(_classes[a[1]], _classes[a[3]])
						});
			}
		}

        public static void CountClasses()
        {
            int count = _classes.Values.Where(c => c.ClassSpec == "Audited"/* || c.ClassSpec == "Metadata"*/).Count();

            Console.WriteLine(count);
        }

        public static void GenerateGraphML(string fileName)
        {
            var graph = new AdjacencyGraph<XOVertex, XOEdge>();

            var relationshipsSubset = _relationships.Values
                //.Where(e => e.Source.ModuleId == "57$61" && e.Target.ModuleId == "57$61")
                .Where(e => (e.Source.ClassSpec == "Audited" || e.Source.ClassSpec == "Metadata") && (e.Target.ClassSpec == "Audited" || e.Target.ClassSpec == "Metadata"))
                .ToArray();

            foreach (var r in relationshipsSubset) 
            {
                if (!graph.ContainsVertex(r.Source))
                    graph.AddVertex(r.Source);

                if (!graph.ContainsVertex(r.Target))
                    graph.AddVertex(r.Target);
            }

            graph.AddEdgeRange(relationshipsSubset);

            int idx = 0;
            foreach (var v in graph.Vertices)
            {
                foreach (var super in v.Superclasses.Select(s => _classes[s]).Where(s => graph.ContainsVertex(s)))
                {
                    string id = string.Concat("inheritance_", idx++);
                    graph.AddEdge(new XOEdge(id, v, super)
                        {
                            Weight = BuildEdgeWeight(v, super),
                            EdgeType = BuildEdgeInheritanceType(v, super),
                            Color = BuildEdgeInheritanceColor(v, super)
                        });
                }
            }

            VertexIdentity<XOVertex> vi = v => v.Id;
            EdgeIdentity<XOVertex, XOEdge> ei = e => e.Id;

            using (XmlWriter xw = XmlWriter.Create(fileName)) 
            {
                graph.SerializeToGraphML<XOVertex, XOEdge, AdjacencyGraph<XOVertex, XOEdge>>(xw, vi, ei);
            }
        }

        private static int BuildEdgeWeight(XOVertex source, XOVertex target)
        {
            /*
            if (source.ModuleName == target.ModuleName)
                return 100;
            if (source.Product == target.Product)
                return 5;
            if (source.ProductArea == target.ProductArea)
                return 5;*/
            return 1;
        }

        private static string BuildEdgeRelationshipType(XOVertex source, XOVertex target)
        {
            if (source.ModuleName == target.ModuleName)
                return "Module Relationship";
            if (source.Product == target.Product)
                return "Product Relationship";
            if (source.ProductArea == target.ProductArea)
                return "ProductArea Relationship";
            return "Other Relationship";
        }

        private static string BuildEdgeInheritanceType(XOVertex source, XOVertex target)
        {
            if (source.ModuleName == target.ModuleName)
                return "Module Inheritance";
            if (source.Product == target.Product)
                return "Product Inheritance";
            if (source.ProductArea == target.ProductArea)
                return "ProductArea Inheritance";
            return "Other Inharitance";
        }

        private static string BuildEdgeRelationshipColor(XOVertex source, XOVertex target)
        {
            if (source.ModuleName == target.ModuleName)
                return "#333333";
            if (source.Product == target.Product)
                return "#737373";
            if (source.ProductArea == target.ProductArea)
                return "#a6a6a6";
            return "#e6e6e6";
        }

        private static string BuildEdgeInheritanceColor(XOVertex source, XOVertex target)
        {
            if (source.ModuleName == target.ModuleName)
                return "#ffff00";
            if (source.Product == target.Product)
                return "#ffff66";
            if (source.ProductArea == target.ProductArea)
                return "#ffff99";
            return "#ffffcc";
        }

        private static string BuildTimeInterval(string date)
        {
            string[] parts = date.Split(' ');

            //return string.Format("<[{0}-{1}-{2}T{3}:{4}:{5}.{6}, 2015-11-12]>", parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], parts[6]);
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}", parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
        }
    }
}

