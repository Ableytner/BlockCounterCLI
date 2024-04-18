using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockCounterCLI.helper
{
    internal class VersionHelper
    {
        public static Tuple<IEnumerable<int>, string>[] VERSIONS =
        [
            new(Enumerable.Range(-1, 100), "1.8.9"),
            new(Enumerable.Range(100, 85), "1.9.4"),
            new(Enumerable.Range(501, 12), "1.10.2"),
            new(Enumerable.Range(800, 923), "1.11.2"),
            new(Enumerable.Range(1022, 322), "1.12.2"),
            new(Enumerable.Range(1444, 188), "1.13.2"),
            new(Enumerable.Range(1901, 169), "1.14.4"),
            new(Enumerable.Range(2200, 122), "1.15.2"),
            new(Enumerable.Range(2504, 83), "1.16.5"),
            new(Enumerable.Range(2701, 30), "1.17.1"),
            new(Enumerable.Range(2825, 151), "1.18.2"),
            new(Enumerable.Range(3066, 272), "1.19.4"),
            new(Enumerable.Range(3442, 259), "1.20.4")
        ];

        public static Tuple<IEnumerable<int>, string> GetVersion(int versionId)
        {
            foreach (var version in VERSIONS)
            {
                if (version.Item1.Contains(versionId))
                {
                    return version;
                }
            }

            return null;
        }

        public static string GetVersionName(int versionId)
        {
            Tuple<IEnumerable<int>, string> version = GetVersion(versionId);
            return version?.Item2;
        }
    }
}
