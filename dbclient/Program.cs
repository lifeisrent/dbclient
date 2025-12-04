using System;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

//"Server=127.0.0.1;Database=FFU_DB;User Id=sa;Password=Sudo123$;TrustServerCertific"
/*
var builder = new SqlConnectionStringBuilder
{
    DataSource = "127.0.0.1",
    InitialCatalog = "Test_ServerDB",
    UserID = "sa",
    Password = "Sudo123$",
    Encrypt = true,
    TrustServerCertificate = true
};
*/


var builder = new SqlConnectionStringBuilder
{
    DataSource = "ASUS\\MONITORTF",
    UserID = "sa",
    Password = "Dosu123$",
    Encrypt = true,
    TrustServerCertificate = true
};


var connectionString = builder.ConnectionString;
using (var connection = new SqlConnection(connectionString))
{
    try
    {
        // 3. 연결 시도
        connection.Open();
        Console.WriteLine("✅ MSSQL 연결 성공");

        //get(connection);
        //insert(connection);
        string query = @"
                SELECT TOP 10 *
                FROM REMOTE2.NSDB.dbo.DCP;
            ";

        using (SqlCommand cmd = new SqlCommand(query, connection))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ 연결 실패: {ex.Message}");
    }
}

void get(SqlConnection connection)
{
    // 4. 예시 쿼리 실행
    string query = "SELECT TOP 10 * FROM Table1;";
    using (SqlCommand command = new SqlCommand(query, connection))
    using (SqlDataReader reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"ID: {reader["id"]}, Timestamp: {reader["timestamp"]}");
        }
    }
}

void insert(SqlConnection connection)
{
    while (true)
    {
        try
        {
            string insertQuery = "INSERT INTO Table1 (timestamp) VALUES (SYSDATETIME());";
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                int rows = command.ExecuteNonQuery();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} - INSERT 성공 ({rows}행 추가)");
            }

            Thread.Sleep(1000); // 1초 대기
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 오류 발생: {ex.Message}");
            break;
        }
    }
}