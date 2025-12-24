using System.Text.Json;

namespace WealthManager.Shared.Utilities;

public static class SensitiveDataMasker
{
    private static readonly HashSet<string> SensitiveFields = new()
    {
        "password",
        "newPassword",
        "oldPassword",
        "currentPassword",
        "token",
        "accessToken",
        "refreshToken",
        "apiKey",
        "secret",
        "privateKey",
        "creditCard",
        "cvv",
        "ssn",
        "authorization"
    };

    private const string MaskValue = "***MASKED***";

    public static string MaskSensitiveData(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return json;

        try
        {
            using var jsonDoc = JsonDocument.Parse(json);

            var maskedData = MaskJsonElement(jsonDoc.RootElement);

            return JsonSerializer.Serialize(maskedData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch
        {
            return json;
        }
    }

    private static object? MaskJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();

                foreach (var property in element.EnumerateObject())
                {
                    if (SensitiveFields.Contains(property.Name))
                    {
                        dict[property.Name] = MaskValue;
                    }
                    else
                    {
                        dict[property.Name] = MaskJsonElement(property.Value);
                    }
                }
                return dict;
            case JsonValueKind.Array:
                return element.EnumerateArray()
                    .Select(MaskJsonElement)
                    .ToList();
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
                return null;
            default:
                return element.ToString();
        }
    }
}
