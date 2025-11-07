using HCAS.Shared;

namespace HCAS.Domain.Features.Staff;

public class StaffReqModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

}

public class StaffResModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public string Username { get; set; } = null!;

    public bool del_flg { get; set; }
}

public static class StaffQuery
{
    public const string Insert = @"
            INSERT INTO Staff (Name, Email, Phone, Role, Username, Password, del_flg)
            VALUES (@Name, @Email, @Phone, @Role, @Username, @Password, 0)";

    public const string ExistsById = "SELECT COUNT(1) FROM Staff WHERE Id = @Id AND del_flg = 0";

    public const string Update = @"
            UPDATE Staff
            SET Name = @Name,
                Email = @Email,
                Phone = @Phone,
                Role = @Role,
                Username = @Username,
                Password = @Password
            WHERE Id = @Id AND del_flg = 0";

    public const string SoftDelete = "UPDATE Staff SET del_flg = 1 WHERE Id = @Id AND del_flg = 0";

    public const string GetAllPaged = @"
            SELECT Id, Name, Email, Phone, Role, Username
            FROM Staff
            WHERE del_flg = 0
            {0}
            ORDER BY Id DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

    public const string CountAll = @"
            SELECT COUNT(*)
            FROM Staff
            WHERE del_flg = 0
            {0}";
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class StaffService
{
    private readonly DapperService _dapper;

    public StaffService(DapperService dapperService)
    {
        _dapper = dapperService;
    }

    #region GetAllStaffAsync
    public async Task<Result<PagedResult<StaffResModel>>> GetAllStaffAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null)
    {
        try
        {
            string whereClause = "";
            var parameters = new Dictionary<string, object>
            {
                {"Offset", (page - 1) * pageSize},
                {"PageSize", pageSize}
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                whereClause = "AND (Name LIKE @Search OR Username LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            var query = string.Format(StaffQuery.GetAllPaged, whereClause);
            var countQuery = string.Format(StaffQuery.CountAll, whereClause);

            var staffList = await _dapper.QueryAsync<StaffResModel>(query, parameters);
            var totalCount = await _dapper.QueryFirstOrDefaultAsync<int>(countQuery, parameters);

            var result = new PagedResult<StaffResModel>
            {
                Items = staffList ?? Enumerable.Empty<StaffResModel>(),
                TotalCount = totalCount
            };

            return Result<PagedResult<StaffResModel>>.Success(result, "Staff list retrieved successfully.");
        }
        catch (Exception ex)
        {
            return Result<PagedResult<StaffResModel>>.SystemError($"Error retrieving staff list: {ex.Message}");
        }
    }
    #endregion

    #region RegisterStaffAsync
    public async Task<Result<StaffReqModel>> RegisterStaffAsync(StaffReqModel dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return Result<StaffReqModel>.ValidationError("Name, Email, Username, and Password are required.");
            }

            var res = await _dapper.ExecuteAsync(StaffQuery.Insert, dto);

            if (res != 1)
                return Result<StaffReqModel>.SystemError("Failed to register staff.");

            return Result<StaffReqModel>.Success(dto, "Staff registered successfully.");
        }
        catch (Exception ex)
        {
            return Result<StaffReqModel>.SystemError($"Error registering staff: {ex.Message}");
        }
    }
    #endregion

    #region UpdateStaffAsync
    public async Task<Result<StaffReqModel>> UpdateStaffAsync(StaffReqModel dto)
    {
        try
        {
            var exists = await _dapper.QueryFirstOrDefaultAsync<int>(StaffQuery.ExistsById, new { dto.Id });

            if (exists == 0)
                return Result<StaffReqModel>.ValidationError("Staff not found.");

            var res = await _dapper.ExecuteAsync(StaffQuery.Update, dto);

            if (res != 1)
                return Result<StaffReqModel>.SystemError("Failed to update staff.");

            return Result<StaffReqModel>.Success(dto, "Staff updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<StaffReqModel>.SystemError($"Error updating staff: {ex.Message}");
        }
    }
    #endregion

    #region DeleteStaffAsync
    public async Task<Result<StaffReqModel>> DeleteStaffAsync(int id)
    {
        try
        {
            var exists = await _dapper.QueryFirstOrDefaultAsync<int>(StaffQuery.ExistsById, new { Id = id });

            if (exists == 0)
                return Result<StaffReqModel>.ValidationError("Staff not found.");

            var res = await _dapper.ExecuteAsync(StaffQuery.SoftDelete, new { Id = id });

            if (res != 1)
                return Result<StaffReqModel>.SystemError("Failed to delete staff.");

            return Result<StaffReqModel>.DeleteSuccess("Staff deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<StaffReqModel>.SystemError($"Error deleting staff: {ex.Message}");
        }
    }
    #endregion
}
