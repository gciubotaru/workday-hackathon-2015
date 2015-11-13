using System;

namespace XpressoHackaton
{
    public class Module
    {
        public Module(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public string Name { get; set; }

        public string Product { get; set; }

        public string ProductArea { get; set; }
    }
}

