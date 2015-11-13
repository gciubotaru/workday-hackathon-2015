using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace XpressoHackaton
{
    public static class ColorHelper
    {
        private static string[] colors;

        static ColorHelper()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XpressoHackaton.Resources.colors.txt"))
            using (StreamReader sr = new StreamReader(stream))
            {
                colors = sr
                    .ReadToEnd()
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .Where(l => l.StartsWith("#"))
                    .Distinct()
                    .ToArray();
            }
        }

        public static string[] Colors { get { return colors; } }
    }
}

