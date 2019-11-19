using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace analyzeCaveDeps
{
    public struct CaveProject
    {
        public string Name;
        public string FileName;
        public string Solution;
        public string Version;
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

        private CaveProject FillProjectData(string projectFile)
        {
            CaveProject cProject = new CaveProject { FileName = projectFile };
            cProject.Solution = Path.GetFileName(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(cProject.FileName), "..")));
            cProject.Name = cProject.Solution + "/" + Path.GetFileNameWithoutExtension(cProject.FileName);
            return cProject;
        }
    }
}
