using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Support.Web.Configuration;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EmployerUsers.Support.Web.Tests.Configuration
{
    [TestFixture]
    public class WebConfigurationTesting
    {
        [SetUp]
        public void Setup()
        {
            _unit = new WebConfiguration
            {
                EmployerUsersApi = new EmployerUsersApiConfiguration
                {
                    ApiBaseUrl = "--- configuration value goes here ---",
                    ClientId = "00000000-0000-0000-0000-000000000000",
                    ClientSecret = "--- configuration value goes here ---",
                    IdentifierUri = "--- configuration value goes here ---",
                    Tenant = "--- configuration value goes here ---",
                    ClientCertificateThumbprint = "--- configuration value goes here ---"
                },
                SiteValidator = new SiteValidatorSettings
                {
                    Audience = "--- configuration value goes here ---",
                    Scope = "--- configuration value goes here ---",
                    Tenant = "--- configuration value goes here ---"
                },
                AccountApi = new AccountApiConfiguration()
                {
                    Tenant = "--- configuration value goes here ---",
                    ApiBaseUrl = "--- configuration value goes here ---",
                    ClientId = "00000000-0000-0000-0000-000000000000",
                    ClientSecret = "--- configuration value goes here ---",
                    IdentifierUri = "--- configuration value goes here ---"
                }
            };
        }

        private const string SiteConfigFileName = "SFA.DAS.Support.EmployerUsers";

        private WebConfiguration _unit;

        [Test]
        public void ItShouldDeserialiseFaithfuly()
        {
            var json = JsonConvert.SerializeObject(_unit);
            Assert.IsNotNull(json);
            var actual = JsonConvert.DeserializeObject<WebConfiguration>(json);
            Assert.AreEqual(json, JsonConvert.SerializeObject(actual));
        }

        [Test]
        public void ItShouldDeserialize()
        {
            var json = JsonConvert.SerializeObject(_unit);
            Assert.IsNotNull(json);
            var actual = JsonConvert.DeserializeObject<WebConfiguration>(json);
            Assert.IsNotNull(actual);
        }


        [Test]
        public void ItShouldGenerateASchema()
        {
            var provider = new FormatSchemaProvider();
            var jSchemaGenerator = new JSchemaGenerator();
            jSchemaGenerator.GenerationProviders.Clear();
            jSchemaGenerator.GenerationProviders.Add(provider);
            var actual = jSchemaGenerator.Generate(typeof(WebConfiguration));


            Assert.IsNotNull(actual);
            // hack to leverage format as 'environmentVariable'
            var schemaString = actual.ToString().Replace($"\"format\":", "\"environmentVariable\":");
            Assert.IsNotNull(schemaString);
            File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.schema.json", schemaString);
        }

        [Test]
        public void ItShouldSerialize()
        {
            var json = JsonConvert.SerializeObject(_unit);
            Assert.IsFalse(string.IsNullOrWhiteSpace(json));


            File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.json", json);
        }
    }
}