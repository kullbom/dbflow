namespace DbFlow.SqlServer.Schema

open DbFlow
open DbFlow.Readers

type SequenceDefinition = {
    StartValue : obj option //	0	98	sql_variant
    Increment : obj //	0	98	sql_variant
    MinimumValue : obj option  //	0	98	sql_variant
    MaximumValue : obj option //	0	98	sql_variant

    CurrentValue : obj //	0	98	sql_variant
}

type Sequence = {
    Object : OBJECT
    
    SequenceDefinition : SequenceDefinition
    IsCycling : bool //	1	104	bit
    IsCached : bool  //	1	104	bit
    CacheSize : int option //	1	56	int
    
    Datatype : Datatype

    IsExhausted : bool //	0	104	bit
    // Applies to SQL Server 2017 and later.
    //last_used_value : string option //	1	98	sql_variant
} 

module Sequence =
    let readAll objects types connection =
        DbTr.reader 
            "SELECT 
                s.object_id,
                s.start_value, 
                CASE 
                    WHEN s.start_value = 
                        CASE 
                            WHEN t.name = 'tinyint' THEN CAST(0 AS NUMERIC(38,0))
                            WHEN t.name = 'smallint' THEN CAST(-32768 AS NUMERIC(38,0))
                            WHEN t.name = 'int' THEN CAST(-2147483648 AS NUMERIC(38,0))
                            WHEN t.name = 'bigint' THEN CAST(-9223372036854775808 AS NUMERIC(38,0))
                            WHEN t.name IN ('decimal', 'numeric') THEN -POWER(CAST(10 AS NUMERIC(38,0)), s.precision - s.scale) + 1
                            ELSE NULL
                        END THEN CAST(0 AS BIT)
                    ELSE CAST(1 AS BIT)
                END AS start_value_explicit,
                
                s.increment, 
                s.minimum_value, s.maximum_value,
                CASE 
                    WHEN s.minimum_value = 
                        CASE 
                            WHEN t.name = 'tinyint' THEN CAST(0 AS NUMERIC(38,0))
                            WHEN t.name = 'smallint' THEN CAST(-32768 AS NUMERIC(38,0))
                            WHEN t.name = 'int' THEN CAST(-2147483648 AS NUMERIC(38,0))
                            WHEN t.name = 'bigint' THEN CAST(-9223372036854775808 AS NUMERIC(38,0))
                            WHEN t.name IN ('decimal', 'numeric') THEN -POWER(CAST(10 AS NUMERIC(38,0)), s.precision - s.scale) + 1
                            ELSE NULL
                        END THEN CAST(0 AS BIT)
                    ELSE CAST(1 AS BIT)
                END AS minimum_value_explicit,
                CASE 
                    WHEN s.maximum_value = 
                        CASE 
                            WHEN t.name = 'tinyint' THEN CAST(255 AS NUMERIC(38,0))
                            WHEN t.name = 'smallint' THEN CAST(32767 AS NUMERIC(38,0))
                            WHEN t.name = 'int' THEN CAST(2147483647 AS NUMERIC(38,0))
                            WHEN t.name = 'bigint' THEN CAST(9223372036854775807 AS NUMERIC(38,0))
                            WHEN t.name IN ('decimal', 'numeric') THEN POWER(CAST(10 AS NUMERIC(38,0)), s.precision - s.scale) - 1
                            ELSE NULL
                        END THEN CAST(0 AS BIT)
                    ELSE CAST(1 AS BIT)
                END AS maximum_value_explicit,
                s.is_cycling, s.is_cached, s.cache_size,
                s.system_type_id, s.user_type_id, s.precision, s.scale, 
                -- to follow 'the regular' type pattern...
                CAST(0 AS SMALLINT) AS max_length, CAST(0 AS BIT) AS is_nullable, 
                s.current_value, s.is_exhausted
                --, s.last_used_value
             FROM sys.sequences s
             JOIN sys.types t ON s.user_type_id = t.user_type_id" 
            []
            (fun m r ->
                let object_id = readInt32 "object_id" r
                Map.add object_id 
                    { 
                        Object = RCMap.pick object_id objects 
                    
                        SequenceDefinition = {
                            StartValue = if readBool "start_value_explicit" r then readObject "start_value" r |> Some else None
                            Increment = readObject "increment" r
                            MinimumValue = if readBool "minimum_value_explicit" r then readObject "minimum_value" r |> Some else None
                            MaximumValue = if readBool "maximum_value_explicit" r then readObject "maximum_value" r |> Some else None
                        
                            CurrentValue = readObject "current_value" r
                        }

                        IsCycling = readBool "is_cycling" r
                        IsCached = readBool "is_cached" r
                        CacheSize = nullable "cache_size" readInt32 r
                        
                        Datatype = Datatype.readType types None r
                        
                        IsExhausted = readBool "is_exhausted" r
                        //last_used_value = nullable "last_used_value" readString r
                    } 
                    m)
            Map.empty
        |> DbTr.commit_ connection
        |> RCMap.ofMap