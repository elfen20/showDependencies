using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace analyzeCaveDeps
{
    public struct CaveProject
    {
        public string Name;
        public string FileName;
        public string Solution;
        public string Version;
        public List<string> usedIn;
        public List<string> dependsOn;
    }

    public class DepAnalyzer
    {
        public string BasePath { get; private set; }
        public Dictionary<string, CaveProject> Projects { get; private set; }

        public DepAnalyzer(string directory)
        {
            BasePath = Path.Combine(directory);
            Projects = new Dictionary<string, CaveProject>();
            Analyze();
        }

        void Analyze()
        {
            FindProjectFiles();
        }

        void FindProjectFiles()
        {
            string[] projectFiles = Directory.GetFiles(BasePath, "*.csproj", SearchOption.AllDirectories);
            foreach (string project in projectFiles)
            {
                CaveProject cProject = FillProjectData(project);
                Projects.Add(cProject.Name, cProject);
            }
        }

        string FindVersion(string solutionName)
        {
            // todo-- check actual git commit
            string result = "CI";
            string gitTagDir = Path.Combine(BasePath, @".git\modules", solutionName, "refs","tags");
            if (Directory.Exists(gitTagDir))
            {
                string[] tagfiles = Directory.GetFiles(gitTagDir);
                if (tagfiles.Length > 0)
                {
                    result = Path.GetFileName(tagfiles[tagfiles.Length - 1]);
                }
            }
            return result;
        }

        private CaveProject FillProjectData(string projectFile)
        {
            CaveProject cProject = new CaveProject { FileName = projectFile };
            cProject.Solution = Path.GetFileName(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(cProject.FileName), "..")));
            cProject.Version = FindVersion(cProject.Solution);
            cProject.dependsOn = GetProjectDependencies(cProject.FileName);
            string pName = Path.GetFileNameWithoutExtension(cProject.FileName);
            cProject.Name = $"{cProject.Solution}/{pName}({cProject.Version})";
            return cProject;
        }

        private List<string> GetProjectDependencies(string fileName)
        {
            List<string> result = new List<string>();
            using (XmlReader pReader = XmlReader.Create(fileName))
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
                                string version = pReader["Version"];
                                if (version != null) { version = $"_{version}"; }
                                if (include != null)
                                {
                                    result.Add($"{include}{version}");
                                }
                                break;
                        }
                    }
                }
            }
            result.Sort();
            result = result.Distinct().ToList();
            return result;
        }
    }
}
