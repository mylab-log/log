# MyLab.Log

For .NET Core 3.1+

[![NuGet](https://img.shields.io/nuget/v/MyLab.Log.svg)](https://www.nuget.org/packages/MyLab.Log/)

.NET Core based framework which defines advanced log entity model for built-in .NET Core logging subsystem. 

## The Model

### LogEntity

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
    /// <summary>
    /// Contains exception info
    /// </summary>
    public ExceptionDto Exception { get; set; }
}
```

The `Logentity` contains:

* `Time` - event occurrence date and time. Current date and time by default;
* `Message` - log message;
* `Labels` - contains named labels with text value. Implements as string-string dictionary;
* `Facts` - contains named facts with object value. Implements as string-object dictionary;
* `Exception` - contains info about exception which is reason of event. 

### ExceptionDto

```c#
/// <summary>
/// Exception log model
/// </summary>
public class ExceptionDto
{
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Contains exception hash
    /// </summary>
    public string ExceptionTrace { get; set; }
    /// <summary>
    /// Stack trace
    /// </summary>
    public string StackTrace { get; set; }
    /// <summary>
    /// .NET type
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Array of aggregated exceptions when origin exception is <see cref="AggregateException"/>
    /// </summary>
    public ExceptionDto[] Aggregated { get; set; }
    /// <summary>
    /// Inner exception
    /// </summary>
    public ExceptionDto Inner { get; set; }
    /// <summary>
    /// Exception facts
    /// </summary>
    public LogFacts Facts{ get; set; }
    /// <summary>
    /// Exception labels
    /// </summary>
    public LogLabels Labels{ get; set; }
}
```

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
Exception:
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
    "fact2": "fact2"
  },
  "Exception": {
    "Message": "Big exception",
    "ExceptionTrace": "cf60b784c483dd053f56c29afb02eb33",
    "Type": "System.NotSupportedException",
    "StackTrace": "   at Demo.Program.CreateException() in C:\\..\\src\\Demo\\Program.cs:line 130",
    "Inner": {
      "Message": "Inner message",
      "Type": "System.InvalidOperationException",
      "StackTrace": "   at Demo.Program.CreateException() in C:\\..\\src\\Demo\\Program.cs:line 120",
      "Labels": {
        "unsuppoted": "true"
      },
      "Facts": {
        "Inner exception fact": "inner fact"
      }
    }
  }
}
```

## Exception

### Exception facts and lables

An exception may store facts and labels which will be represents in `LogEntity`. To assign log facts and labels to exception object please use the extension methods `AndFactIs` and `AndLabel`.  

The following example shows how use exception to attach log facts and labels:

```C#
Exception exception;
try
{
    // Create an exception
    var ex = new InvalidOperationException("Inner message")
        .AndFactIs("Inner exception fact", "inner fact")
        .AndLabel("error", "true");								
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
logEntity.Exception = exception;									

// Write log
logger.Log(LogLevel.Error, default, logEntity, null, LogEntityFormatter.Yaml); 	
```

Result:

```yaml
Message: Error
Time: 2021-02-24T01:28:54.748
Exception:
  Message: Inner message
  ExeptionTrace: cf60b784c483dd053f56c29afb02eb33
  Type: System.InvalidOperationException
  StackTrace: '   at Demo.Program.WriteLogWithExceptionStuff(ILogger`1 logger, Func`3 formatter) in C:\..\src\Demo\Program.cs:line 37'
  Labels:
    error: true
  Facts:
    Inner exception fact: inner fact
```

### Exception Trace

If `dto` created based on exception object then `ExceptionTrace` property value calculated automatically based on message, type and stack trace (without line numbers) this exception with exception traces of aggregated and inner exceptions.

## `MyLabConsoleFormatter`

### Formatter overview

The `MyLabConsoleFormatter` integrates in standard .net log infrastructure and extends console logger format collection. The key of the formatter is `mylab`.

It interprets all logs as `MyLab` `LogEntiy` and applies special `yaml` formatter to them.  

```C#
var logger = loggerFactory.CreateLogger("foo");

logger.LogInformation("baz");
```

Log output:

```yaml
Message: baz
Time: 2021-11-17T15:58:09.807
Facts:
  log-category: foo
```

Use extension methods for `ILggingBuilder` to integrate formatter:

```c#
var sp = new ServiceCollection()
                .AddLogging(l => l
					.AddMyLabConsole()  // Adds console logger with mylab formatter
                	)
                .BuildServiceProvider();
```

### Tracing

The `MyLabConsoleFormatter` adds `trace-id` fact (current trace id) from scope independent of `IncludeScopes ` options.

### Scoped facts

The `MyLabConsoleFormatter` applies scoped facts from `FactLogScope` log scope:

```C#
var scopeFacts = new Dictionary<string, object>
{
    { "bar", "baz" }
};
var factScope = new FactLogScope(scopeFacts);

//Act
using (logger.BeginScope(factScope))
{
    logger.LogInformation("qoz");
}
```

Output log message:

```yaml
Message: qoz
Time: 2023-01-12T17:11:33.363
Facts:
  bar: baz
```

### `IncludeScopes`

When `ConsoleFormatterOptions.IncludeScopes` is set to `true` then `MyLabConsoleFormatter` adds all scopes into one fact named `log-scopes`:

```yaml
Message: qoz
Time: 2023-01-13T20:12:05.796
Facts:
  log-scopes:
    TestLogScopes1:
      foo: bar
    TestLogScopes2:
      baz: qoz
```

### `FactLogScope`

The `FactLogScope` may be useful to passing log facts with scope into the log. It used by `MyLabConsoleFormatter` to get context facts independent of `ConsoleFormatterOptions.IncludeScopes` option.

```C#
var scopeFacts = new Dictionary<string, object>
{
    { "bar", "baz" }
};
var factScope = new FactLogScope(scopeFacts);

using (logger.BeginScope(factScope))
{
    logger.LogInformation("qoz");
}
```

```yaml
Message: qoz
Time: 2023-01-13T20:12:05.812
Facts:
  bar: baz
```

### `LabelLogScope`

The `LabelLogScope` may be useful to passing log labels with scope into the log. It used by `MyLabConsoleFormatter` to get context labels independent of `ConsoleFormatterOptions.IncludeScopes` option.

```C#
var scopeFacts = new Dictionary<string, string>
{
    { "bar", "baz" }
};
var labelScope = new LabelLogScope(scopeFacts);

using (logger.BeginScope(labelScope))
{
    logger.LogInformation("qoz");
}
```

```yaml
Message: qoz
Time: 2023-01-13T20:12:05.812
Labels:
  bar: baz
```

## Developing points

### Serializers

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

### Exception Yaml

`ExceptionDto` yaml schema:

```yaml
Exception:
  type: object
  description: Exception
  properties:
    Message:
      type: string
      description: A message that describes the current exception
      example: 'The log table has overflowed'
    ExceptionTrace:
      type: string
      description: 'Contains exception hash'
      example: '3bbc061395e43cf367b3118093b56963'
    StackTrace:
      type: string
      description: A string representation of the immediate frames on the call stack
      example: >
        at NDP_UE_CS.LogTable.AddRecord(String newRecord)
        at NDP_UE_CS.OverflowDemo.Main()
    Type:
      type: string
      description: '.NET class full name'
      example: 'System.InvalidOperationException'
    Aggregated:
      type: array
      description: A collection of the Exception instances that caused the current exception
      items:
        $ref: '#/components/schemas/Exception'
    Inner:
      $ref: '#/components/schemas/Exception'
    Facts:
      type: object
      description: Contains log named facts with object values
      additionalProperties: true
      example: 
        TargetDesc:
          Id: 123
          Size: big
    Labels:
      type: object
      description: Contains log named labels with string values
      example:
        UserId: "123"
      additionalProperties:
        type: string
```

