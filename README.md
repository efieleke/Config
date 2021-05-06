Config
=========

Class for retrieving settings from app.config, optionally related to a type. A separate project exists for web.config files.

Below is an example for a config file, which assumes the existence of class Base, as well as classes Buggy and Stable, which inherit from Base (all in the same assembly). In the example, type Buggy has log level Debug, Stable has level Error, and all other derivatives of Base have level Warn. Classes not inheriting from Base have level Info. All types have Timestamp set to true in the example.

```
<configuration>
  <configSections>
    <section name = "logSection" type="Sayer.Config.ConfigSection,Sayer.Config"/>
  </configSections>
  <logSection>
    <field name="LogLevel" value="Info"/>
    <field name="Base.LogLevel" value="Warn"/>
    <field name="Base.Buggy.LogLevel" value="Debug"/>
    <field name="Base.Stable.LogLevel" value="Error"/>
    <field name="Timestamp" value="true"/>
  </logSection>
<configuration/>
```

The following statement, given the .config settings below, would return the value Debug:
AppSettings.Get<LogLevel>("logSection", typeof(Buggy), "LogLevel");

The following statement would return the value Info:
AppSettings.Get<LogLevel>("logSection", "LogLevel");

And the following statement would return true:
AppSettings.Get<bool>("logSection", typeof(MyClass), "Timestamp")

Author
----
Eric Fieleke
