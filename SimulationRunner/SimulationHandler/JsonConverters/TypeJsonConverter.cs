using System.Text.Json.Serialization;
using System.Text.Json;
using SimulationStandard;

namespace SimulationHandler.JsonConverters;

// TODO Find better solution. This can be unsafe.
internal class TypeJsonConverter : JsonConverter<Type>
{
    public override Type Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => TypesHelper.AllowedTypes.FirstOrDefault(type => type.ToString() == typeToConvert.ToString()) ?? throw new Exception("Disallowed type");

    public override void Write(
        Utf8JsonWriter writer,
        Type typeValue,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(typeValue.ToString());
}