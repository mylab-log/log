# MyLab.Logging

For .NET Core 3.1+

[![NuGet](https://img.shields.io/nuget/v/MyLab.Logging.svg)](https://www.nuget.org/packages/MyLab.Logging/)

.NET Core based framework which defines advanced log entity model for built-in .NET Core logging subsystem. 

## The Model

The following block represent the log model:

```c#
/// <summary>
/// Contains log item data
/// </summary>
public class LogEntity
{
    /// <summary>
    /// Occurrence time
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Log message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Labels
    /// </summary>
    public LogLabels Labels { get; }
    
    /// <summary>
    /// Facts
    /// </summary>
    public LogFacts Facts { get; }
}
```

The `Logentity` contains:

* `Time` - event occurrence date and time. Current date and time by default;
* `Message` - log message;
* `Labels` - contains named labels with text value. Implements as string-string dictionary;
* `Facts` - contains named facts with object value. Implements as string-object dictionary.

## Serialization

There is supports two built-in formatters:

* `LogEntityFormatter.Yaml`
* `LogEntityFormatter.Json`

The both formatters represent the same model (set of properties) in corresponded formats. 

`Yaml` example:

```yaml
Message: Test message
Time: 2021-02-22T17:25:58.713
Labels:
  label1: value1
  label2: value2
Facts:
  fact1: fact1
  fact2: fact2
```

`Json` example:

```json
{
  "Message": "Test message",
  "Time": "2021-02-22T17:30:06.615",
  "Labels": {
    "label1": "value1",
    "label2": "value2"
  },
  "Facts": {
    "fact1": "fact1",
    "fact2": "fact2"
  }
}
```

`Yaml` example with exception:

```yaml
Message: Test error message
Time: 2021-02-22T17:25:59.196
Labels:
  label1: value1
  label2: value2
Facts:
  fact1: fact1
  fact2: fact2
  exception:
    Message: Big exception
    Type: System.NotSupportedException
    StackTrace: '   at Demo.Program.CreateException() in C:\..\src\Demo\Program.cs:line 93'
    Inner:
      Message: Inner message
      Type: System.InvalidOperationException
      StackTrace: '   at Demo.Program.CreateException() in C:\..\src\Demo\Program.cs:line 87'

```

`Json` example with exception:

```json
{
  "Message": "Test error message",
  "Time": "2021-02-22T17:30:06.749",
  "Labels": {
    "label1": "value1",
    "label2": "value2"
  },
  "Facts": {
    "fact1": "fact1",
    "fact2": "fact2",
    "exception": {
      "Message": "Big exception",
      "Type": "System.NotSupportedException",
      "StackTrace": "   at Demo.Program.CreateException() in C:\\..\\src\\Demo\\Program.cs:line 93",
      "Inner": {
        "Message": "Inner message",
        "Type": "System.InvalidOperationException",
        "StackTrace": "   at Demo.Program.CreateException() in C:\\..\\src\\Demo\\Program.cs:line 87"
      }
    }
  }
}
```

### Exceptions

An exception may store facts and labels which will be represents in `LogEntity`.

To set exception for `LogEntity` use method `SetException`. It add or replace fact with key `exception` which stores exception info.

The following example shows how use exception to attach log facts and labels:

```C#
Exception exception;
try
{
    // Throw an exception
    throw new InvalidOperationException("Inner message") 			
	    // Add facts (may be sequence of `AndFactIs` calls)
        .AndFactIs("Inner exception fact", "inner fact") 			
        // Add labels (may be sequence of `AndMarkAs` calls)
        .AndMark("error", "true");								
}
catch (Exception e)
{
	exception = e;
}

// Create log netity
var logEntity = new LogEntity										
{
	Message = "Error"
};

// Set caught exception to log entity
logEntity.SetException(exception);									

// Write log
logger.Log(LogLevel.Error, default, logEntity, null, LogEntityFormatter.Yaml); 	
```

Result:

```yaml
Message: Error
Time: 2021-02-22T17:51:21.208
Facts:
  exception:
    Message: Inner message
    Type: System.InvalidOperationException
    StackTrace: '   at Demo.Program.WriteLogWithExceptionStuff(ILogger`1 logger, Func`3 formatter) in C:\...\src\Demo\Program.cs:line 31'
    Labels:
      error: true
    Facts:
      Inner exception fact: inner fact
```

### Developing points

#### Serializers

There is the interface to specify `LogEntity` serializer:

```c#
/// <summary>
/// Describes <see cref="LogEntity"/> serializer
/// </summary>
public interface ILogEntitySerializer
{
    /// <summary>
    /// Serializes a <see cref="LogEntity"/> to <see cref="string"/>
    /// </summary>
    string Serialize(LogEntity logEntity);
}
```

An follow implementations for it:

* `YamlLogEntitySerializer` - converts `LogEntity` to `yaml` string;
* `JsonLogEntitySerializer`- converts `LogEntity` to `json` string.

Use `ILogEntitySerializer` when develop tools for `LogEntity` to customize serialization. 