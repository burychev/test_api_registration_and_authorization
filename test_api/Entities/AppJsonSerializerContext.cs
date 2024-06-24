using System.Text.Json.Serialization;

[JsonSerializable(typeof(List<User>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
