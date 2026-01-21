using System.Text.Json;
using System.Text.Json.Serialization;

namespace JinChanChanTool.Tools
{
    /// <summary>
    /// Color类型的JSON转换器，用于System.Text.Json序列化和反序列化Color对象
    /// </summary>
    public class ColorJsonConverter : JsonConverter<Color>
    {
        /// <summary>
        /// 从JSON读取Color对象
        /// </summary>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("期望一个JSON对象来表示Color");
            }

            int r = 0, g = 0, b = 0, a = 255;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return Color.FromArgb(a, r, g, b);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "R":
                            r = reader.GetInt32();
                            break;
                        case "G":
                            g = reader.GetInt32();
                            break;
                        case "B":
                            b = reader.GetInt32();
                            break;
                        case "A":
                            a = reader.GetInt32();
                            break;
                        // 忽略其他只读属性
                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            throw new JsonException("JSON对象未正确结束");
        }

        /// <summary>
        /// 将Color对象写入JSON
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("R", value.R);
            writer.WriteNumber("G", value.G);
            writer.WriteNumber("B", value.B);
            writer.WriteNumber("A", value.A);
            writer.WriteEndObject();
        }
    }
}
