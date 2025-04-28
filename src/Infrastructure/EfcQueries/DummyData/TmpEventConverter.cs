using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EfcDataAccess.Context;

public class TmpEventConverter : JsonConverter<EventSeedFactory.TmpEvent>
{
    public override EventSeedFactory.TmpEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Create a dictionary to hold the property values
        var properties = new Dictionary<string, object>();

        // Read the JSON object
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            // Read the property name
            string propertyName = reader.GetString();
            reader.Read(); // Move to the value

            // Read the value based on the property name
            switch (propertyName)
            {
                case "Id":
                case "Title":
                case "Description":
                    properties[propertyName] = reader.GetString();
                    break;
                case "Status":
                    // Convert string status to int
                    string statusString = reader.GetString();
                    properties[propertyName] = statusString switch
                    {
                        "cancelled" => 4,
                        "draft" => 3,
                        "ready" => 2,
                        "active" => 1,
                        "inactive" => 0,
                        _ => throw new JsonException($"Unknown status: {statusString}")
                    };
                    break;
                case "Visibility":
                    // Convert string visibility to int
                    string visibilityString = reader.GetString();
                    properties[propertyName] = visibilityString switch
                    {
                        "public" => 1,
                        "private" => 0,
                        _ => throw new JsonException($"Unknown visibility: {visibilityString}")
                    };
                    break;
                case "Start": // Change to match JSON
                    properties["StartTime"] = reader.GetString(); // Map to StartTime
                    break;
                case "End": // Change to match JSON
                    properties["EndTime"] = reader.GetString(); // Map to EndTime
                    break;
                case "MaxGuests":
                    properties[propertyName] = reader.GetInt32();
                    break;
                default:
                    reader.Skip(); // Skip unknown properties
                    break;
            }
        }

        // Create and return the TmpEvent object
        return new EventSeedFactory.TmpEvent(
            (string)properties["Id"],
            (string)properties["Title"],
            (string)properties["Description"],
            (int)properties["Status"],
            (int)properties["Visibility"],
            (string)properties["StartTime"], // Now correctly mapped
            (string)properties["EndTime"], // Now correctly mapped
            (int)properties["MaxGuests"]
        );
    }

    public override void Write(Utf8JsonWriter writer, EventSeedFactory.TmpEvent value, JsonSerializerOptions options)
    {
        // Implement if you need to serialize TmpEvent back to JSON
        throw new NotImplementedException();
    }
}
