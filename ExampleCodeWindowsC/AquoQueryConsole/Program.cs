using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquoQueryConsole.Convertors;
using AquoQueryConsole.Helpers;
using AquoQueryConsole.Models;
using CommandLine;
using CommandLine.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace AquoQueryConsole
{
	internal static class Program
	{
		private const int Limit = 500;
		private static readonly string[] GenericDomainTableFields = new[]
		{
			"Eind geldigheid", 
			"Begin geldigheid", 
			"Datum gewijzigd", 
			"Verantwoordelijke organisatie", 
			"URL Verantwoordelijke organisatie", 
			"Toelichting (nl)", 
			"Voorkeurslabel", 
			"Metadata"
		};
		
		private static async Task Main(string[] args)
		{
			var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
			parseResult.WithNotParsed(_ => ThrowOnParseError(parseResult));
			await parseResult.WithParsedAsync(ProcessParametersAsync)
			                 .ConfigureAwait(false);
		}

		private static async Task ProcessParametersAsync(CommandLineOptions arg)
		{
			DoAdditionalChecks(arg);

			var tablesInfo = await GetDomainTablesAsync(arg.BaseUrl!)
				                 .ConfigureAwait(false);
			if (tablesInfo == null)
			{
				Console.WriteLine("Error retrieving domain tables.");
				Environment.Exit(0);
			}

			if (arg.TableNamesOnly)
			{
				await PrintTablesInfoAsync(tablesInfo);
				Environment.Exit(0);
			}

			if (!tablesInfo.ContainsKey(arg.Name!))
			{
				Console.WriteLine($"Unknown or non-current table with name: {arg.Name}.");
				Environment.Exit(0);
			}

			var tableInfo = tablesInfo[arg.Name];
			if (tableInfo?.FieldNames == null || !tableInfo.FieldNames.Any())
			{
				Console.WriteLine($"Insufficient information retrieved for table {arg.Name}.");
				Environment.Exit(0);
			}

			var tableData = await GetTableDataAsync(arg.BaseUrl, tablesInfo[arg.Name])
				                .ConfigureAwait(false);
			if (tableData == null || tableData.Count == 0)
			{
				Console.WriteLine("Error retrieving table data.");
				Environment.Exit(0);
			}

			await PrintTableDataAsync(tableData, tablesInfo[arg.Name])
				.ConfigureAwait(false);
		}

		private static async Task PrintTableDataAsync(IEnumerable<Dictionary<string, object>> tableData, DomainTableInfo info)
		{
			var config    = new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = (_, _) => true };
			var writer    = new StringWriter();
			var csvWriter = new CsvWriter(writer, config);
			if (!info.FieldNames.Any())
			{
				Console.WriteLine("No fields available.");
				Environment.Exit(0);
			}

			var fields = info.FieldNames.Union(new [] {"id"}).ToList();
			var header = string.Join(",", fields.Select(name => $"\"{name}\"")); // Generate the CSV Header.
			Console.WriteLine(header);
			foreach (var data in tableData)
			{
				foreach (var field in fields)
				{
					csvWriter.WriteField(data[field] != null
						                     ? data[field]
							                     .ToString()
						                     : string.Empty); // Write each field.
				}

				await csvWriter.NextRecordAsync()
				               .ConfigureAwait(false);
			}

			Console.Write(writer.ToString());
		}

		private static async Task<List<Dictionary<string, object>>?> GetTableDataAsync(string baseUrl, DomainTableInfo tablesInfo)
		{
			var client     = new RestClient();
			var offset     = 0L;
			var list       = new List<Dictionary<string, object>>();
			do
			{
				var uri = AquoUrlBuilder.BuildUrl(baseUrl, tablesInfo.Id!, tablesInfo.FieldNames)
				                        .Replace("<<limit>>",  Limit.ToString())
				                        .Replace("<<offset>>", offset.ToString());
				if (Debugger.IsAttached)
				{
					Debug.WriteLine(uri);
				}

				var request = new RestRequest(uri, Method.GET, DataFormat.Json);
				var response = await client.ExecuteAsync(request)
				                           .ConfigureAwait(false);
				var domainValues = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Content, new AquoConverter());
				if (domainValues == null)
				{
					break; // No more data.
				}

				list.AddRange(domainValues);
				if (domainValues.Count < Limit)
				{
					break; // No next page.
				}

				offset += Limit;
			} while (true);

			return list;
		}

		private static async Task PrintTablesInfoAsync(Dictionary<string, DomainTableInfo>? tablesInfo)
		{
			if (tablesInfo?.Count == 0 || tablesInfo?.Values == null || tablesInfo.Values.Any() == false)
			{
				return;
			}

			var config = new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = (_, _) => true };
			var writer = new StringWriter();
			await new CsvWriter(writer, config).WriteRecordsAsync(tablesInfo!.Values)
			                                   .ConfigureAwait(false);
			Console.Write(writer.ToString());
		}

		private static void DoAdditionalChecks(CommandLineOptions arg)
		{
			if (!arg.Table || !string.IsNullOrEmpty(arg.Name))
			{
				return;
			}

			Console.WriteLine("--name is required when --table is specified.");
			Environment.Exit(0);
		}

		private static void ThrowOnParseError<T>(ParserResult<T> result)
		{
			var builder       = SentenceBuilder.Create();
			var errorMessages = HelpText.RenderParsingErrorsTextAsLines(result, builder.FormatError, builder.FormatMutuallyExclusiveSetErrors, 1);
			var excList = errorMessages.Select(msg => new ArgumentException(msg))
			                           .ToList();
			if (excList.Any())
			{
				Environment.Exit(0);
			}
		}

		private static async Task<Dictionary<string, DomainTableInfo>?> GetDomainTablesAsync(string baseUrl)
		{
			var client = new RestClient();
			var offset = 0L;
			var list   = new List<DomainTableInfo>();

			do
			{
				var uri = AquoUrlBuilder.BuildUrl(baseUrl, GenericDomainTableFields)
				                        .Replace("<<limit>>",  Limit.ToString())
				                        .Replace("<<offset>>", offset.ToString());
				var request = new RestRequest(uri, Method.GET, DataFormat.Json);
				var response = await client.ExecuteAsync(request)
				                           .ConfigureAwait(false);
				var domainTables = JsonConvert.DeserializeObject<List<DomainTable>>(response.Content, new AquoConverter<DomainTable>());
				if (domainTables == null)
				{
					break; // No more data.
				}

				list.AddRange(domainTables.Select(tableInfo => new DomainTableInfo(tableInfo)));
				if (domainTables.Count < Limit)
				{
					break; // No next page.
				}

				offset += Limit;
			} while (true);

			return list.ToDictionary(table => table.Name!, table => table);
		}
	}
}