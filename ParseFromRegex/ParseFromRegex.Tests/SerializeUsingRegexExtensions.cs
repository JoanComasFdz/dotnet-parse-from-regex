using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ParseFromRegex.Tests
{
    public class SerializeUsingRegexExtensions
    {
        [Fact]
        public void ParseGroupNamesWithValues_ReturnsAllValues()
        {
            var input = "Connor, John (19)";
            var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\)";

            var values = input.ParseGroupNamesWithValues(pattern);

            Assert.True(values.ContainsKey("FirstName"));
            Assert.True(values.ContainsKey("LastName"));
            Assert.True(values.ContainsKey("Age"));

            Assert.Equal("John", values["FirstName"]);
            Assert.Equal("Connor", values["LastName"]);
            Assert.Equal("19", values["Age"]);
        }

        public record Person(string FirstName, string LastName, int Age);

        [Fact]
        public void DeserializeFromRegex_ReturnsInstanceWithAllValues()
        {
            var input = "Connor, John (19)";
            var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\)";

            var person = input.DeserializeFromRegex<Person>(pattern);

            Assert.NotNull(person);

            Assert.Equal("John", person.FirstName);
            Assert.Equal("Connor", person.LastName);
            Assert.Equal(19, person.Age);
        }

        public enum Sex
        {
            F,
            M
        }

        public record PersonWithSex(string FirstName, string LastName, int Age, Sex Sex);

        public class SexConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Sex);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }

                var s = (string)JToken.Load(reader);

                var customerPatientGender = s == "Male"
                    ? Sex.M
                    : Sex.F;
                return customerPatientGender;
            }

            public override bool CanWrite => false;

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void DeserializeFromRegex_WithCustomJsonConverter_ReturnsInstanceWithAllValues()
        {
            var input = "Connor, Sarah (19) Female";
            var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\) (?<Sex>\w.*)";

            var person = input.DeserializeFromRegex<PersonWithSex>(pattern, new SexConverter());

            Assert.NotNull(person);

            Assert.Equal("Sarah", person.FirstName);
            Assert.Equal("Connor", person.LastName);
            Assert.Equal(19, person.Age);
            Assert.Equal(Sex.F, person.Sex);
        }
    }
}
