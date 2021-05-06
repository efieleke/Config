using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sayer.Config.Test
{
    [TestClass]
    public class SettingsTest
    {
        private class Base { }
        private class Derived : Base { }

        [TestMethod]
        public void TestSectionMissing()
        {
            Settings settings = new SettingsTester(new ConfigSection(), "rootSection", typeof(Derived));

            try
            {
                settings.Get<int>("Missing");
                Assert.Fail("Did not expect to get here");
            }
            catch (SettingsPropertyNotFoundException e)
            {
                Assert.AreEqual("Setting 'Missing' not found for type Sayer.Config.Test.SettingsTest+Derived. Section name is 'rootSection'", e.Message);
            }
        }

        [TestMethod]
        public void TestClassHeirarchy()
        {
            string rootName = "Root";
            string baseName = typeof(Base).Name;
            string derivedName = typeof(Derived).Name;

            var mockedSection = new ConfigSection();

            // Ensure coverage of the plumbing classes around typed fields.
            var field = new FieldElement { Name = "SettingOne", Value = "boot" };
            mockedSection.Fields.Add(field);
            mockedSection.Fields.Remove(field);
            Assert.AreEqual(0, mockedSection.Fields.Count);
            mockedSection.Fields.Add(field);
            Assert.AreEqual("SettingOne", mockedSection.Fields["SettingOne"].Name);
            Assert.AreEqual("boot", mockedSection.Fields["SettingOne"].Value);
            mockedSection.Fields.Clear();
            Assert.AreEqual(0, mockedSection.Fields.Count);
            mockedSection.Fields.Add(field);
            mockedSection.Fields.Remove("SettingOne");
            Assert.AreEqual(0, mockedSection.Fields.Count);
            mockedSection.Fields.Add(field);
            mockedSection.Fields.Remove(field);
            Assert.AreEqual(0, mockedSection.Fields.Count);
            mockedSection.Fields.Add(field);
            mockedSection.Fields[field.Name] = new FieldElement { Name = "SettingOne", Value = rootName };
            Assert.AreEqual(1, mockedSection.Fields.Count);
            Assert.AreEqual(rootName, mockedSection.Fields["SettingOne"].Value);

            mockedSection.Fields.Add(new FieldElement { Name = "Base.SettingTwo", Value = baseName });
            mockedSection.Fields.Add(new FieldElement { Name = "Base.Derived.SettingThree", Value = derivedName });
            mockedSection.Fields.Add(new FieldElement { Name = "SettingFour", Value = rootName });
            mockedSection.Fields.Add(new FieldElement { Name = "Base.SettingFour", Value = baseName });
            mockedSection.Fields.Add(new FieldElement { Name = "SettingFive", Value = rootName });
            mockedSection.Fields.Add(new FieldElement { Name = "Base.SettingFive", Value = baseName });
            mockedSection.Fields.Add(new FieldElement { Name = "Base.Derived.SettingFive", Value = derivedName });
            mockedSection.Fields.Add(new FieldElement { Name = "SettingSix", Value = rootName });
            mockedSection.Fields.Add(new FieldElement { Name = "Base.Derived.SettingSix", Value = derivedName });

            Settings settings = new SettingsTester(mockedSection, "rootSection", typeof(Derived));
            Assert.AreEqual(rootName, settings.Get<string>("SettingOne"));
            Assert.AreEqual(baseName, settings.Get<string>("SettingTwo"));
            Assert.AreEqual(derivedName, settings.Get<string>("SettingThree"));
            Assert.AreEqual(baseName, settings.Get<string>("SettingFour"));
            Assert.AreEqual(derivedName, settings.Get<string>("SettingFive"));
            Assert.AreEqual(derivedName, settings.Get<string>("SettingSix"));
            new SettingsTester(mockedSection, "rootSection", typeof(Derived)).Set("SettingSix", "different");
            Assert.AreEqual("different", settings.Get<string>("SettingSix"));
            settings.Set("SettingSix", derivedName);
            Assert.AreEqual(derivedName, settings.Get<string>("SettingSix"));

            Assert.AreEqual(rootName, new SettingsTester(mockedSection, "rootSection", typeof(Derived)).Get<string>("SettingOne"));
            Assert.AreEqual(baseName, new SettingsTester(mockedSection, "rootSection", typeof(Base)).Get<string>("SettingTwo"));
            Assert.AreEqual(baseName, new SettingsTester(mockedSection, "rootSection", typeof(Base)).Get<string>("SettingFive"));
            Assert.AreEqual(rootName, new SettingsTester(mockedSection, "rootSection").Get<string>("SettingOne"));

            new SettingsTester(mockedSection, "rootSection").Set("SettingOne", 12);
            Assert.AreEqual(12, settings.Get<int>("SettingOne"));
            new SettingsTester(mockedSection, "rootSection").Set("SettingOne", rootName);

            try
            {
                Assert.AreEqual(rootName, new SettingsTester(mockedSection, "rootSection").Get<string>("SettingTwo"));
                Assert.Fail("Did not expect to get here");
            }
            catch (SettingsPropertyNotFoundException e)
            {
                Assert.AreEqual("Setting 'SettingTwo' not found for type null. Section name is 'rootSection'", e.Message);
            }

            try
            {
                settings.Get<int>("Missing");
                Assert.Fail("Did not expect to get here");
            }
            catch (SettingsPropertyNotFoundException e)
            {
                Assert.AreEqual("Setting 'Missing' not found for type Sayer.Config.Test.SettingsTest+Derived. Section name is 'rootSection'", e.Message);
            }

            try
            {
                settings.Get<int>("SettingOne");
                Assert.Fail("Did not expect to get here");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Root is not a valid value for Int32.", e.Message);
            }
        }

        [TestMethod]
        public void TestValueType()
        {
            var mockedSection = new ConfigSection();
            mockedSection.Fields.Add(new FieldElement { Name = "Object.ValueType.Int32.SettingOne", Value = "foo" });
            Settings settings = new SettingsTester(mockedSection, "blah", typeof(int));
            Assert.AreEqual("foo", settings.Get<string>("SettingOne"));
        }

        private class SettingsTester : Settings
        {
            internal SettingsTester(ConfigSection configSection, string sectionName) : this(configSection, sectionName, null) {}


            internal SettingsTester(ConfigSection configSection, string sectionName, Type type) : base(sectionName, type)
            {
                _configSection = configSection;
            }

            protected override ConfigSection GetConfigSection() => _configSection;
            private readonly ConfigSection _configSection;
        }
    }
}
