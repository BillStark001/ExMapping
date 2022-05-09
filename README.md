# ExMapping

A simple, general users-facing(especially for those who do not understand JSON format) format designed to represent mappings with string-type keys and values.

## Usage

TBD

For example, 
```
key1=value1
key2=value2
key3!=value4
key4!==value3
```
parses to
```
{
    "key1": "value1", 
    "key2": "value2", 
    "key3!": "value4", 
    "key4!": "=value3"
}
```



### Reserved words

#### `=`
- Used to separate **key** and **value**
- the spaces near the sign is **not** ignored
- Any equal signs after the first one will be ignored
- When `=` is the first character, an empty key is used
    Example:
    ```
    key1=value1
    key2=value2=value3, value4
    key3 = = = value3
    =empty
    ```
    is parsed to
    ```
    {
        "key1": "value1"
        "key2": "value2=value3, value4"
        "key3 ": " = = value3"
        "": "empty"
    }
    ```

#### `#`
- Used to ignore everything on the line as the first character (mark the current line as a comment)
    Example:
    ```
    # this is an example of file format
    # key1=value1
    key2=value2
    ```
    is parsed to
    ```
    {
        "key2": "value2"
    }
    ```

#### `\`
- Used as an escape character in key values
- Supports any reserved characters defined here and `\n`, `\r`, `\t`, `\f`, `\b`, `\xdd` and `\udddd`
- Both `\` and `\\` are parsed as `\`
    Examples:
    `\#key\=value=value=value` => `{ "#key=value": "value=value" }`
    `\#key\\==value` => `{ "#key\\": "=value" }`

#### `$`
- As the first character, concatenates the current line **with** a return character to the previous line with a `=`
#### `&`
- As the first character, concatenates the current line **without** a return character to the previous line with a `=`
- If there is at least one non-space character and no `=` in a line, the line is parsed as `$` concatenated with the line's content
- When there is no pervious key, a key `$"LINE{i}"` is generated where `i` is the current line number
    Example: 
    ```

            
    hello?
    longText=
    &line1
    $=line2
    $$line3
     and its tail
    noKeyText=


    # there are 10 space characters in the next line
              
    what the hell?
    ```
    is parsed to 
    ```
    {
        "#LINE3": "hello?"
        "longText": "line1\n=line2\n$line3\n and its tail"
        "noKeyText": "\nwhat the hell?"
    }
    ```