using System;
using System.Collections.Generic;

namespace AquoQueryConsole.Helpers
{
	public static class AquoUrlBuilder
	{
		public static string BuildUrl(string baseUrl, IEnumerable<string> fields)
		{
			return $"{baseUrl}/index.php?title=Speciaal:Vragen&x=[[Categorie:Domeintabellen]][[Begin geldigheid::<{DateTime.Now:yyyy-MM-dd}]][[Eind geldigheid::>{DateTime.Now:yyyy-MM-dd}]]/?{string.Join("|?", fields)}&limit=<<limit>>&offset=<<offset>>&format=json&unescape=true";
		}

		public static string BuildUrl(string baseUrl, string tableId, IEnumerable<string> fields)
		{
			return $@"{baseUrl}/index.php?title=Speciaal:Vragen&q=[[Categorie:Domeinwaarden]]+[[Breder::{tableId}]][[Begin geldigheid::<{DateTime.Now:yyyy-MM-dd}]][[Eind geldigheid::>{DateTime.Now:yyyy-MM-dd}]]&po={string.Join("|?", fields)}&p[limit]=<<limit>>&p[offset]=<<offset>>&p[format]=json&p[unescape]=true&sort_num=&order_num=ASC&p[source]=&p[limit]=<<limit>>&p[offset]=<<offset>>&p[link]=none&p[sort]=&p[headers]=show";
		}
	}
}