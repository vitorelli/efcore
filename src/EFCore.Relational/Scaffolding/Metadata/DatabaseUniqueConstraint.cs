// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
    /// <summary>
    ///     A simple model for a database unique constraint used when reverse engineering an existing database.
    /// </summary>
    public class DatabaseUniqueConstraint : Annotatable
    {
        /// <summary>
        ///     The table on which the unique constraint is defined.
        /// </summary>
        [DisallowNull]
        public virtual DatabaseTable? Table { get; [param: NotNull] set; }

        /// <summary>
        ///     The name of the constraint.
        /// </summary>
        public virtual string? Name { get; [param: CanBeNull] set; }

        /// <summary>
        ///     The ordered list of columns that make up the constraint.
        /// </summary>
        public virtual IList<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        /// <inheritdoc />
        public override string ToString()
            => Name ?? "<UNKNOWN>";
    }
}
