// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using JetBrains.Annotations;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    /// <summary>
    ///     A <see cref="MigrationOperation" /> for re-starting an existing sequence.
    /// </summary>
    [DebuggerDisplay("ALTER SEQUENCE {Name} RESTART")]
    public class RestartSequenceOperation : MigrationOperation
    {
        /// <summary>
        ///     The name of the sequence.
        /// </summary>
        public virtual string Name { get; [param: NotNull] set; } = null!;

        /// <summary>
        ///     The schema that contains the sequence, or <see langword="null" /> if the default schema should be used.
        /// </summary>
        public virtual string? Schema { get; [param: CanBeNull] set; }

        /// <summary>
        ///     The value at which the sequence should re-start, defaulting to 1.
        /// </summary>
        public virtual long StartValue { get; set; } = 1L;
    }
}
