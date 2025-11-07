using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Patient;
using HCAS.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Specializations;

public class SpecializationReqModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class SpecializationService
{
    private readonly AppDbContext _dbContext;

    public SpecializationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static SpecializationResModel MapToResModel(Specialization specialization) => new SpecializationResModel
    {
        Id = specialization.Id,
        Name = specialization.Name,    
        DelFlg = specialization.DelFlg
    };

    private static Result<T> ValidateSpecializationDto<T>(SpecializationReqModel dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<T>.ValidationError("Name is required");

        return null!;
    }

    public async Task<Result<PagedResult<SpecializationResModel>>> GetSpecializationAsync(
        int page = 1, int pageSize = 10, string? search = null, int? specializationId = null)
    {
        try
        {
            var query = _dbContext.Specializations           
                .Where(d => !d.DelFlg);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.Name.Contains(search));

            var total = await query.CountAsync();

            var result = await query
                .OrderByDescending(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var specialization = result.Select(MapToResModel);
            var pagedResult = new PagedResult<SpecializationResModel>
            {
                Items = specialization,
                TotalCount = total
            };

            return specialization.Any()
                ? Result<PagedResult<SpecializationResModel>>.Success(pagedResult, $"Success. Total: {total}")
                : Result<PagedResult<SpecializationResModel>>.NotFound("No Specialization found");
        }
        catch (Exception ex)
        {
            return Result<PagedResult<SpecializationResModel>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SpecializationResModel>> RegisterSpecializationAsync(SpecializationReqModel dto)
    {
        var validation = ValidateSpecializationDto<SpecializationResModel>(dto);
        if (validation != null) return validation;

        try
        {
            var specialization = new Specialization
            {
                Name = dto.Name,                
                DelFlg = false
            };

            _dbContext.Specializations.Add(specialization);
            await _dbContext.SaveChangesAsync();

            return Result<SpecializationResModel>.Success(MapToResModel(specialization), "Specialization registered successfully");
        }
        catch (Exception ex)
        {
            return Result<SpecializationResModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SpecializationResModel>> UpdateSpecializationAsync(int id, SpecializationReqModel dto)
    {
        var validation = ValidateSpecializationDto<SpecializationResModel>(dto);
        if (validation != null) return validation;

        try
        {
            var specialization = await _dbContext.Specializations.FindAsync(id);
            if (specialization is null || specialization.DelFlg)
                return Result<SpecializationResModel>.NotFound("Specialization not found");
                      
            specialization.Name = dto.Name;           

            await _dbContext.SaveChangesAsync();

            return Result<SpecializationResModel>.Success(MapToResModel(specialization), "Specialization updated successfully");
        }
        catch (Exception ex)
        {
            return Result<SpecializationResModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteSpecializationAsync(int id)
    {
        try
        {
            var specialization = await _dbContext.Specializations.FindAsync(id);
            if (specialization is null || specialization.DelFlg)
                return Result<bool>.NotFound("Specialization not found");

            specialization.DelFlg = true;
            await _dbContext.SaveChangesAsync();

            return Result<bool>.DeleteSuccess("Specialization deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<bool>.SystemError(ex.Message);
        }
    }    
}