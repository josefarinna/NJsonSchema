﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerYamlDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema.Infrastructure;
using YamlDotNet.Serialization;

namespace NJsonSchema.Yaml
{
    /// <summary>Extension methods to load and save <see cref="JsonSchema4"/> from/to YAML.</summary>
    public static class JsonSchemaYaml
    {
        /// <summary>Creates a JSON Schema from a YAML string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <returns>The <see cref="JsonSchema4"/>.</returns>
        public static async Task<JsonSchema4> FromYamlAsync(string data, string documentPath = null)
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new StringReader(data));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            return await JsonSchema4.FromJsonAsync(json, documentPath).ConfigureAwait(false);
        }

        /// <summary>Converts the JSON Schema to YAML.</summary>
        /// <returns>The YAML string.</returns>
        public static string ToYaml(this JsonSchema4 document)
        {
            var json = document.ToJson();
            var expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);

            var serializer = new Serializer();
            return serializer.Serialize(deserializedObject);
        }

        /// <summary>Creates a JSON Schema from a JSON file.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="JsonSchema4" />.</returns>
        public static async Task<JsonSchema4> FromFileAsync(string filePath)
        {
            var data = await DynamicApis.FileReadAllTextAsync(filePath).ConfigureAwait(false);
            return await FromYamlAsync(data, filePath).ConfigureAwait(false);
        }

        /// <summary>Creates a JSON Schema from an URL.</summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="JsonSchema4"/>.</returns>
        public static async Task<JsonSchema4> FromUrlAsync(string url)
        {
            var data = await DynamicApis.HttpGetAsync(url).ConfigureAwait(false);
            return await FromYamlAsync(data, url).ConfigureAwait(false);
        }
    }
}
