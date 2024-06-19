using System.Text.Json.Serialization;
using System.Text.Json;

namespace MilkStore_BE
{
    public static class JsonSerializerOptionsProvider
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
    }
}
