/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/

using Newtonsoft.Json;

namespace System.Configuration
{
    /// <summary>
    /// Allows <see cref="Enumeration"/> to be converted to string and inversely when used with Json.
    /// </summary>
    public sealed class EnumerationJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
            => objectType.IsEquivalentTo(typeof(Enumeration)) || objectType.IsSubclassOf(typeof(Enumeration));

        /// <summary>
        /// Reads the JSON representation of the <see cref="Enumeration"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string value)
            {
                try
                {
                    return Enumeration.FromDisplayName(objectType, value);
                }
                catch
                {
                    throw new JsonSerializationException($"'{reader.Value}' is not a valid value in '{objectType.Name}'.");
                }
            }

            throw new JsonSerializationException($"'{reader.Value}' is not a valid type value for '{objectType.Name}'.");
        }

        /// <summary>
        /// Writes the JSON representation of the <see cref="Enumeration"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Enumeration enumeration)
                writer.WriteValue(enumeration.DisplayName);
            else
                writer.WriteNull();
        }
    }
}