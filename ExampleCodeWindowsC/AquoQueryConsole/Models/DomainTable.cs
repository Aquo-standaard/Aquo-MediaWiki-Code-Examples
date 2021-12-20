using System.Collections.Generic;
using Newtonsoft.Json;

namespace AquoQueryConsole.Models
{
	public class DomainTable
	{
		[JsonProperty("id")]                                public string       Id               { set; get; } = string.Empty;
		[JsonProperty("fullurl")]                           public string       Url              { set; get; } = string.Empty;
		[JsonProperty("Begin geldigheid")]                  public Validity[]   ValidFrom        { set; get; } = new Validity[2];
		[JsonProperty("Eind geldigheid")]                   public Validity[]   ValidUntil       { set; get; } = new Validity[2];
		[JsonProperty("Metadata")]                          public List<string> Properties       { set; get; } = new();
		[JsonProperty("Voorkeurslabel")]                    public List<string> Label            { set; get; } = new();
		[JsonProperty("Datum gewijzigd")]                   public Validity[]   ChangeDate       { set; get; } = new Validity[2];
		[JsonProperty("Is open domein")]                    public string       IsOpenDomain     { set; get; } = string.Empty;
		[JsonProperty("Verantwoordelijke organisatie")]     public List<string> ResponsibilityOf { set; get; } = new();
		[JsonProperty("URL Verantwoordelijke organisatie")] public string       UrlOfResponsible { set; get; } = string.Empty;
		[JsonProperty("Toelichting (nl)")]                  public List<string> Description      { set; get; } = new();
	}
}