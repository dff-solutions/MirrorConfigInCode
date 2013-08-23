using System.Configuration;

namespace MirrorConfigInCode
{
    public class DeployTarget : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("isactive", DefaultValue = "true", IsKey = false, IsRequired = false)]
        public bool isactive
        {
            get { return (bool)base["isactive"]; }
            set { base["isactive"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("servername", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string servername
        {
            get { return (string)base["servername"]; }
            set { base["servername"] = value; }
        }
    }
}
