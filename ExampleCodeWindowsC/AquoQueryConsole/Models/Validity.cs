using Newtonsoft.Json;

namespace AquoQueryConsole.Models
{
	public class Validity
	{
		[JsonProperty("timestamp")] public long TimeStamp { get; set; }
	}
}