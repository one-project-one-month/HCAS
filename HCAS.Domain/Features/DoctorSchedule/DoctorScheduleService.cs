using HCAS.Domain.Models.DoctorSchedule;
using HCAS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Doctors;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Domain.Features.DoctorSchedule;

public static class DoctorScheduleQuery
{
    public const string Insert = @"
        INSERT INTO DoctorSchedules (DoctorId, ScheduleDate, MaxPatients, del_flg)
        VALUES (@DoctorId, @ScheduleDate, @MaxPatients, 0)";

    public const string ExistsById = @"
        SELECT COUNT(1) FROM DoctorSchedules WHERE Id = @Id";

    public const string Update = @"
        UPDATE DoctorSchedules
        SET DoctorId = @DoctorId, ScheduleDate = @ScheduleDate, MaxPatients = @MaxPatients, del_flg = 0
        WHERE Id = @Id";

    public const string GetAll = @"
        SELECT ds.Id, ds.DoctorId, d.Name AS DoctorName, ds.ScheduleDate, ds.MaxPatients
        FROM DoctorSchedules ds
        INNER JOIN Doctors d ON ds.DoctorId = d.Id";

    public const string SoftDelete = @"
        UPDATE DoctorSchedules SET del_flg = 1 WHERE Id = @Id";

    public const string GetAvailable = @"
        SELECT 
            ds.Id, 
            d.Name AS DoctorName, 
            s.Name AS Specializations,
            ds.ScheduleDate, 
            ds.MaxPatients, 
            COUNT(a.Id) AS AppointmentCount
        FROM DoctorSchedules ds
        INNER JOIN Doctors d ON ds.DoctorId = d.Id
        INNER JOIN Specializations s ON s.Id = d.SpecializationId
        LEFT JOIN Appointments a ON ds.Id = a.ScheduleId
        WHERE ds.ScheduleDate > GETDATE()
        GROUP BY ds.Id, d.Name, ds.ScheduleDate, ds.MaxPatients, s.Name
        ORDER BY ds.ScheduleDate";
}

public class DoctorScheduleService
{
    private readonly DapperService _dapper;
    private readonly AppDbContext _appDbContext;

    public DoctorScheduleService(DapperService dapperService, AppDbContext appDbContext)
    {
        _dapper = dapperService;
        _appDbContext = appDbContext;
    }

    public async Task<Result<DoctorScheduleResModel>> CreateSchedule(DoctorScheduleReqModel dto)
    {
        try
        {
            if (dto.DoctorId <= 0)
                return Result<DoctorScheduleResModel>.ValidationError("Invalid DoctorId");

            if (dto.ScheduleDate < DateTime.Now)
                return Result<DoctorScheduleResModel>.ValidationError("Schedule date cannot be in the past");

            var res = await _dapper.ExecuteAsync(DoctorScheduleQuery.Insert, dto);

            if (res != 1)
                return Result<DoctorScheduleResModel>.SystemError("Failed to create schedule");

            var result = new DoctorScheduleResModel
            {
                DoctorId = dto.DoctorId,
                ScheduleDate = dto.ScheduleDate,
                MaxPatients = dto.MaxPatients
            };

            return Result<DoctorScheduleResModel>.Success(result, "Schedule created successfully");
        }
        catch (Exception ex)
        {
            return Result<DoctorScheduleResModel>.SystemError($"Error creating schedule: {ex.Message}");
        }
    }

    public async Task<Result<DoctorScheduleResModel>> UpdateSchedule(int id, DoctorScheduleReqModel dto)
    {
        try
        {
            var exists = await _dapper.QueryFirstOrDefaultAsync<int>(DoctorScheduleQuery.ExistsById, new { Id = id });

            if (exists == 0)
                return Result<DoctorScheduleResModel>.ValidationError("Schedule does not exist");

            var parameters = new
            {
                Id = id,
                dto.DoctorId,
                dto.ScheduleDate,
                dto.MaxPatients
            };

            var res = await _dapper.ExecuteAsync(DoctorScheduleQuery.Update, parameters);

            if (res != 1)
                return Result<DoctorScheduleResModel>.SystemError("Failed to update schedule");

            var result = new DoctorScheduleResModel
            {
                DoctorId = dto.DoctorId,
                ScheduleDate = dto.ScheduleDate,
                MaxPatients = dto.MaxPatients
            };

            return Result<DoctorScheduleResModel>.Success(result, "Schedule updated successfully");
        }
        catch (Exception ex)
        {
            return Result<DoctorScheduleResModel>.SystemError($"Error updating schedule: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<DoctorScheduleResModel>>> GetAllSchedules()
    {
        try
        {
            var result = await _dapper.QueryAsync<DoctorScheduleResModel>(DoctorScheduleQuery.GetAll);

            var message = result.Any() ? "Success" : "No schedules found";

            return Result<IEnumerable<DoctorScheduleResModel>>.Success(result, message);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DoctorScheduleResModel>>.SystemError($"Error retrieving schedules: {ex.Message}");
        }
    }

    public async Task<Result<DoctorScheduleResModel>> DeleteSchedule(int id)
    {
        try
        {
            var res = await _dapper.ExecuteAsync(DoctorScheduleQuery.SoftDelete, new { Id = id });

            if (res != 1)
                return Result<DoctorScheduleResModel>.ValidationError("Failed to delete schedule");

            return Result<DoctorScheduleResModel>.Success(new DoctorScheduleResModel { Id = id },
                "Schedule deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<DoctorScheduleResModel>.SystemError($"Error deleting schedule: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<DoctorScheduleResModel>>> GetAvailableSchedules()
    {
        try
        {
            var result = await _dapper.QueryAsync<DoctorScheduleResModel>(DoctorScheduleQuery.GetAvailable);

            var message = result.Any() ? "Success" : "No available schedules found";

            return Result<IEnumerable<DoctorScheduleResModel>>.Success(result, message);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DoctorScheduleResModel>>.SystemError(
                $"Error retrieving available schedules: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<DoctorScheduleResponseModel>>> GetDoctorSchedulesAsync(
        int page = 1, int pageSize = 10,
        string? doctorName = null, string? specialization = null, DateTime? scheduleDate = null)
    {
        try
        {
            var query = _appDbContext.DoctorSchedules
                .Include(ds => ds.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(ds => ds.ScheduleDate > DateTime.Now)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(ds => ds.Doctor != null 
                                          && ds.Doctor.Name.Contains(doctorName));
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(ds => ds.Doctor != null 
                                          && ds.Doctor.Specialization.Name.Contains(specialization));
            }

            if (scheduleDate.HasValue)
            {
                query = query.Where(ds => ds.ScheduleDate.HasValue
                                          && ds.ScheduleDate.Value.Date == scheduleDate.Value.Date);
            }

            var total = await query.CountAsync();

            var schedules = await query
                .OrderBy(ds => ds.ScheduleDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ds => new DoctorScheduleResponseModel
                {
                    Id = ds.Id,
                    DoctorId = ds.DoctorId,
                    DoctorName = ds.Doctor!.Name,
                    Specialization = ds.Doctor.Specialization.Name,
                    ScheduleDate = ds.ScheduleDate,
                    MaxPatients = ds.MaxPatients,
                    AppointmentCount = ds.Appointments.Count(),
                    AvailableSlots = ds.MaxPatients - ds.Appointments.Count(a => a.Status != "Cancelled")
                })
                .ToListAsync();

            var pagedResult = new PagedResult<DoctorScheduleResponseModel>
            {
                Items = schedules,
                TotalCount = total
            };

            return schedules.Any()
                ? Result<PagedResult<DoctorScheduleResponseModel>>.Success(pagedResult)
                : Result<PagedResult<DoctorScheduleResponseModel>>.NotFound("No schedules found");
        }
        catch (Exception ex)
        {
            return Result<PagedResult<DoctorScheduleResponseModel>>.SystemError(ex.Message);
        }
    }
}