# types

number-type ->
    | 'int8'
    | 'int16'
    | 'int32'
    | 'int64'

    | 'float'
    | 'double'

type -> 
    | 'auto'
    | identifier
    | 'array' 'of' type
    | 'int8'
    | 'int16'
    | 'int32'
    | 'int64'

    | 'float'
    | 'double'

    | 'string'