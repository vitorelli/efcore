// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Utilities;

#nullable enable

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         Reads result sets from a relational database.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class RelationalDataReader : IDisposable, IAsyncDisposable
    {
        private readonly IRelationalCommand _relationalCommand;

        private IRelationalConnection _relationalConnection = default!;
        private DbCommand _command = default!;
        private DbDataReader _reader = default!;
        private Guid _commandId;
        private IDiagnosticsLogger<DbLoggerCategory.Database.Command>? _logger;
        private DateTimeOffset _startTime;
        private readonly Stopwatch _stopwatch = new();

        private int _readCount;

        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelationalDataReader" /> class.
        /// </summary>
        /// <param name="relationalCommand"> The relational command which owns this relational reader. </param>
        public RelationalDataReader([NotNull] IRelationalCommand relationalCommand)
        {
            Check.NotNull(relationalCommand, nameof(relationalCommand));

            _relationalCommand = relationalCommand;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelationalDataReader" /> class.
        /// </summary>
        /// <param name="relationalConnection"> The relational connection. </param>
        /// <param name="command"> The command that was executed. </param>
        /// <param name="reader"> The underlying reader for the result set. </param>
        /// <param name="commandId"> A correlation ID that identifies the <see cref="DbCommand" /> instance being used. </param>
        /// <param name="logger"> The diagnostic source. </param>
        public virtual void Initialize(
            [NotNull] IRelationalConnection relationalConnection,
            [NotNull] DbCommand command,
            [NotNull] DbDataReader reader,
            Guid commandId,
            [CanBeNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command>? logger)
        {
            Check.NotNull(command, nameof(command));
            Check.NotNull(reader, nameof(reader));

            _relationalConnection = relationalConnection;
            _command = command;
            _reader = reader;
            _commandId = commandId;
            _logger = logger;
            _startTime = DateTimeOffset.UtcNow;
            _disposed = false;
            _stopwatch.Restart();
        }

        /// <summary>
        ///     Gets the underlying reader for the result set.
        /// </summary>
        public virtual DbDataReader DbDataReader
            => _reader;

        /// <summary>
        ///     Gets the underlying command for the result set.
        /// </summary>
        public virtual DbCommand DbCommand
            => _command;

        /// <summary>
        ///     Calls <see cref="DbDataReader.Read()" /> on the underlying <see cref="System.Data.Common.DbDataReader" />.
        /// </summary>
        /// <returns> <see langword="true" /> if there are more rows; otherwise <see langword="false" />. </returns>
        public virtual bool Read()
        {
            _readCount++;

            return _reader.Read();
        }

        /// <summary>
        ///     Calls <see cref="DbDataReader.ReadAsync(CancellationToken)" /> on the underlying
        ///     <see cref="System.Data.Common.DbDataReader" />.
        /// </summary>
        /// <returns> <see langword="true" /> if there are more rows; otherwise <see langword="false" />. </returns>
        public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        {
            _readCount++;

            return _reader.ReadAsync(cancellationToken);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                var interceptionResult = default(InterceptionResult);
                try
                {
                    _reader.Close(); // can throw

                    if (_logger != null)
                    {
                        interceptionResult = _logger.DataReaderDisposing(
                            _relationalConnection,
                            _command,
                            _reader,
                            _commandId,
                            _reader.RecordsAffected,
                            _readCount,
                            _startTime,
                            _stopwatch.Elapsed); // can throw
                    }
                }
                finally
                {
                    _disposed = true;

                    if (!interceptionResult.IsSuppressed)
                    {
                        _reader.Dispose();
                        _command.Parameters.Clear();
                        _command.Dispose();
                        _relationalConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                var interceptionResult = default(InterceptionResult);
                try
                {
                    await _reader.CloseAsync().ConfigureAwait(false); // can throw

                    if (_logger != null)
                    {
                        interceptionResult = _logger.DataReaderDisposing(
                            _relationalConnection,
                            _command,
                            _reader,
                            _commandId,
                            _reader.RecordsAffected,
                            _readCount,
                            _startTime,
                            _stopwatch.Elapsed); // can throw
                    }
                }
                finally
                {
                    _disposed = true;

                    if (!interceptionResult.IsSuppressed)
                    {
                        await _reader.DisposeAsync().ConfigureAwait(false);
                        _command.Parameters.Clear();
                        await _command.DisposeAsync().ConfigureAwait(false);
                        await _relationalConnection.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
