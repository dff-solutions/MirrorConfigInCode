#MirrorConfigInCode
==================

Kleines Beispielprojekt in dem gezeigt wird wie man eigene Config Sections im der .config erstellen kann
und sie im .Net auslesen/benutzen kann


------------------------------------------------------------------------------------------------------------------------
##PROBLEM: 
Mehrere Settingkeys aus einer Config auslesen ist nervig weil man jeden Key den man braucht direkt 
per Namen im Code ansprechen muss.  

Bsp.: 

Config: 
```xml
<appSettings>   
    <add key="target1" value="192.168.2.137"/>
    <add key="target2" value="192.168.2.136"/>    
</appSettings>
```

Code:
```cs
String target1 = ConfigurationSettings.AppSettings["target1"];
String target2 = ConfigurationSettings.AppSettings["target2"];
```

------------------------------------------------------------------------------------------------------------------------
##BESSER:
Leichter zu pflegen waere eine Ansatz mit dem es moeglich waere Keys zusammenhaengend in der config anzugeben. 
Code seitig wuerde man dann eine Enumeration bekommen. 
Jedes der ConfigELemente kann Attribute haben die KlassenSeitig interpretiert werden koennen.

Bsp.:

Config:
```xml
<deployTargetSection>
	<Targets>
		<Target name="Netto: 01TTComT-01V" path="D:\Intranet" isactive="true" servername="localhost"/>
		<Target name="Netto: 01TTComT-02V" path="\\01TTCOMT-02V\Intranet" isactive="true" servername="\\01TTCOMT-02V"/>
	</Targets>
</deployTargetSection>
```

Code:
```cs
ConfigSettings settings = new ConfigSettings();

foreach (var t in settings.DeployTargets)
{
	strb.Add(string.Format(" Target Name : {0} ; Path : {1}", t.name, t.path));
	Console.WriteLine(strb.Last());
}
```

##AUFBAU:
Zunaechst brauche wir eine Klasse die unsere Targets abbilden kann.  
Diese Klasse interpretiert die Attribute der einzelnen Positionen.
```cs
// Mapped in unserem Bsp. ein Target
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
```


Eine Klasse die collection abbildet:
// Mapped in unserem Bsp. quasi die Node <Targets></Targets>

```cs
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
```

Eine kleine Hilfsklasse:
```cs
public class ConnectionSection : ConfigurationSection
{
	[ConfigurationProperty("Targets")]
	public TargetAppearanceCollection DeployTarget
	{
	    get { return ((TargetAppearanceCollection)(base["Targets"])); }
	    set { base["Targets"] = value; }
	}
}
```
	
Und zu guter letzt eine Klasse die das alles miteinander verknuepft und unseren direkten Draht zu den Settings darstellt.
```cs
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
```

##BENUTZUNG:
```cs
var config = new ConfigSettings();
foreach (var target in config.DeployTargets)
{
    Console.WriteLine(target.name);
}
```

##GEHTNICHT?!:
####Fehler: 
Unbehandelte Ausnahme: System.Configuration.ConfigurationErrorsException: Das Konfigurationssystem konnte nicht initialisiert werden. ---> System.Configuration. ConfigurationErrorsException: Unbekannter Konfigurationsabschnitt "deployTargetS ection". 

###Lösung: 
Moeglicherweise hast du vergessen die ConfigSection in der Config Dateu bekannt zu machen. Ja das ist noetig! ;) Regelt vermutlich den Zugriff aus den Namespaces herraus.

```xml
<configSections>
	<section name="deployTargetSection" type="MirrorConfigInCode.ConnectionSection, MirrorConfigInCode"/>
</configSections>
```
----------------------

###Fehler:
Unbehandelte Ausnahme: System.Configuration.ConfigurationErrorsException: Das Konfigurationssystem konnte nicht initialisiert werden. ---> System.Configuration.wConfigurationErrorsException: Pro Konfigurationsdatei ist nur ein <configSection s>-Element zulaessig und muss, sofern vorhanden, das erste untergeordnete Element des Stamm-<configuration>-Elements sein. 

###Lösung: 
Die Exception Message sagt es schon. Es darf nur eine <configSection></configSection> geben  und SIE MUSS DIREKT ZU ANFANG stehen direkt nach <configuration>

