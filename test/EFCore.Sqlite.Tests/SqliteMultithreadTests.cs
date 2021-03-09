// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class SqliteMultithreadTests
    {
        [ConditionalFact]
        public void IsMultithreadSupported_when_using_SingleConnection()
        {
            int maxThreads = 5;
            var tasks = new Task[maxThreads];
            var singleConnection = new SqliteConnection("Data Source=Maltesers");
            singleConnection.Open();

            singleConnection.CreateFunction("sleep", () => Delay());

            for (int i = 0; i < maxThreads; i++)
            {
                if (i == 0)
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        using (var cmd = singleConnection.CreateCommand())
                        {
                            cmd.CommandText = "select sleep()";
                            cmd.ExecuteNonQuery();
                        }
                    });
                }
                else
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        using var context = new ProviderContext(
                            new DbContextOptionsBuilder()
                                .UseInternalServiceProvider(
                                    new ServiceCollection()
                                        .AddEntityFrameworkSqlite()
                                        .BuildServiceProvider())
                                .UseSqlite(singleConnection).Options);

                        Assert.NotNull(context.Database.GetDbConnection());
                    });
                }
            }

            Task.WaitAll(tasks);
        }

        private static string Delay()
        {
            Task.Delay(5000).Wait();

            return string.Empty;
        }

        private class ProviderContext : DbContext
        {
            public ProviderContext(DbContextOptions options)
                : base(options)
            {
            }
        }
    }
}
