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

        public record Person(string FirstName, string LastName, int Age);
    }
}
