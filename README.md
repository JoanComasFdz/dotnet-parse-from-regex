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