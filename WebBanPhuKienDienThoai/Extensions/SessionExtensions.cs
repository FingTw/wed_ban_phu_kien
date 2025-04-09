using System.Text.Json;

namespace WebBanPhuKienDienThoai.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key, T defaultValue = default)
        {
            var value = session.GetString(key);
            if (value == null)
            {
                return defaultValue;
            }
            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (JsonException)
            {
                return defaultValue;
            }
        }
    }
}