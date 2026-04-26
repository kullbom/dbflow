module DbFlow.Test.Plans

open DbFlow.SqlServer.Experimental.ShowPlanXml

let samplesFolder = __SOURCE_DIRECTORY__ + "\\"
let output fmt = Printf.ksprintf (System.Console.Out.Write) fmt

[<EntryPoint>]
let main args =
    let sw = System.Diagnostics.Stopwatch ()
    sw.Start ()
    let plan01 = System.IO.File.ReadAllText (samplesFolder + "AWPlan01.xml")
    let timeRead = sw.ElapsedMilliseconds 
    sw.Restart ()
    let doc = System.Xml.Linq.XDocument.Parse plan01
    let timeXmlParse = sw.ElapsedMilliseconds 
    sw.Restart ()
    
    let mutable plans = [] 
    for i = 0 to 1000 do
        let plan = Plan.parseXDocument doc
        plans <- plan :: plans
        ()
    let timePlanParse = sw.ElapsedMilliseconds 
    
    output "Time to read file: %d ms" timeRead
    output "Time to parse XML: %d ms" timeXmlParse
    output "Time to parse plan: %d ms" timePlanParse
    
    0