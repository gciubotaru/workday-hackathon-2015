using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace XpressoHackaton
{
	public class XOVertex
	{
		public XOVertex (string id)
		{
			if (string.IsNullOrWhiteSpace (id))
				throw new Exception ("XOVertex with null id");

			Id = id;
		}

		public string Id { get; private set; }

		[XmlAttribute("Label")]
		public string Name { get; set; }

		[XmlAttribute("module_id")]
		public string ModuleId {get;set;}

		[XmlAttribute("module_name")]
		public string ModuleName {get;set;}

        [XmlAttribute("product")]
        public string Product {get;set;}

        [XmlAttribute("product_area")]
        public string ProductArea {get;set;}

        [XmlAttribute("intsances_count")]
		public int InstancesCount {get;set;}

		[XmlAttribute("attributes_count")]
		public int AttributesCount {get;set;}

		[XmlAttribute("methods_count")]
		public int MethodsCount {get;set;}

        [XmlAttribute("access_modifier")]
        public string AccessModifier {get;set;}

        [XmlAttribute("class_spec")]
        public string ClassSpec {get;set;}

        [XmlAttribute("creation_date")]
        public string CreationDate {get;set;}

        [XmlAttribute("color")]
        public string Color {get;set;}

        [XmlAttribute("index_for_event_layout")]
        public int IndexForEventLayout {get;set;}

        //[XmlAttribute("superclasses")]
        public IEnumerable<string> Superclasses {get;set;}

		public override string ToString ()
		{
			return Id;
		}
	}
}

