# DbFlow

DbFlow is a tool with the ambition to help in all stages of developing, testing and releasing databases.

The current version of DbFlow can be used for 

- generate complete scripts for a database schema that can be used for improved version control	
- clone an existing database to a local copy that can be used for unit testing or other

### DbFlow supports

- Microsoft Sql Server

### Known bugs/limitation

- Does **not** consider ansi padding of individual columns
- XML INDEX **is not** yet supported 
- Disabled keys **is not** yet supprted 
- Disabled default constraints **is not** yet supprted 

### Planned features:

- Replace the need for DbUp
- Generate documentation from ms_description
- Support for PostgreSql 

## Examples 

### To clone a database (F#):

```fsharp
open Microsoft.Data.SqlClient
open DbFlow
open DbFlow.SqlServer

let options = Options.Default

let logger _message = ()

let srcConnectionStr = ...
let dstConnectionStr = ...

let dbSchema = 
    use connection = new SqlConnection(srcConnectionStr)
    connection.Open()
    Execute.readSchema logger options connection

use connection = new SqlConnection(dstConnectionStr)
connection.Open()
Execute.clone logger options dbSchema connection 
```

### To generate scripts from a database (F#):

```fsharp
open Microsoft.Data.SqlClient
open DbFlow
open DbFlow.SqlServer

let options = Options.Default

let logger _message = ()

let srcConnectionStr = ...
let dstDirectory = ...

let dbSchema = 
    use connection = new SqlConnection(srcConnectionStr)
    connection.Open()
    Execute.readSchema logger options connection

Execute.generateScriptFiles options dbSchema dstDirectory   
```

### To clone a database (C#):

```csharp
var logger = LoggerModule.fromFunc(s => { });
var options = OptionsModule.Default;

using var sourceConnection = new Microsoft.Data.SqlClient.SqlConnection(sourceConnectionString);
sourceConnection.Open();
var schema = Execute.readSchema(logger, options, sourceConnection);

var cloneDb = Execute.cloneToLocal(logger, options, schema);
```

### To generate scripts from a database (C#):

```csharp
var logger = LoggerModule.fromFunc(s => { });
var options = OptionsModule.Default;

using var sourceConnection = new Microsoft.Data.SqlClient.SqlConnection(sourceConnectionString);
sourceConnection.Open();
var schema = Execute.readSchema(logger, options, sourceConnection);

Execute.generateScriptFiles(options, schema, destinationDirectory);
```