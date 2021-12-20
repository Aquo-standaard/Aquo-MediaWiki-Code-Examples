using System;
using System.Linq;
using AquoQueryConsole.Models;

namespace AquoQueryConsole.Helpers
{
	public static class ValidityConvertor
	{
		public static DateTimeOffset? ConvertToDateTimeOffset(Validity[] validity)
		{
			if (!validity.Any() || validity[0] == null)
			{
				return null;
			}
			
			return DateTimeOffset.FromUnixTimeSeconds(validity[0].TimeStamp);
		}
	}
}