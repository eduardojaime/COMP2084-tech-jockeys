using Newtonsoft.Json;

namespace TechJockeys.Extensions
{
    // Static class to hold extension methods for ISession
    // It must be static because extension methods can only be defined in static classes.
    public static class SessionExtensions
    {
        /// <summary>
        /// Sets an object in the session by serializing it to a JSON string.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Gets an object from the session by deserializing the JSON string back to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
