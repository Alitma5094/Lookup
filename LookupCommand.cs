using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DnsClient;
using DnsClient.Protocol;
using Spectre.Console;
using Spectre.Console.Cli;

namespace lookup;

internal sealed class LookupCommand : Command<LookupCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<url>")]
        [Description(description:"Make the output text rainbow")]
        public required string Url { get; set; }
        
        [CommandArgument(1, "[record types]")]
        [Description(description:"Make the output text rainbow")]
        [DefaultValue("ANY")]
        public string[]? Records { get; set; }
        
        [CommandOption("--rainbow")]
        [Description(description:"Make the output text rainbow")]
        [DefaultValue(false)]
        public bool RainbowColor { get; init; }
    }
    
    public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (settings.Records == null || !settings.Records.Any()) return base.Validate(context, settings);
        foreach (var record in settings.Records)
        {
            if (!Enum.TryParse(record.ToUpper(), out QueryType parsedType))
            {
                return ValidationResult.Error($"Could not execute because the specified record type \"{record.ToUpper()}\" does not or is not supported");
            }
        }
        return base.Validate(context, settings);
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var client = new LookupClient();
        IEnumerable<DnsResourceRecord> results = new List<DnsResourceRecord>();
        if (settings.Records != null && settings.Records.Any())
        {
            foreach (var record in settings.Records)
            {
                Enum.TryParse(record.ToUpper(), out QueryType parsedType);
                results = results.Concat(client.Query(settings.Url, parsedType).Answers);
            }
        }
        else
        {
            results = client.Query("google.com", QueryType.ANY).Answers;
        }
        
        IEnumerable<DnsResourceRecord> query = from i in results
            orderby i.RecordType.ToString()
            select i;
        var table = new Table();
        // table.Title = new TableTitle("Test");
        table.AddColumn("Type").AddColumn("Name").AddColumn("TTL").AddColumn("Data");
        var colors = settings.RainbowColor ? new List<string> { "#FF5555", "#FFB86C", "#F1FA8C", "#50FA7B", "#BD93F9"} : new List<string> { "white", "grey"};
        foreach (var record in query)
        {
            var temp = new string[4];
            temp[0] = $"[{colors[0]}]{record.RecordType.ToString()}[/]";
            temp[1] = $"[{colors[0]}]{record.DomainName}[/]";
            temp[2] = $"[{colors[0]}]{record.TimeToLive.ToString()}[/]";
             
            if (record.GetType() == typeof(ARecord) | record.GetType() == typeof(AaaaRecord))
            {   
                var tempRecord = record as AddressRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.Address}[/]";
            }
            else if (record.GetType() == typeof(CNameRecord))
            {
                var tempRecord = record as CNameRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.CanonicalName}[/]";
            }
            else if (record.GetType() == typeof(MxRecord))
            {
                var tempRecord = record as MxRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.Exchange}[/]";
            }
            else if (record.GetType() == typeof(TxtRecord))
            {
                var tempRecord = record as TxtRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.EscapedText.FirstOrDefault()}[/]";
            }
            else if (record.GetType() == typeof(CaaRecord))
            {
                var tempRecord = record as CaaRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.Value}[/]";
            }
            else if (record.GetType() == typeof(NsRecord))
            {
                var tempRecord = record as NsRecord;
                temp[3] = $"[{colors[0]}]{tempRecord?.NSDName}[/]";
            }
            else
            {
                temp[3] = $"[{colors[0]}]Not Supported[/]";
            }
            table.AddRow(temp[0], temp[1], temp[2], temp[3] );
        
            var tempColor = colors[0];
            colors.RemoveAt(0);
            colors.Add(tempColor);
        }
        AnsiConsole.Write(table);
        return 0;
    }
}