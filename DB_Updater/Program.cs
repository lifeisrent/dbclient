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
        // 윈폼과 동일한 문자열 사용
        var connectionString = Builder.ConnectionString;

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // 윈폼의 CreateMonitorTableAsync 에 있던 DB 전환 로직 그대로
        TrySwitchToNSDB(connection);

        // 테이블 이름 결정 (현재달 -> 이전달 -> fallback)
        string tableName = ResolveSourceTableName(connection);

        //Console.WriteLine($"  Target Database: {connection.Database}");
        //Console.WriteLine($"  Source Table   : REMOTE2.NSDB.dbo.[{tableName}]");

        // MONITOR_DCP 생성 쿼리 (네 코드 그대로, 날짜/테이블만 치환)
        string query = $@"
IF OBJECT_ID('dbo.MONITOR_DCP', 'U') IS NOT NULL
    DROP TABLE dbo.MONITOR_DCP;

WITH RankedData AS (
    SELECT 
        R.*,
        ROW_NUMBER() OVER (PARTITION BY R.DCP_ID ORDER BY R.DT DESC) as rn
    FROM REMOTE2.NSDB.dbo.[{tableName}] R
)
SELECT 
    D.Name,
    D.ID, 
    R.*
INTO dbo.MONITOR_DCP
FROM RankedData R
LEFT JOIN REMOTE2.NSDB.dbo.DCP D ON R.DCP_ID = D.ID
WHERE R.rn = 1
ORDER BY R.DT DESC, R.DCP_ID ASC;
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
