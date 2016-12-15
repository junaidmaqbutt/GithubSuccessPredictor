using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GithubApiFetcher
{
    class Program
    {
        static string BaseURL = "https://api.github.com/";

        static string[] AllAccessToken = {
            "","","","","",""
        ,};

        static List<string> AccessToken = new List<string>();
        static int TOkenID = 0;
        static string SearchQ = "search/repositories?q=language:";
        static string ReleaseP = "/repos/releases";
        static string[] Languages = {"C++", "HTML","Java","JavaScript", "PHP"};
        static List<Projects> ListOfProjects = new List<Projects>();
        static void Main(string[] args)
        {
            //GETDBDATA();


            //FetchAllProjectsbasedOnLanguages();

            Console.WriteLine(" * *****************");
            Console.WriteLine("C++ , HTML , Java , JavaScript , PHP ");
            Console.WriteLine("******************");
            Console.Write("Enter Language : ");
            //string Input = Console.ReadLine();
            foreach (string Language in Languages)
                ListOfProjects.Add(ByteArrayToClass(
                        File.ReadAllBytes(
                                Language == "C++" ? "C" : Language)
                            , typeof(Projects))
                        as Projects
                        );
            //To enable Breaks Withing Execution
            //***********************************
            //var myList = new List<Item>();
            //myList.AddRange(ListOfProjects[0].items);
            //Console.WriteLine("REMOVAL DETAILS ");
            //Console.Write("Enter Start Index : ");
            //int StartIndex = int.Parse( Console.ReadLine());
            //Console.Write("Enter Count : ");
            //int EndCOunt = int.Parse(Console.ReadLine());
            //myList.RemoveRange(StartIndex, EndCOunt);
            //ListOfProjects[0].items = myList.ToArray();
            //myList.Clear();
            //***********************************
            ProjectDetailsList PDL = new ProjectDetailsList();
            PDL.ListOfAllProjects = new List<ProjectDetails>();
            if (File.Exists("data.bin"))
                using (Stream stream = File.Open("data.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    PDL = (ProjectDetailsList)bin.Deserialize(stream);
                }
            AccessToken.Clear();
            
            //SaveToCSV(PDL);


            for (int ID = 0; ID < AllAccessToken.Length; ID++)
            {
                RateCheck Temp1 = GetJasonData<RateCheck>("https://api.github.com/rate_limit" + AllAccessToken[ID]);
                if (!(Temp1 == null || Temp1.rate == null || Temp1.rate.remaining <= 7))
                    AccessToken.Add(AllAccessToken[ID]);
            }
            Parallel.ForEach(ListOfProjects, (LanguageProjects) =>
            {
                Console.WriteLine(LanguageProjects.items[0].language);
                Parallel.ForEach(LanguageProjects.items, (Project) =>
                {
                    Console.WriteLine(Project.url);
                    if (PDL.ListOfAllProjects.FindIndex(X => X.Project.url == Project.url) == -1)
                    {
                        Random Rand = new Random(Environment.TickCount);
                        ProjectDetails tempProject = new ProjectDetails(Project);
                        int PageNum = 1;
                        bool HasSubs = true, HasCommi = true, HasContri = true, HasBranch = true,
                            HasRelease = true, HasIssue = true, HasPull = true;
                        do
                        {
                            do
                            {
                                RateCheck Temp1 = GetJasonData<RateCheck>("https://api.github.com/rate_limit" + AccessToken[TOkenID]);
                                if (Temp1.rate == null)
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Api");
                                    continue;
                                }
                                if (Temp1 == null || Temp1.rate == null || Temp1.rate.remaining <= 7)
                                {
                                    do
                                    {
                                        Thread.Sleep(Rand.Next(3000));
                                        AccessToken.Clear();
                                        for (int ID = 0; ID < AllAccessToken.Length; ID++)
                                        {
                                            Temp1 = GetJasonData<RateCheck>("https://api.github.com/rate_limit" + AllAccessToken[ID]);
                                            if (!(Temp1 == null || Temp1.rate == null || Temp1.rate.remaining <= 7))
                                                AccessToken.Add(AllAccessToken[ID]);
                                        }
                                        TOkenID = 0;
                                        Console.WriteLine("API KEY CHANGE KEYS LEFT " + AccessToken.Count);
                                    } while (AccessToken.Count == 0);
                                }
                                else break;
                            } while (true);
                            int Count = 0;
                            while (HasSubs)
                            {
                                try
                                {
                                    dynamic TempSubs = GetJasonData<Object>(Project.subscribers_url + AccessToken[TOkenID] + "&per_page=100&page=" + PageNum);
                                    if (TempSubs != null && TempSubs.Count != null && TempSubs.Count != 0)
                                    {
                                        tempProject.Subscribers += TempSubs.Count;
                                        break;
                                    }
                                    else
                                        HasSubs = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Subscribers " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasCommi)
                            {
                                try
                                {
                                    dynamic TempCommi = GetJasonData<Object>(Project.commits_url.Replace("{/sha}", "") + AccessToken[TOkenID] + "&per_page=100&page=" + PageNum);
                                    if (TempCommi != null && TempCommi.Count != null && TempCommi.Count != 0)
                                    {
                                        tempProject.Commits += TempCommi.Count;
                                        break;
                                    }
                                    else
                                        HasCommi = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Commits " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasContri)
                            {
                                try
                                {
                                    dynamic TempContri = GetJasonData<Object>(Project.contributors_url + AccessToken[TOkenID] + "&per_page=100&page=" + PageNum);
                                    if (TempContri != null && TempContri.Count != null && TempContri.Count != 0)
                                    {
                                        tempProject.Contributers += TempContri.Count;
                                        break;
                                    }
                                    else
                                        HasContri = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Contributers " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasBranch)
                            {
                                try
                                {
                                    dynamic TempBranch = GetJasonData<Object>(Project.branches_url.Replace("{/branch}", "") + AccessToken[TOkenID] + "&per_page=100&page=" + PageNum);
                                    if (TempBranch != null && TempBranch.Count != null && TempBranch.Count != 0)
                                    {
                                        tempProject.Branches += TempBranch.Count;
                                        break;
                                    }
                                    else
                                        HasBranch = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Branch " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasRelease)
                            {
                                try
                                {
                                    dynamic TempRelease = GetJasonData<Object>(Project.releases_url.Replace("{/id}", "") + AccessToken[TOkenID] + "&per_page=100&page=" + PageNum);
                                    if (TempRelease != null && TempRelease.Count != null && TempRelease.Count != 0)
                                    {
                                        tempProject.ReleaseCount += TempRelease.Count;
                                        break;
                                    }
                                    else
                                        HasRelease = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Release " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasIssue)
                            {

                                try
                                {
                                    dynamic TempIssue = GetJasonData<Object>(Project.issues_url.Replace("{/number}", "") + AccessToken[TOkenID] + "&per_page=100&state=all&page=" + PageNum);
                                    if (TempIssue != null && TempIssue.Count != null && TempIssue.Count != 0)
                                    {
                                        tempProject.IssuesCount += TempIssue.Count;
                                        break;
                                    }
                                    else
                                        HasIssue = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Issue " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }
                                }
                            }
                            Count = 0;
                            while (HasPull)
                            {
                                try
                                {
                                    dynamic TempPull = GetJasonData<Object>(Project.pulls_url.Replace("{/number}", "") + AccessToken[TOkenID] + "&per_page=100&state=all&page=" + PageNum);
                                    if (TempPull != null && TempPull.Count != null && TempPull.Count != 0)
                                    {
                                        tempProject.PullRequests += TempPull.Count;
                                        break;
                                    }
                                    else
                                        HasPull = false;
                                }
                                catch
                                {
                                    Thread.Sleep(Rand.Next(3000));
                                    Console.WriteLine("Fail Pull " + Count);
                                    Count++;
                                    if (Count > 3)
                                    {
                                        break;
                                    }

                                }
                            }
                            PageNum++;
                            if (!(HasSubs || HasCommi || HasContri || HasBranch ||
                            HasRelease || HasIssue || HasPull))
                                break;
                        } while (true);
                        PDL.ListOfAllProjects.Add(tempProject);
                        while (true)
                        {
                            try
                            {
                                using (Stream stream = File.Open("data.bin", FileMode.Create))
                                {
                                    BinaryFormatter bin = new BinaryFormatter();
                                    bin.Serialize(stream, PDL);
                                }
                                Console.WriteLine("Done " + PDL.ListOfAllProjects.Count);
                                break;
                            }
                            catch { Thread.Sleep(100); }
                        }
                    }
                });
            });
        }

        private static void SaveToCSV(ProjectDetailsList PDL)
        {
            List<string> AllLinesToWrite = new List<string>();
            string Line = "Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL";
            AllLinesToWrite.Add(Line);
            foreach (ProjectDetails P in PDL.ListOfAllProjects.FindAll(X => X.Language.ToUpper() == "HTML"))
            {
                Line = P.Project.full_name.Split('/')[0] + "," +
                    P.Project.name + "," +
                    P.Language + "," +
                    P.Contributers + "," +
                    P.Commits + "," +
                    P.Stars + "," +
                    P.Forks + "," +
                    P.Branches + "," +
                    P.Subscribers + "," +
                    P.PullRequests + "," +
                    P.IssuesCount + "," +
                    P.OpenIssuesCount + "," +
                    P.Project.has_downloads + "," +
                    P.ReleaseCount + "," +
                    P.Project.url.Replace("/repos/", "/").Replace("api.", "");
                AllLinesToWrite.Add(Line);
            }
            File.WriteAllLines("DataSet(HTML).csv", AllLinesToWrite);
            AllLinesToWrite.Clear();
            AllLinesToWrite.Add("Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL");
            foreach (ProjectDetails P in PDL.ListOfAllProjects.FindAll(X => X.Language.ToUpper() == "C"))
            {
                Line = P.Project.full_name.Split('/')[0] + "," +
                    P.Project.name + "," +
                    P.Language + "," +
                    P.Contributers + "," +
                    P.Commits + "," +
                    P.Stars + "," +
                    P.Forks + "," +
                    P.Branches + "," +
                    P.Subscribers + "," +
                    P.PullRequests + "," +
                    P.IssuesCount + "," +
                    P.OpenIssuesCount + "," +
                    P.Project.has_downloads + "," +
                    P.ReleaseCount + "," +
                    P.Project.url.Replace("/repos/", "/").Replace("api.", "");
                AllLinesToWrite.Add(Line);
            }
            File.WriteAllLines("DataSet(C).csv", AllLinesToWrite);
            AllLinesToWrite.Clear();
            AllLinesToWrite.Add("Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL");
            foreach (ProjectDetails P in PDL.ListOfAllProjects.FindAll(X => X.Language.ToUpper() == "PHP"))
            {
                Line = P.Project.full_name.Split('/')[0] + "," +
                    P.Project.name + "," +
                    P.Language + "," +
                    P.Contributers + "," +
                    P.Commits + "," +
                    P.Stars + "," +
                    P.Forks + "," +
                    P.Branches + "," +
                    P.Subscribers + "," +
                    P.PullRequests + "," +
                    P.IssuesCount + "," +
                    P.OpenIssuesCount + "," +
                    P.Project.has_downloads + "," +
                    P.ReleaseCount + "," +
                    P.Project.url.Replace("/repos/", "/").Replace("api.", "");
                AllLinesToWrite.Add(Line);
            }
            File.WriteAllLines("DataSet(PHP).csv", AllLinesToWrite);
            AllLinesToWrite.Clear();
            AllLinesToWrite.Add("Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL");
            foreach (ProjectDetails P in PDL.ListOfAllProjects.FindAll(X => X.Language.ToUpper() == "JAVA"))
            {
                Line = P.Project.full_name.Split('/')[0] + "," +
                    P.Project.name + "," +
                    P.Language + "," +
                    P.Contributers + "," +
                    P.Commits + "," +
                    P.Stars + "," +
                    P.Forks + "," +
                    P.Branches + "," +
                    P.Subscribers + "," +
                    P.PullRequests + "," +
                    P.IssuesCount + "," +
                    P.OpenIssuesCount + "," +
                    P.Project.has_downloads + "," +
                    P.ReleaseCount + "," +
                    P.Project.url.Replace("/repos/", "/").Replace("api.", "");
                AllLinesToWrite.Add(Line);
            }
            File.WriteAllLines("DataSet(JAVA).csv", AllLinesToWrite);
            AllLinesToWrite.Clear();
            AllLinesToWrite.Add("Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL");
            foreach (ProjectDetails P in PDL.ListOfAllProjects.FindAll(X => X.Language.ToUpper() == "JAVASCRIPT"))
            {
                Line = P.Project.full_name.Split('/')[0] + "," +
                    P.Project.name + "," +
                    P.Language + "," +
                    P.Contributers + "," +
                    P.Commits + "," +
                    P.Stars + "," +
                    P.Forks + "," +
                    P.Branches + "," +
                    P.Subscribers + "," +
                    P.PullRequests + "," +
                    P.IssuesCount + "," +
                    P.OpenIssuesCount + "," +
                    P.Project.has_downloads + "," +
                    P.ReleaseCount + "," +
                    P.Project.url.Replace("/repos/", "/").Replace("api.", "");
                AllLinesToWrite.Add(Line);
            }
            File.WriteAllLines("DataSet(JAVASCRIPT).csv", AllLinesToWrite);
            AllLinesToWrite.Clear();
            AllLinesToWrite.Add("Authors,ProjectName,Language,Contributers,Commits,Stars,Forks,Branches,Watchers,PullRequests,TotalIssues,OpenIssues,HasDownloads,ReleaseCount,URL");
        }

        private static void GETDBDATA()
        {
            Database.DBConnect DB = new Database.DBConnect("", "", "", "");
            List<string[]> List = new List<string[]>();
            List.AddRange(DB.ExecuteQuery("Dataset", "SELECT Sum(`Contributers`),Sum(`Commits`),Sum(`Forks`),Sum(`Branches`),Sum(`Watchers`),Sum(`PullRequests`),Sum(`TotalIssues`),Sum(`OpenIssues`),Sum(`ReleaseCounts`) FROM `Dataset` WHERE `Language`=\"C\";"));
            List.AddRange(DB.ExecuteQuery("Dataset", "SELECT Sum(`Contributers`),Sum(`Commits`),Sum(`Forks`),Sum(`Branches`),Sum(`Watchers`),Sum(`PullRequests`),Sum(`TotalIssues`),Sum(`OpenIssues`),Sum(`ReleaseCounts`) FROM `Dataset` WHERE `Language`=\"PHP\";"));
            List.AddRange(DB.ExecuteQuery("Dataset", "SELECT Sum(`Contributers`),Sum(`Commits`),Sum(`Forks`),Sum(`Branches`),Sum(`Watchers`),Sum(`PullRequests`),Sum(`TotalIssues`),Sum(`OpenIssues`),Sum(`ReleaseCounts`) FROM `Dataset` WHERE `Language`=\"Java\";"));
            List.AddRange(DB.ExecuteQuery("Dataset", "SELECT Sum(`Contributers`),Sum(`Commits`),Sum(`Forks`),Sum(`Branches`),Sum(`Watchers`),Sum(`PullRequests`),Sum(`TotalIssues`),Sum(`OpenIssues`),Sum(`ReleaseCounts`) FROM `Dataset` WHERE `Language`=\"JavaScript\";"));
            List.AddRange(DB.ExecuteQuery("Dataset", "SELECT Sum(`Contributers`),Sum(`Commits`),Sum(`Forks`),Sum(`Branches`),Sum(`Watchers`),Sum(`PullRequests`),Sum(`TotalIssues`),Sum(`OpenIssues`),Sum(`ReleaseCounts`) FROM `Dataset` WHERE `Language`=\"HTML\";"));


            string Text = "Sum(`Contributers`)\tSum(`Commits`)\tSum(`Forks`)\tSum(`Branches`)\tSum(`Watchers`)\tSum(`PullRequests`)\tSum(`TotalIssues`)\tSum(`OpenIssues`)\tSum(`ReleaseCounts`)\n";
            foreach (string[] Lines in List)
            {
                foreach (string Num in Lines)
                    Text += Num + "\t\t\t";
                Text += "\n";
            }
            File.WriteAllText("00.txt", Text);

        }

        static private void FetchAllProjectsbasedOnLanguages()
        {
            foreach (string Language in Languages)
            {
                Random Rand = new Random(Environment.TickCount);
                var temp = BaseURL + SearchQ + Language + AccessToken[TOkenID] + "&per_page=100&page=";
                Item[] TempArray;
                Projects Temp = GetJasonData<Projects>(temp + "1");
                while (Temp.items == null)
                {
                    System.Threading.Thread.Sleep(Rand.Next(3000));
                    Temp = GetJasonData<Projects>(temp + "1");
                }
                TempArray = Temp.items;
                for (int PageNum = 2; PageNum < 11; PageNum++)
                {
                    int OrignalLength = TempArray.Length;
                    var TempNext = GetJasonData<Projects>(temp + PageNum.ToString());
                    int count = 0;
                    while (TempNext == null || TempNext.items == null)
                    {
                        if (count > 10) break;
                        System.Threading.Thread.Sleep(Rand.Next(3000));
                        TempNext = GetJasonData<Projects>(temp + PageNum.ToString());
                        count++;
                    }
                    if (TempNext == null || TempNext.items == null) continue;
                    Array.Resize<Item>(ref TempArray, TempArray.Length + TempNext.items.Length);
                    Array.Copy(TempNext.items, 0, TempArray, OrignalLength, TempNext.items.Length);
                }
                Temp.items = TempArray;
                ListOfProjects.Add(Temp);
            }
            foreach (Projects P in ListOfProjects)
            {
                File.WriteAllBytes(P.items[0].language, ClassToByteArray(P));
            }
        }
        private static T GetJasonData<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    w.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)");
                    w.Headers.Add("Accept", "application/json");
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }
        private static byte[] ClassToByteArray(Object objClass)
        {
            try
            {
                String strXML = null;
                MemoryStream ms = new MemoryStream();
                XmlSerializer xmlS = new XmlSerializer(objClass.GetType());
                XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);

                xmlS.Serialize(xmlTW, objClass);
                ms = (MemoryStream)xmlTW.BaseStream;

                return ms.ToArray();
            }
            catch
            {
            }
            return null;
        }
        private static Object ByteArrayToClass(byte[] buffer, Type objectType)
        {
            try
            {
                XmlSerializer xmlS = new XmlSerializer(objectType);
                MemoryStream ms = new MemoryStream(buffer);
                XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);

                return xmlS.Deserialize(ms);
            }
            catch
            {
            }
            return null;
        }
    }


}
