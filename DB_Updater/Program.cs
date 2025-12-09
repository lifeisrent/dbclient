using System;
using System.Threading;
using Microsoft.Data.SqlClient;

class Program
{
    // 윈폼과 동일한 방식으로 연결 문자열 구성
    private static readonly SqlConnectionStringBuilder Builder = new SqlConnectionStringBuilder
    {
        DataSource = "ASUS\\MONITORTF",
        UserID = "sa",
        Password = "Dosu123$",
        Encrypt = true,
        TrustServerCertificate = true
    };

    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(500); 
    private const string FallbackTableName = "H01RAW_202511";

    static void Main()
    {
        Console.WriteLine("=== MONITOR_DCP 반복 재구성 콘솔 ===");
        Console.WriteLine($"주기: {Interval.TotalMinutes}분마다 실행");
        Console.WriteLine("중지: Ctrl + C");

        while (true)
        {
            Console.WriteLine();
            //Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MONITOR_DCP 재구성 시작");

            try
            {
                RunOnce();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MONITOR_DCP 업데이트");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] SqlException: {ex.Number} - {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine("  Inner: " + ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 일반 오류: {ex.Message}");
            }

            Thread.Sleep(Interval);
        }
    }

    private static void RunOnce()
    {
        var connectionString = Builder.ConnectionString;

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // 필요하면 여기서 master / NSDB 전환
        // TrySwitchToNSDB(connection);  // MONITOR_DCP를 master에 둘 거면 이건 빼라.

        // 테이블 이름 결정 (현재달 -> 이전달 -> fallback)
        string tableName = ResolveSourceTableName(connection);

        // 안전한 MONITOR_DCP 재구성 쿼리 (NEW/OLD 스왑 방식)
        string query = $@"
BEGIN TRY
    BEGIN TRAN;

    ----------------------------------------------------------------
    -- 1) 새 데이터용 테이블 MONITOR_DCP_NEW 먼저 만든다
    ----------------------------------------------------------------
    IF OBJECT_ID('dbo.MONITOR_DCP_NEW', 'U') IS NOT NULL
        DROP TABLE dbo.MONITOR_DCP_NEW;

    WITH RankedData AS (
        SELECT 
            R.*,
            ROW_NUMBER() OVER (PARTITION BY R.DCP_ID ORDER BY R.DT DESC) AS rn
        FROM REMOTE2.NSDB.dbo.[{tableName}] AS R
    )
    SELECT 
          D.Name
        , D.ID
        , R.DCP_ID
        , R.DT
        , R.VALUE
        , R.RAW_VALUE
        , R.STATUS
        , R.STATUS_CHANGED
        , R.rn
    INTO dbo.MONITOR_DCP_NEW
    FROM RankedData AS R
    LEFT JOIN REMOTE2.NSDB.dbo.DCP AS D
        ON R.DCP_ID = D.ID
    WHERE R.rn = 1;

    ----------------------------------------------------------------
    -- 2) 기존 테이블은 이름만 바꿔서 백업으로 빼두고,
    --    새 테이블을 MONITOR_DCP로 승격
    ----------------------------------------------------------------
    IF OBJECT_ID('dbo.MONITOR_DCP_OLD', 'U') IS NOT NULL
        DROP TABLE dbo.MONITOR_DCP_OLD;

    IF OBJECT_ID('dbo.MONITOR_DCP', 'U') IS NOT NULL
        EXEC sp_rename 'dbo.MONITOR_DCP', 'MONITOR_DCP_OLD';

    EXEC sp_rename 'dbo.MONITOR_DCP_NEW', 'MONITOR_DCP';

    ----------------------------------------------------------------
    -- 3) 백업 테이블 삭제 (선택)
    ----------------------------------------------------------------
    IF OBJECT_ID('dbo.MONITOR_DCP_OLD', 'U') IS NOT NULL
        DROP TABLE dbo.MONITOR_DCP_OLD;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;

    IF OBJECT_ID('dbo.MONITOR_DCP_NEW', 'U') IS NOT NULL
        DROP TABLE dbo.MONITOR_DCP_NEW;

    THROW;
END CATCH;
";

        using var cmd = new SqlCommand(query, connection);
        cmd.CommandTimeout = 300;
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// 윈폼 CreateMonitorTableAsync 에 있던 DB 전환 로직 그대로
    /// </summary>
    private static void TrySwitchToNSDB(SqlConnection connection)
    {
        try
        {
            if (connection.Database == "master")
            {
                using var cmdCheck = new SqlCommand(
                    "SELECT database_id FROM sys.databases WHERE name = 'NSDB'",
                    connection
                );
                var result = cmdCheck.ExecuteScalar();
                if (result != null)
                {
                    connection.ChangeDatabase("NSDB");
                    Console.WriteLine("  DB 변경: master → NSDB");
                }
                else
                {
                    //Console.WriteLine("  NSDB 데이터베이스가 없어 master 그대로 사용");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("  DB 전환 중 예외 발생 (무시하고 진행): " + ex.Message);
            // 윈폼도 catch { } 로 무시하니까 여기서도 동일하게 무시
        }
    }

    /// <summary>
    /// 현재달 → 이전달 → fallback(H01RAW_202511) 순으로 원본 테이블 결정
    /// </summary>
    private static string ResolveSourceTableName(SqlConnection connection)
    {
        var currentMonth = DateTime.Now;
        string currentTable = $"H01RAW_{currentMonth:yyyyMM}";

        if (TableExists(connection, currentTable))
        {
            //Console.WriteLine($"  현재 월 테이블 존재: {currentTable}");
            return currentTable;
        }

        var prevMonth = currentMonth.AddMonths(-1);
        string prevTable = $"H01RAW_{prevMonth:yyyyMM}";

        if (TableExists(connection, prevTable))
        {
            Console.WriteLine($"  이전 월 테이블 사용: {prevTable}");
            return prevTable;
        }

        Console.WriteLine($"  현재/이전 월 테이블 없음 → fallback 사용: {FallbackTableName}");
        return FallbackTableName;
    }

    /// <summary>
    /// 윈폼 TableExistsAsync와 동일한 로직 (동기 버전)
    /// </summary>
    private static bool TableExists(SqlConnection connection, string tableName)
    {
        const string sql = @"
SELECT COUNT(*) 
FROM REMOTE2.NSDB.INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = @tableName;
";
        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        int count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        return count > 0;
    }
}
