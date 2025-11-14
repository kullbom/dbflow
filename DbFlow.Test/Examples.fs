namespace DbFlow

module ``To clone a database:`` = 
    open Microsoft.Data.SqlClient
    open DbFlow
    open DbFlow.SqlServer
    
    let options = Options.Default
    
    let logger = Logger.dummy
    
    let srcConnectionStr = "<...>"
    let dstConnectionStr = "<...>"
    
    let dbSchema = 
        use connection = new SqlConnection(srcConnectionStr)
        connection.Open()
        Execute.readSchema logger options connection
    
    (use connection = new SqlConnection(dstConnectionStr)
     connection.Open()
     Execute.clone logger options dbSchema connection)


module ``To generate scripts from a database:`` =
    open Microsoft.Data.SqlClient
    open DbFlow
    open DbFlow.SqlServer
    
    let options = Options.Default
    
    let logger = Logger.dummy

    let srcConnectionStr = "<...>"
    let dstDirectory = "<...>"
    
    let dbSchema = 
        use connection = new SqlConnection(srcConnectionStr)
        connection.Open()
        Execute.readSchema logger options connection
    
    Execute.generateScriptFiles options dbSchema dstDirectory   

