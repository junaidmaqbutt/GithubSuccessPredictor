using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubApiFetcher
{
    [Serializable]
    class ProjectDetails
    {
        public ProjectDetails(Item project)
        {
            Project = project;
            Subscribers = 0;
            Commits = 0;
            Contributers = 0;
            Branches = 0;
            ReleaseCount = 0;
            IssuesCount = 0;
            PullRequests = 0;
        }
        public Item Project { set; get; }
        public string Language { get { return Project.language; } }
        public int Commits { get; set; }
        public int Contributers { get; set; }
        public int Subscribers { get; set; }
        public  int Stars { get { return Project.stargazers_count; } }
        public int Branches { get; set; }
        public int Forks { get { return Project.forks; } }
        public int ReleaseCount { get; set; }
        public int OpenIssuesCount {  get { return Project.open_issues_count; } }
        public int IssuesCount { get; set; }
        public int PullRequests { get; set; }

    }
    [Serializable]
    class ProjectDetailsList
    {

        public List<ProjectDetails> ListOfAllProjects { get; set; }
    }
}
