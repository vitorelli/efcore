// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
    /// <summary>
    ///     A simple model for a database primary key used when reverse engineering an existing database.
    /// </summary>
    public class DatabasePrimaryKey : Annotatable
    {
        /// <summary>
        ///     The table on which the primary key is defined.
        /// </summary>
        public virtual DatabaseTable? Table { get; [param: CanBeNull] set; }

        /// <summary>
        ///     The name of the primary key.
        /// </summary>
        public virtual string? Name { get; [param: CanBeNull] set; }

        /// <summary>
        ///     The ordered list of columns that make up the primary key.
        /// </summary>
        public virtual IList<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        /// <inheritdoc />
        public override string ToString()
            => Name ?? "<UNKNOWN>";
    }
}
