# Lookup

This is a command line DNS record lookup tool written in C#.


## Installation

To use this tool, you'll need to compile the C# source code. Make use you have the [.NET SDK](https://dotnet.microsoft.com/en-us/download) installed.

1. Clone this repository to your local machine:

   ```shell
   git clone https://github.com/alitma5094/lookup.git
   cd lookup
   ```

2. Run the tool:

   ```shell
   dotnet run -- example.org
   ```

## Usage

The tool supports a variety of command-line options. Here's how to use it:

```shell
USAGE:
    lookup <Url> [Record Types] [OPTIONS]

ARGUMENTS:
    <url>             The url you want to look up                                         
    [record types]    The record types you want to find (defaults to all if not specified)

OPTIONS:
    -h, --help       Prints help information     
        --rainbow    Make the output text rainbow
```

Example usages:

- Lookup all records for a domain:

  ```shell
  ./lookup example.org
  ```

- Lookup MX records for a domain:

  ```shell
  ./lookup example.org MX
  ```

- Output results in a rainbow of colors:

  ```shell
  ./lookup example.org --rainbow
  ```

## Contributing

If you find any issues or have ideas for improvements, please open an issue or submit a pull request. I welcome all contributions.

## License

This tool is open-source and available under the [MIT License](LICENSE.md).
