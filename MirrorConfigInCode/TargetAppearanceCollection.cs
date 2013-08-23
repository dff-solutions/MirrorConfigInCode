using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorConfigInCode
{
    [ConfigurationCollection(typeof(DeployTarget))]
    public class TargetAppearanceCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Target";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }


        public override bool IsReadOnly()
        {
            return false;
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new DeployTarget();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DeployTarget)(element)).name;
        }

        public DeployTarget this[int idx]
        {
            get
            {
                return (DeployTarget)BaseGet(idx);
            }
        }
    }
}
