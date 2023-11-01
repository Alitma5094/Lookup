using System.Diagnostics.CodeAnalysis;
using DnsClient;
using DnsClient.Protocol;
using DnsClient.Protocol.Options;
using Spectre.Console;


internal class lookup
{
    public static void Main(string[] args)
    {
        var client = new LookupClient();
        var results = client.Query("google.com", QueryType.ANY).Answers;
        IEnumerable<DnsResourceRecord> query = from i in results
            orderby i.RecordType.ToString()
            select i;
        var table = new Table();
        table.AddColumn("Type").AddColumn("Name").AddColumn("TTL").AddColumn("Data");
        var colors = new List<string> { "#FF5555", "#FFB86C", "#F1FA8C", "#50FA7B", "#BD93F9"};
        var colorIndex = 0;
        foreach (var record in query)
        {
            var temp = new string[4];
            temp[0] = $"[{colors[colorIndex]}]{record.RecordType.ToString()}[/]";
            temp[1] = $"[{colors[colorIndex]}]{record.DomainName}[/]";
            temp[2] = $"[{colors[colorIndex]}]{record.TimeToLive.ToString()}[/]";
            
            if (record.GetType() == typeof(ARecord) | record.GetType() == typeof(AaaaRecord))
            {   
                var tempRecord = record as AddressRecord;
                temp[3] = $"[{colors[colorIndex]}]{tempRecord.Address}[/]";
            }
            else if (record.GetType() == typeof(CNameRecord))
            {
                var tempRecord = record as CNameRecord;
                temp[3] = $"[{colors[colorIndex]}]{tempRecord.CanonicalName}[/]";
            }
            else if (record.GetType() == typeof(MxRecord))
            {
                var tempRecord = record as MxRecord;
                temp[3] = $"[{colors[colorIndex]}]{tempRecord.Exchange.ToString()}[/]";
            }
            else if (record.GetType() == typeof(TxtRecord))
            {
                var tempRecord = record as TxtRecord;
                temp[3] = $"[{colors[colorIndex]}]{tempRecord.EscapedText.FirstOrDefault()}[/]";
            }
            else
            {
                temp[3] = $"[{colors[colorIndex]}]Not Supported[/]";
            }
            table.AddRow(temp[0], temp[1], temp[2], temp[3] );

            var tempColor = colors[0];
            colors.RemoveAt(0);
            colors.Add(tempColor);
        }

        AnsiConsole.Write(table);
    }

}