using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ParseFromRegex
{
    /// <summary>
    /// Provides methods to parse strings, providing a pattern or template in regex with group names.
    /// </summary>
    public static class SerializeUsingRegexExtensions
    {
        /// <summary>
        ///<para>
        /// Parses the string in the <see cref="input"/> parameter using the specified <see cref="regexPattern"/> with capturing group
        /// names and returns a new instance of the <see cref="T"/> type, assigning the values from the named groups to the public properties of
        /// the type.
        ///</para>
        /// <example>For example:
        /// <para><c>public record Person(string FirstName, string LastName, int Age);</c></para>
        /// <para><c>var input = "Comas, Joan (29)";</c></para>
        /// <para><c>var pattern = "^(?&lt;LastName&gt;\w.*), (?&lt;FirstName&gt;\w.*) \((&lt;Age&gt;\w.*)\)</c></para>
        /// <para><c>var instance = input.DeserializeFromRegex&lt;Person&gt;(pattern);</c></para>
        /// <para><c>Console.WriteLine(instance.FirstName); // Joan</c></para>
        /// <para><c>Console.WriteLine(instance.LastName); // Comas</c></para>
        /// <para><c>Console.WriteLine(instance.Age); // 29</c></para>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The type to be returns with all the values extracted from the <see cref="input"/> parameter.</typeparam>
        /// <param name="input">The string to be parsed.</param>
        /// <param name="regexPattern">The pattern/template to extract values from the string to be parsed. The pattern must have group names.</param>
        /// <returns>A new instance of the <see cref="T"/> type with the parsed values from the <see cref="input"/> parameter.</returns>
        public static T DeserializeFromRegex<T>(this string input, string regexPattern)
        {
            var properties = input.ParseGroupNamesWithValues(regexPattern);
            var json = JsonConvert.SerializeObject(properties);
            var instance = JsonConvert.DeserializeObject<T>(json);
            return instance;
        }

        ///  <summary>
        /// <para>
        ///  Parses the string in the <see cref="input"/> parameter using the specified <see cref="regexPattern"/> with capturing group
        ///  names and the specified <see cref="JsonConverter"/> to further enhance parsing. Then returns a new instance of the <see cref="T"/> type,
        ///  assigning the values from the named groups to the public properties of the type.
        /// </para>
        ///  <example>For example:
        ///  <para><c>public enum Sex { M, F };</c></para>
        ///  <para><c>public record Person(string FirstName, string LastName, int Age, Sex Sex);</c></para>
        ///  <para><c>var input = "Comas, Sarah (29) Female";</c></para>
        ///  <para><c>var pattern = "^(?&lt;LastName&gt;\w.*), (?&lt;FirstName&gt;\w.*) \((&lt;Age&gt;\w.*)\) (?&lt;Sex&gt;\w.*)"</c></para>
        ///  <para><c>var instance = input.DeserializeFromRegex&lt;Person&gt;(pattern, new SexJsonConverter());</c></para>
        ///  <para><c>Console.WriteLine(instance.FirstName); // Sarah</c></para>
        ///  <para><c>Console.WriteLine(instance.LastName); // Comas</c></para>
        ///  <para><c>Console.WriteLine(instance.Age); // 29</c></para>
        ///  <para><c>Console.WriteLine(instance.Sex); // F</c></para>
        ///  </example>
        /// <para>
        ///  How to do a custom JsonConverter: https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm
        /// </para>
        ///  </summary>
        ///  <typeparam name="T">The type to be returns with all the values extracted from the <see cref="input"/> parameter.</typeparam>
        ///  <param name="input">The string to be parsed.</param>
        ///  <param name="regexPattern">The pattern/template to extract values from the string to be parsed. The pattern must have group names.</param>
        ///  <param name="jsonConverter">A class inheriting from JsonConverter with the necessary transformations to parse text into particular types like enums.</param>
        ///  <returns>A new instance of the <see cref="T"/> type with the parsed values from the <see cref="input"/> parameter.</returns>
        public static T DeserializeFromRegex<T>(this string input, string regexPattern, JsonConverter jsonConverter)
        {
            var properties = input.ParseGroupNamesWithValues(regexPattern);
            var json = JsonConvert.SerializeObject(properties);

            var instance = JsonConvert.DeserializeObject<T>(json, jsonConverter);
            return instance;
        }

        /// <summary>
        /// <para>
        /// Parses the string in the <see cref="input"/> parameter using the specified <see cref="Regex"/> parameter with capturing group names,
        /// returning a dictionary of the group names and their values.
        /// </para>
        /// <example>For example:
        /// <para><c>var input = "Comas, Joan (29)";</c></para>
        /// <para><c>var pattern = "^(?&lt;LastName&gt;\w.*), (?&lt;FirstName&gt;\w.*) \((&lt;Age&gt;\w.*)\)</c></para>
        /// <para><c>var dict = input.ParseGroupNamesWithValues(pattern);</c></para>
        /// <para><c>Console.WriteLine(dict["FirstName"]); // Joan</c></para>
        /// <para><c>Console.WriteLine(dict["LastName"]); // Comas</c></para>
        /// <para><c>Console.WriteLine(dict["Age"]); // 29</c></para>
        /// </example>
        /// </summary>
        /// <param name="input">The string to be parsed.</param>
        /// <param name="regexPattern">The pattern/template to extract values from the string to be parsed. The pattern must have group names.</param>
        /// <returns>A new instance of the <see cref="T"/> type with the parsed values from the <see cref="input"/> parameter.</returns>
        /// <returns></returns>
        public static Dictionary<string, string> ParseGroupNamesWithValues(this string input, string regexPattern)
        {
            var regex = new Regex(regexPattern);
            // Source: https://stackoverflow.com/questions/1381097/how-do-i-get-the-name-of-captured-groups-in-a-c-sharp-regex
            var groups = regex.Match(input).Groups;
            var groupNames = regex.GetGroupNames();
            return groupNames
                .Skip(1) // First is always "0" with the whole text.
                .Where(groupName => groups[groupName].Captures.Count > 0)
                .ToDictionary(groupName => groupName, groupName => groups[groupName].Value);
        }
    }
}