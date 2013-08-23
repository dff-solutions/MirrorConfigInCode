using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorConfigInCode
{
    public class ConnectionSection : ConfigurationSection
    {
        [ConfigurationProperty("Targets")]
        public TargetAppearanceCollection DeployTarget
        {
            get { return ((TargetAppearanceCollection)(base["Targets"])); }
            set { base["Targets"] = value; }
        }
    }
}
