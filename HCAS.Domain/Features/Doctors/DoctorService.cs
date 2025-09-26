using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Models.Doctors;
using HCAS.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Doctors;

public class SpecializationModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}

public class DoctorService
{
    private readonly AppDbContext _dbContext;

    public DoctorService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static DoctorsResModel MapToResModel(Doctor doctor) => new DoctorsResModel
    {
        Id = doctor.Id,
        Name = doctor.Name,
        SpecializationId = doctor.SpecializationId,
        DelFlg = doctor.DelFlg
    };

    private static Result<T> ValidateDoctorDto<T>(DoctorsReqModel dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<T>.ValidationError("Name is required");

        if (dto.SpecializationId <= 0)
            return Result<T>.ValidationError("Valid SpecializationId is required");

        return null!;
    }

    public async Task<Result<PagedResult<DoctorsResModel>>> GetDoctorsAsync(
        int page = 1, int pageSize = 10, string? search = null, int? specializationId = null)
    {
        try
        {
            var query = _dbContext.Doctors
                .Include(d => d.Specialization)
                .Where(d => !d.DelFlg);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.Name.Contains(search));

            if (specializationId.HasValue)
                query = query.Where(d => d.SpecializationId == specializationId);

            var total = await query.CountAsync();

            var result = await query
                .OrderByDescending(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var doctors = result.Select(MapToResModel);
            var pagedResult = new PagedResult<DoctorsResModel>
            {
                Items = doctors,
                TotalCount = total
            };

            return doctors.Any()
                ? Result<PagedResult<DoctorsResModel>>.Success(pagedResult, $"Success. Total: {total}")
                : Result<PagedResult<DoctorsResModel>>.NotFound("No doctors found");
        }
        catch (Exception ex)
        {
            return Result<PagedResult<DoctorsResModel>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<DoctorsResModel>> RegisterDoctorAsync(DoctorsReqModel dto)
    {
        var validation = ValidateDoctorDto<DoctorsResModel>(dto);
        if (validation != null) return validation;

        try
        {
            var doctor = new Doctor
            {
                Name = dto.Name,
                SpecializationId = dto.SpecializationId,
                DelFlg = false
            };

            _dbContext.Doctors.Add(doctor);
            await _dbContext.SaveChangesAsync();

            return Result<DoctorsResModel>.Success(MapToResModel(doctor), "Doctor registered successfully");
        }
        catch (Exception ex)
        {
            return Result<DoctorsResModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<DoctorsResModel>> UpdateDoctorAsync(int id, DoctorsReqModel dto)
    {
        var validation = ValidateDoctorDto<DoctorsResModel>(dto);
        if (validation != null) return validation;

        try
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor is null || doctor.DelFlg)
                return Result<DoctorsResModel>.NotFound("Doctor not found");

            doctor.Name = dto.Name;
            doctor.SpecializationId = dto.SpecializationId;

            await _dbContext.SaveChangesAsync();

            return Result<DoctorsResModel>.Success(MapToResModel(doctor), "Doctor updated successfully");
        }
        catch (Exception ex)
        {
            return Result<DoctorsResModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteDoctorAsync(int id)
    {
        try
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor is null || doctor.DelFlg)
                return Result<bool>.NotFound("Doctor not found");

            doctor.DelFlg = true;
            await _dbContext.SaveChangesAsync();

            return Result<bool>.DeleteSuccess("Doctor deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<bool>.SystemError(ex.Message);
        }
    }

    public async Task<IEnumerable<SpecializationModel>> GetSpecializationsAsync()
    {
        return await _dbContext.Specializations
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .OrderBy(s => s.Name)
            .Select(s => new SpecializationModel
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
    }
}