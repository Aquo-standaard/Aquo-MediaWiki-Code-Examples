using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AquoQueryConsole.Convertors
{
	public class AquoConverter<T> : JsonConverter where T : class, new()
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.GetTypeInfo()
			                 .IsClass;
		}

		private static JObject? Flattener(JToken? token)
		{
			if (token == null)
			{
				return null;
			}

			var jsonObject          = (JObject) token;
			var convertedJsonObject = new JObject();
			foreach (var (key, value) in jsonObject)
			{
				if (value == null || !value.HasValues)
				{
					continue;
				}

				convertedJsonObject.Add(key, value);
				convertedJsonObject["id"]      = jsonObject.Parent?.Parent?["fulltext"];
				convertedJsonObject["fullurl"] = jsonObject.Parent?.Parent?["fullurl"];
			}

			return convertedJsonObject;
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var jo    = JObject.Load(reader);
			var items = new List<T>();
			if (!jo.ContainsKey("results"))
			{
				return null;
			}

			foreach (var result in (JObject) jo["results"]!)
			{
				var value = result.Value?["printouts"] ?? null; // Printouts contain the properties of the entity.
				if (value == null)
				{
					continue;
				}

				var flattenedToken = Flattener(value);
				if (flattenedToken == null)
				{
					continue;
				}
				
				var record = JsonConvert.DeserializeObject<T>(flattenedToken.ToString());
				items.Add(record);
			}

			return items;
		}
	}

	public class AquoConverter : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.GetTypeInfo()
			                 .IsClass;
		}

		private static JObject? Flattener(JToken? token)
		{
			if (token == null)
			{
				return null;
			}

			var jsonObject          = (JObject) token;
			var convertedJsonObject = new JObject();
			foreach (var (key, value) in jsonObject)
			{
				if (value == null)
				{
					continue;
				}

				convertedJsonObject.Add(key, value);
				convertedJsonObject["id"]      = jsonObject.Parent?.Parent?["fulltext"];
				convertedJsonObject["fullurl"] = jsonObject.Parent?.Parent?["fullurl"];
			}

			return convertedJsonObject;
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var jo    = JObject.Load(reader);
			var items = new List<Dictionary<string, object>>();
			if (!jo.ContainsKey("results"))
			{
				return null;
			}

			foreach (var result in (JObject) jo["results"]!)
			{
				var value = result.Value?["printouts"] ?? null; // Printouts contain the properties of the entity.
				if (value == null)
				{
					continue;
				}

				var flattenedToken = Flattener(value);
				if (flattenedToken == null)
				{
					continue;
				}

				var dict = new Dictionary<string, object>();
				if (flattenedToken.HasValues)
				{
					foreach (var (key, jToken) in flattenedToken)
					{
						if (jToken == null)
						{
							continue;
						}

						switch (key)
						{
							// Dates
							case "Begin geldigheid":
							case "Eind geldigheid":
							case "Datum gewijzigd":
							{
								var timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(jToken.Values()
								                                                                    .First()
								                                                                    .Values()
								                                                                    .First()
								                                                                    .ToString()));
								dict.Add(key, timestamp);
								break;
							}

							// Specials
							case "id":
							{
								var itemValue = jToken.Value<string>();
								dict.Add(key, itemValue);
								break;
							}

							default:
							{
								var itemValue = (jToken.HasValues
									                 ? jToken.Values()
									                         .FirstOrDefault()
									                         ?.ToString()
									                 : string.Empty) ?? string.Empty;
								dict.Add(key, itemValue);
								break;
							}
						}
					}
				}

				items.Add(dict);
			}

			return items;
		}
	}
}