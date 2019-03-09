using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace showDependencies
{
    class Program
    {
        Dictionary<string, List<string>> dependecies = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>> flatDependecies = new Dictionary<string, List<string>>();

        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private static void Msg(string text)
        {
            Console.WriteLine(text);
        }

        private void Run(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            string filter = string.Empty;

            if (args.Length > 0)
            {
                path = args[0];
            }
            if (args.Length > 1)
            {
                filter = args[1];
            }

            string[] projectFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

            Console.WriteLine($"{projectFiles.Length} Projects found. Analyzing...");

            foreach (string p in projectFiles)
            {
                Analyze(p);
            }

            GenerateFlatDependencies(filter);

            Console.WriteLine("Flat dependencies:");
            PrintFlatDependecies();
            Console.WriteLine(new String('-', 20));
            Console.WriteLine("Tree dependencies:");
            PrintTreeDependecies("", 0, filter);
        }

        private void PrintTreeDependecies(string key = "", int tabs = 0, string include = "")
        {
            if (tabs > 10)
            {
                Console.WriteLine("tabs > 10!!");
                return;
            }
            if (key.Length > 0)
            {
                Console.WriteLine(new String('\t', tabs) + key);
                if (dependecies.ContainsKey(key))
                {
                    List<string> deps = dependecies[key].Where(x => x.Contains(include)).ToList(); ;
                    deps.Sort();
                    foreach (string dep in deps)
                    {
                        PrintTreeDependecies(dep, tabs + 1, include);
                    }
                }
            }
            else
            {
                foreach (var item in dependecies.OrderBy(x => x.Value.Count).ThenBy(n => n.Key))
                {
                    PrintTreeDependecies(item.Key, 0, include);
                    Console.WriteLine();
                }
            }
        }

        private void GenerateFlatDependencies(string include = "")
        {
            foreach (var item in dependecies)
            {
                List<string> flatDeps = DependenyArray(item.Key).Where(x => x.Contains(include)).ToList();
                flatDeps.Sort();
                flatDependecies.Add(item.Key, flatDeps);
            }
        }

        private List<string> DependenyArray(string key)
        {
            List<string> result = new List<string>();
            if (dependecies.ContainsKey(key))
            {
                foreach (string dep in dependecies[key])
                {
                    result.Add(dep);
                    result = result.Union(DependenyArray(dep), StringComparer.InvariantCultureIgnoreCase).ToList();
                }
            }
            return result;
        }


        private void PrintFlatDependecies()
        {
            foreach (var item in flatDependecies.OrderBy(x => x.Value.Count).ThenBy(n => n.Key))
            {
                Console.WriteLine($"{item.Key} : {item.Value.Count}");
                foreach (string dep in item.Value)
                {
                    Console.WriteLine($"\t{dep}");
                }
                Console.WriteLine(String.Empty);
            }
        }

        private void Analyze(string projectFile)
        {
            string pName = Path.GetFileNameWithoutExtension(projectFile);
            using (XmlReader pReader = XmlReader.Create(projectFile))
            {
                while (pReader.Read())
                {
                    if (pReader.IsStartElement())
                    {
                        switch (pReader.Name.ToLower())
                        {
                            case "reference":
                            case "packagereference":
                                string include = pReader["Include"];
                                if (include != null)
                                {
                                    AddDependency(pName, include);
                                }
                                break;
                        }
                    }
                }
            }

        }

        private void AddDependency(string pName, string include)
        {
            List<string> includes;
            if (!dependecies.ContainsKey(pName))
            {
                includes = new List<string>();
                dependecies.Add(pName, includes);
            }
            else
            {
                includes = dependecies[pName];
            }
            if (!includes.Contains(include))
            {
                includes.Add(include);
            }
        }
    }
}
