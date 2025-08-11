# DbFlow

DbFlow is a tool with the ambition to help in all stages of developing, testing and releasing databases.

The current version of DbFlow can be used for 

- generate complete scripts for a database schema that can be used for improved version control	
- clone an existing database to a local copy that can be used for unit testing or other

DbFlow supports

- Microsoft Sql Server
- (PostgreSql is in planning stage)

To clone a database:

```fsharp
open DbFlow.SqlServer

let options = Options.Default

let logger _message = ()

let srcConnectionStr = ...
let dstConnectionStr = ...

let dbSchema = 
    use connection = new SqlConnection(srcConnectionStr)
    connection.Open()
    Schema.DATABASE.read logger options connection

use connection = new SqlConnection(dstConnectionStr)
connection.Open()
Execute.clone logger options dbSchema 
```

To generate scripts from a database:

```fsharp
open DbFlow.SqlServer

let options = Options.Default

let logger _message = ()

let srcConnectionStr = ...
let dstDirectory = ...

let dbSchema = Schema.DATABASE.read logger options connection

Scripts.Execute.generateScriptFiles options dbSchema dstDirectory 
```