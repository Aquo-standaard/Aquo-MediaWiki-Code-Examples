using System;
using System.Collections.Generic;
using System.Linq;
using AquoQueryConsole.Helpers;

namespace AquoQueryConsole.Models
{
	public class DomainTableInfo
	{
		
		public DomainTableInfo(DomainTable table)
		{
			Id               = table.Id;
			Url              = table.Url;
			ValidFrom        = ValidityConvertor.ConvertToDateTimeOffset(table.ValidFrom);
			ValidUntil       = ValidityConvertor.ConvertToDateTimeOffset(table.ValidUntil);
			FieldNames       = table.Properties;
			Name             = table.Label.First();
			ChangeDate       = ValidityConvertor.ConvertToDateTimeOffset(table.ChangeDate);
			IsOpenDomain     = table.IsOpenDomain != "N";
			ResponsibilityOf = table.ResponsibilityOf.FirstOrDefault();
			UrlOfResponsible = table.UrlOfResponsible;
			Description      = table.Description.FirstOrDefault();
		}
		
		public string          Id               { get; } = string.Empty;
		public string          Url              { get; } = string.Empty;
		public DateTimeOffset? ValidFrom        { get; } = DateTimeOffset.MinValue;
		public DateTimeOffset? ValidUntil       { get; } = DateTimeOffset.MinValue;
		public DateTimeOffset? ChangeDate       { get; }
		public List<string>    FieldNames       { get; } = new();
		public string          Name             { get; } = string.Empty;
		public bool            IsOpenDomain     { set; get; }
		public string?         ResponsibilityOf { set; get; }
		public string?         UrlOfResponsible { set; get; }
		public string?         Description      { set; get; }
	}
}