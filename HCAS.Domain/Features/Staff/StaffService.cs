using HCAS.Shared;

namespace HCAS.Domain.Features.Staff;

public class StaffResModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public string Username { get; set; } = null!;

    public bool DeleteFlag { get; set; }
}

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

public static class StaffQuery
{
    public const string GetAll = @"
        SELECT Id, Name, Email, Phone, Role, Username 
        FROM Staff 
        WHERE del_flg = 0";

    public const string ExistsById = @"
        SELECT COUNT(1) 
        FROM Staff 
        WHERE Id = @Id AND del_flg = 0";

    public const string Insert = @"
        INSERT INTO Staff (Name, Email, Phone, Role, Username, Password, del_flg)
        VALUES (@Name, @Email, @Phone, @Role, @Username, @Password, 0)";

    public const string Update = @"
        UPDATE Staff
        SET Name = @Name, Email = @Email, Phone = @Phone, Role = @Role, Username = @Username, Password = @Password
        WHERE Id = @Id AND del_flg = 0";

    public const string SoftDelete = @"
        UPDATE Staff 
        SET del_flg = 1 
        WHERE Id = @Id AND del_flg = 0";
}

public class StaffService
{
    private readonly DapperService _dapper;

    public StaffService(DapperService dapperService)
    {
        _dapper = dapperService;
    }

    public async Task<Result<IEnumerable<StaffResModel>>> GetAllStaffAsync()
    {
        try
        {
            var staffs = await _dapper.QueryAsync<StaffResModel>(StaffQuery.GetAll);

            if (!staffs.Any())
                return Result<IEnumerable<StaffResModel>>.SystemError("No staff found.");

            return Result<IEnumerable<StaffResModel>>.Success(staffs, StaffQuery.GetAll);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<StaffResModel>>.SystemError($"Error retrieving staff: {ex.Message}");
        }
    }

    public async Task<Result<StaffReqModel>> RegisterStaffAsync(StaffReqModel dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<StaffReqModel>.ValidationError("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                return Result<StaffReqModel>.ValidationError("Email is required.");
            if (string.IsNullOrWhiteSpace(dto.Username))
                return Result<StaffReqModel>.ValidationError("Username is required.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                return Result<StaffReqModel>.ValidationError("Password is required.");

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

    public async Task<Result<StaffReqModel>> UpdateStaffAsync(StaffReqModel dto)
    {
        try
        {
            if (dto.Id <= 0)
                return Result<StaffReqModel>.ValidationError("Invalid staff ID.");

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

    public async Task<Result<StaffReqModel>> DeleteStaffAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result<StaffReqModel>.ValidationError("Invalid staff ID.");

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
}