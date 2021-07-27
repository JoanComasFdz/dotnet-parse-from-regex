# dotnet-parse-from-regex
A tool to parse strings via regex with named capture groups into a type, in C# .NET 5.

When you find yourselve having to parse strings, in dotnet you would inmediately go for [string.split](https://docs.microsoft.com/en-us/dotnet/api/system.string.split?view=net-5.0), [stirng.substring](https://docs.microsoft.com/en-us/dotnet/api/system.string.substring?view=net-5.0), or [Spans](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-5.0).

With this tool you can create a regex pattern or tempalte with [named capturing groups](https://www.regular-expressions.info/named.html) and extract the values in a cleaner way.

You move the complexity from the code itself, to being able to obtain a regex pattern that can parse it.

To help with regex, use:
- https://regex101.com/
- https://regexr.com/
## Usage
Two extension methods are available for strings:

1. ParseGroupNamesWithValues: Returns a dictionary with the values.
2. DeserializeFromRegex: Returns an instance of the specified type with the values.
### ParseGroupNamesWithValues

```csharp
var input = "Connor, John (19)";
var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\)";
var values = input.ParseGroupNamesWithValues(pattern);
Console.WriteLine(dict["FirstName"]); // John
Console.WriteLine(dict["LastName"]); // Connor
Console.WriteLine(dict["Age"]); // 19
```

### DeserializeFromRegex
```csharp
public record Person(string FirstName, string LastName, int Age);
var input = "Connor, John (19)";
var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\)";
var values = input.DeserializeFromRegex<Person>(pattern);
Console.WriteLine(instance.FirstName); // John
Console.WriteLine(instance.LastName); // Connor
Console.WriteLine(instance.Age); // 19
```

A second override is available, where a [custom JsonConverter](https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm) can be passed. For example, if a string has "Male" or "Female" and the type has an enum property with values "M" and "F", the previous call will fail because "Male" cannot be converted to "M".

Create your own JsonConverter and pass it as a second argument:

```csharp
public enum Sex { F, M }
public record PersonWithSex(string FirstName, string LastName, int Age, Sex Sex);
public class SexConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Sex);
    }
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
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
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
var input = "Connor, Sarah (19) Female";
var pattern = @"^(?<LastName>\w.*), (?<FirstName>\w.*) \((?<Age>\w.*)\) (?<Sex>\w.*)";
var values = input.DeserializeFromRegex<PersonWithSex>(pattern, new SexConverter());
Console.WriteLine(instance.FirstName); // Sarah
Console.WriteLine(instance.LastName); // Connor
Console.WriteLine(instance.Age); // 19
Console.WriteLine(instance.Sex); // F
```