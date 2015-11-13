using System;
using QuickGraph;
using QuickGraph.Serialization;
using System.Xml;
using QuickGraph.Algorithms;

namespace XpressoHackaton
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//InputLoader.CountClasses();
            InputLoader.GenerateGraphML(@"full_no_edge_weights.graphml");
		}
	}
}
