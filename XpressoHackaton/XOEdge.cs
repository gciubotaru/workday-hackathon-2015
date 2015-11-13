using System;
using QuickGraph;
using System.Xml.Serialization;

namespace XpressoHackaton
{
	public class XOEdge : Edge<XOVertex>
	{
		public XOEdge (string id, XOVertex source, XOVertex target)
			: base(source, target)
		{
			Id = id;
		}

		public string Id { get; private set; }

		[XmlAttribute("Label")]
		public string Name { get; set; }

        [XmlAttribute("Weight")]
        public int Weight { get; set; }

        [XmlAttribute("edge_type")]
        public string EdgeType {get;set;}

        [XmlAttribute("color")]
        public string Color {get;set;}


        public override string ToString ()
		{
			return Id;
		}
	}
}

