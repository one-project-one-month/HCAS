using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Models.Specialization;
using HCAS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HCAS.Domain.Features.Specialization;

public static class SpecializationQuery
{
    public const string GetAll = "SELECT * FROM Specializations WHERE del_flg = 0";

    public const string ExistsById = "SELECT COUNT(1) FROM Specializations WHERE Id = @Id";

    public const string Insert = @"
        INSERT INTO Specializations (Name, del_flg)
        VALUES (@Name, 0)";

    public const string Update = @"
        UPDATE Specializations
        SET Name = @Name, del_flg = 0
        WHERE Id = @Id";

    public const string SoftDelete = "UPDATE Specializations SET del_flg = 1 WHERE Id = @Id";
}

public class SpecializationService
{
    private readonly DapperService _dapper;

    public SpecializationService(DapperService dapperService)
    {
        _dapper = dapperService;
    }

    public async Task<Result<IEnumerable<SpecializationResModel>>> GetAllSpecializationList()
    {
        try
        {
            var result = await _dapper.QueryAsync<SpecializationResModel>(SpecializationQuery.GetAll);

            var message = result.Any() ? "Load data successful." : "No data found.";
            return Result<IEnumerable<SpecializationResModel>>.Success(result, message);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<SpecializationResModel>>.SystemError($"Error loading data: {ex.Message}");
        }
    }

    public async Task<Result<SpecializationResModel>> RegisterSpecializations(SpecializationResModel dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<SpecializationResModel>.ValidationError("Name is required.");

            var res = await _dapper.ExecuteAsync(SpecializationQuery.Insert, dto);

            if (res != 1)
                return Result<SpecializationResModel>.SystemError("Failed to register specialization.");

            return Result<SpecializationResModel>.Success(dto, "Specialization registered successfully.");
        }
        catch (Exception ex)
        {
            return Result<SpecializationResModel>.SystemError($"Error registering specialization: {ex.Message}");
        }
    }

    public async Task<Result<SpecializationReqModel>> UpdateSpecializations(SpecializationReqModel dto, int id)
    {
        try
        {
            var exists = await _dapper.QueryFirstOrDefaultAsync<int>(SpecializationQuery.ExistsById, new { Id = id });

            if (exists == 0)
                return Result<SpecializationReqModel>.ValidationError("Specialization not found.");

            var res = await _dapper.ExecuteAsync(SpecializationQuery.Update, new { Id = id, dto.Name });

            if (res != 1)
                return Result<SpecializationReqModel>.SystemError("Failed to update specialization.");

            return Result<SpecializationReqModel>.Success(dto, "Specialization updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<SpecializationReqModel>.SystemError($"Error updating specialization: {ex.Message}");
        }
    }

    public async Task<Result<SpecializationReqModel>> DeleteSpecializations(int id)
    {
        try
        {
            var res = await _dapper.ExecuteAsync(SpecializationQuery.SoftDelete, new { Id = id });

            if (res != 1)
                return Result<SpecializationReqModel>.ValidationError("Failed to delete specialization.");

            return Result<SpecializationReqModel>.Success(new SpecializationReqModel { Id = id }, "Specialization deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<SpecializationReqModel>.SystemError($"Error deleting specialization: {ex.Message}");
        }
    }
}
