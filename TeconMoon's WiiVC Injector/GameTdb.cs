using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TeconMoon_s_WiiVC_Injector
{
    public class GameTdb
    {
        private const string ResourcePath = "TeconMoon_s_WiiVC_Injector.Resources.wiitdb.txt";
        private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

        public static string GetName(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            using (var stream = CurrentAssembly.GetManifestResourceStream(ResourcePath))
            {
                if (stream == null) return null;
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                        if (split.Length == 2 && split[0].Equals(id, StringComparison.Ordinal))
                            return split[1];
                    }
                }
            }
            return null;
        }

        public static List<string> GetIds(string name)
        {
            var ids = new List<string>();
            if (string.IsNullOrEmpty(name)) return ids;

            using (var stream = CurrentAssembly.GetManifestResourceStream(ResourcePath))
            {
                if (stream == null) return ids;
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                        if (split.Length == 2 && split[1].Equals(name, StringComparison.Ordinal))
                            ids.Add(split[0]);
                    }
                }
            }
            return ids;
        }

        public static List<string> GetIdsStartingWith(string idStart)
        {
            var ids = new List<string>();
            if (string.IsNullOrEmpty(idStart)) return ids;

            using (var stream = CurrentAssembly.GetManifestResourceStream(ResourcePath))
            {
                if (stream == null) return ids;
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("TITLES =", StringComparison.Ordinal))
                            continue;

                        var split = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
                        if (split.Length < 2) continue;

                        if (split[0].StartsWith(idStart, StringComparison.Ordinal))
                            ids.Add(split[0]);

                        // Gametdb titles are ordered alphabetically by id, so stop searching
                        if (split[0].Length >= idStart.Length && string.Compare(idStart, split[0].Substring(0, idStart.Length), StringComparison.Ordinal) < 0)
                            break;
                    }
                }
            }
            return ids;
        }

        internal static IEnumerable<string> GetAlternativeIds(string initialId)
        {
            if (string.IsNullOrEmpty(initialId) || initialId.Length < 4)
            {
                if (!string.IsNullOrEmpty(initialId)) yield return initialId;
                yield break;
            }

            var tried = new HashSet<string>
            {
                initialId,
                initialId.ReplaceAt(3, 'E'),
                initialId.ReplaceAt(3, 'P'),
            };

            foreach (var id in tried)
            {
                yield return id;
            }

            var gameName = GetName(initialId);
            if (!string.IsNullOrEmpty(gameName))
            {
                var ids = GetIds(gameName).Where(id => !tried.Contains(id));

                foreach (var id in ids)
                {
                    yield return id;
                    tried.Add(id);
                }
            }

            // as last resort, try a match on only the 3 first characters of
            // the key (e.g. for Obscure 2)
            var moreIds = GetIdsStartingWith(initialId.Substring(0, 3))
                .Where(id => !tried.Contains(id));

            foreach (var id in moreIds)
            {
                yield return id;
            }
        }
    }
}

