using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorConfigInCode
{
    public class ConfigSettings
    {
        public ConnectionSection TargetAppearanceConfiguration
        {
            get
            {
                return (ConnectionSection)ConfigurationManager.GetSection("deployTargetSection");
            }
        }

        public TargetAppearanceCollection TargetApperances
        {
            get
            {
                return this.TargetAppearanceConfiguration.DeployTarget;
            }
        }

        public IEnumerable<DeployTarget> DeployTargets
        {
            get
            {
                foreach (DeployTarget selement in this.TargetApperances)
                {
                    if (selement != null)
                        yield return selement;
                }
            }
        }
    }
}
