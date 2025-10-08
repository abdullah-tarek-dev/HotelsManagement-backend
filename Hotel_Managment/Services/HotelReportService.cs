using Dapper;
using Npgsql;
using Hotel_Managment.DTOs;

public class HotelReportService
{
    private readonly string _connectionString;

    public HotelReportService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<TopHotelDto>> GetTopHotelsAsync(int limit = 10)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT * FROM get_top_hotels(@limit);";

        var result = await connection.QueryAsync<TopHotelDto>(sql, new { limit });

        return result.ToList();
    }
}
