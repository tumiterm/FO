using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Infrastructure.Repository
{
    public class BackgroundJobQueueRepository : Repository<BackgroundJobQueueItem>, IBackgroundJobQueue
    {
        private readonly ApplicationDbContext _db;

        public BackgroundJobQueueRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<BackgroundJobQueueItem?> TryClaimNextAsync(string queue, string workerId, TimeSpan lockDuration, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(queue))
            {
                queue = "default";
            }

            if (string.IsNullOrWhiteSpace(workerId))
            {
                workerId = Environment.MachineName;
            }

            var now = DateTimeOffset.UtcNow;
            var lockedUntil = now.Add(lockDuration);

            var sql = @"
                        ;WITH cte AS
                        (
                            SELECT TOP (1) *
                            FROM [FO].[BackgroundJobQueueItem] WITH (READPAST, UPDLOCK, ROWLOCK)
                            WHERE [IsDeleted] = 0
                              AND [Queue] = @Queue
                              AND (
                                    [Status] = 'Pending'
                                    OR ([Status] = 'Processing' AND ([LockedUntilUtc] IS NULL OR [LockedUntilUtc] < @Now))
                                  )
                            ORDER BY [DateCreated] ASC
                        )
                        UPDATE cte
                        SET
                            [Status] = 'Processing',
                            [LockedUntilUtc] = @LockedUntil,
                            [LockedBy] = @WorkerId,
                            [Attempts] = CASE WHEN [Status] = 'Pending' THEN [Attempts] ELSE [Attempts] END,
                            [DateModified] = @Now
                        OUTPUT INSERTED.*;
                        ";

            await using var conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct).ConfigureAwait(false);
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add(new SqlParameter("@Queue", SqlDbType.NVarChar, 64) { Value = queue });
            cmd.Parameters.Add(new SqlParameter("@WorkerId", SqlDbType.NVarChar, 128) { Value = workerId });
            cmd.Parameters.Add(new SqlParameter("@Now", SqlDbType.DateTimeOffset) { Value = now });
            cmd.Parameters.Add(new SqlParameter("@LockedUntil", SqlDbType.DateTimeOffset) { Value = lockedUntil });

            await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await reader.ReadAsync(ct).ConfigureAwait(false))
            {
                return null;
            }

            var item = new BackgroundJobQueueItem
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Queue = reader["Queue"]?.ToString() ?? "default",
                JobType = reader["JobType"]?.ToString() ?? string.Empty,
                PayloadJson = reader["PayloadJson"]?.ToString() ?? "{}",
                Status = reader["Status"]?.ToString() ?? "Processing",
                Attempts = reader["Attempts"] is int a ? a : Convert.ToInt32(reader["Attempts"]),
                LockedBy = reader["LockedBy"] as string,
                LockedUntilUtc = reader["LockedUntilUtc"] as DateTimeOffset?,
                LastError = reader["LastError"] as string,
                ProcessedUtc = reader["ProcessedUtc"] as DateTimeOffset?,
                IsDeleted = reader["IsDeleted"] is bool b && b,
                DateCreated = (DateTimeOffset)reader["DateCreated"],
                DateModified = (DateTimeOffset)reader["DateModified"],
                UserCreated = reader["UserCreated"] as string,
                UserModified = reader["UserModified"] as string,
                Code = reader["Code"] as string,
                Name = reader["Name"] as string,
                DateDeleted = reader["DateDeleted"] as DateTimeOffset?
            };

            return item;
        }

        public Task<BackgroundJobQueueItem> UpdateAsync(BackgroundJobQueueItem item, CancellationToken ct = default)
        {
            _db.BackgroundJobQueueItems.Update(item);
            return Task.FromResult(item);
        }
    }

}
