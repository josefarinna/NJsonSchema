//-----------------------------------------------------------------------
// <copyright file="ClassTemplateModel.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace NJsonSchema.CodeGeneration.Models
{
    /// <summary>The class template base class.</summary>
    public abstract class ClassTemplateModelBase : TemplateModelBase
    {
        private readonly JsonSchema4 _schema;
        private readonly object _rootObject;
        private readonly TypeResolverBase _resolver;

        /// <summary>Initializes a new instance of the <see cref="ClassTemplateModelBase" /> class.</summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="rootObject">The root object.</param>
        protected ClassTemplateModelBase(TypeResolverBase resolver, JsonSchema4 schema, object rootObject)
        {
            _schema = schema;
            _rootObject = rootObject;
            _resolver = resolver;
        }

        /// <summary>Gets the class.</summary>
        public abstract string ClassName { get; }

        /// <summary>Gets or sets a value indicating whether the type is abstract.</summary>
        public bool IsAbstract => _schema.IsAbstract;

        /// <summary>Gets the derived class names (discriminator key/type name).</summary>
        public ICollection<DerivedClassModel> DerivedClasses => _schema
            .GetDerivedSchemas(_rootObject)
            .Select(p => new DerivedClassModel(p.Value, _resolver.GetOrGenerateTypeName(p.Key, p.Value), p.Key.ActualTypeSchema.IsAbstract))
            .ToList();

        /// <summary>The model of a derived class.</summary>
        public class DerivedClassModel
        {
            internal DerivedClassModel(string discriminator, string className, bool isAbstract)
            {
                ClassName = className;
                IsAbstract = isAbstract;
                Discriminator = !string.IsNullOrEmpty(discriminator) ? discriminator : className;
            }

            /// <summary>Gets the discriminator.</summary>
            public string Discriminator { get; }

            /// <summary>Gets the class name.</summary>
            public string ClassName { get; }

            /// <summary>Gets a value indicating whether the class is abstract.</summary>
            public bool IsAbstract { get; }
        }
    }
}