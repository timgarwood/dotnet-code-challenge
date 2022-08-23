using Newtonsoft.Json;
using System;

namespace CodeChallenge.JsonConverters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateOnly.ParseExact(reader.Value as string, "yyyy-MM-dd");
        }

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
