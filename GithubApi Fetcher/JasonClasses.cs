using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubApiFetcher
{


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class smscorpus
    {

        private smscorpusSms[] smsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sms")]
        public smscorpusSms[] sms
        {
            get
            {
                return this.smsField;
            }
            set
            {
                this.smsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class smscorpusSms
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("class", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("date", typeof(System.DateTime), DataType = "date")]
        [System.Xml.Serialization.XmlElementAttribute("number", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("source", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("text", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        @class,

        /// <remarks/>
        date,

        /// <remarks/>
        number,

        /// <remarks/>
        source,

        /// <remarks/>
        text,
    }


    [Serializable]
    public class Projects
    {
        public int total_count { get; set; }
        public bool incomplete_results { get; set; }
        public Item[] items { get; set; }
    }
    [Serializable]
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public Owner owner { get; set; }
        public bool _private { get; set; }
        public string html_url { get; set; }
        public string description { get; set; }
        public bool fork { get; set; }
        public string url { get; set; }
        public string forks_url { get; set; }
        public string keys_url { get; set; }
        public string collaborators_url { get; set; }
        public string teams_url { get; set; }
        public string hooks_url { get; set; }
        public string issue_events_url { get; set; }
        public string events_url { get; set; }
        public string assignees_url { get; set; }
        public string branches_url { get; set; }
        public string tags_url { get; set; }
        public string blobs_url { get; set; }
        public string git_tags_url { get; set; }
        public string git_refs_url { get; set; }
        public string trees_url { get; set; }
        public string statuses_url { get; set; }
        public string languages_url { get; set; }
        public string stargazers_url { get; set; }
        public string contributors_url { get; set; }
        public string subscribers_url { get; set; }
        public string subscription_url { get; set; }
        public string commits_url { get; set; }
        public string git_commits_url { get; set; }
        public string comments_url { get; set; }
        public string issue_comment_url { get; set; }
        public string contents_url { get; set; }
        public string compare_url { get; set; }
        public string merges_url { get; set; }
        public string archive_url { get; set; }
        public string downloads_url { get; set; }
        public string issues_url { get; set; }
        public string pulls_url { get; set; }
        public string milestones_url { get; set; }
        public string notifications_url { get; set; }
        public string labels_url { get; set; }
        public string releases_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime pushed_at { get; set; }
        public string git_url { get; set; }
        public string ssh_url { get; set; }
        public string clone_url { get; set; }
        public string svn_url { get; set; }
        public string homepage { get; set; }
        public int size { get; set; }
        public int stargazers_count { get; set; }
        public int watchers_count { get; set; }
        public string language { get; set; }
        public bool has_issues { get; set; }
        public bool has_downloads { get; set; }
        public bool has_wiki { get; set; }
        public bool has_pages { get; set; }
        public int forks_count { get; set; }
        public object mirror_url { get; set; }
        public int open_issues_count { get; set; }
        public int forks { get; set; }
        public int open_issues { get; set; }
        public int watchers { get; set; }
        public string default_branch { get; set; }
        public float score { get; set; }
    }
    [Serializable]
    public class Owner
    {
        public string login { get; set; }
        public int id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }
    [Serializable]
    public class RateCheck
    {
        public Resources resources { get; set; }
        public Rate rate { get; set; }
    }
    [Serializable]
    public class Resources
    {
        public Core core { get; set; }
        public Search search { get; set; }
    }
    [Serializable]
    public class Core
    {
        public int limit { get; set; }
        public int remaining { get; set; }
        public int reset { get; set; }
    }
    [Serializable]
    public class Search
    {
        public int limit { get; set; }
        public int remaining { get; set; }
        public int reset { get; set; }
    }
    [Serializable]
    public class Rate
    {
        public int limit { get; set; }
        public int remaining { get; set; }
        public int reset { get; set; }
    }


}
