using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Model.DoctorSchedules;
using HCAS.Shared;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HCAS.Domain.Features.DoctorSchedules
{
    public class DoctorModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PageResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }
    }
    public class DoctorScheduleService
    {
        private readonly DapperService _dapperService;
        private readonly AppDbContext _dbContext;

        public DoctorScheduleService(DapperService dapperService, AppDbContext dbContext)
        {
            _dapperService = dapperService;
            _dbContext = dbContext;
        }

        private static DoctorScheduleResModel MapToResModel(DoctorSchedule schedule) => new DoctorScheduleResModel
        {
            Id = schedule.Id,
            DoctorId = schedule.DoctorId,
            // DoctorName = schedule.Doctor.Name,
            ScheduleDate = schedule.ScheduleDate,
            MaxPatients = schedule.MaxPatients,
            DelFlag = schedule.DelFlg
        };

        private static Result<T> ValidateDoctorScheduleDto<T>(DoctorScheduleReqModel dto)
        {
            if (dto.DoctorId <= 0)
            {
                return Result<T>.ValidationError("Valid doctorId is required");
            }

            if (dto.ScheduleDate < DateTime.Now)
            {
                return Result<T>.ValidationError("Schedule date cannot be in past");
            }

            return null;
        }

        #region CreateSchedule
        public async Task<Result<DoctorScheduleResModel>> CreateScheduleAsync(DoctorScheduleReqModel dto)
        {
            var validation = ValidateDoctorScheduleDto<DoctorScheduleResModel>(dto);
            if (validation != null) return validation;

            try
            {
                var schedule = new DoctorSchedule
                {
                    DoctorId = dto.DoctorId,
                    ScheduleDate = dto.ScheduleDate,
                    MaxPatients = dto.MaxPatients,
                    DelFlg = false
                };

                _dbContext.DoctorSchedules.Add(schedule);
                await _dbContext.SaveChangesAsync();

                return Result<DoctorScheduleResModel>.Success(MapToResModel(schedule), "Schedule registered successfully");
            }
            catch (Exception ex)
            {
                return Result<DoctorScheduleResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region UpdateSchedule
        public async Task<Result<DoctorScheduleResModel>> UpdateScheduleAsync(int id, DoctorScheduleReqModel dto)
        {
            var validation = ValidateDoctorScheduleDto<DoctorScheduleResModel>(dto);
            if (validation != null) return validation;

            try
            {
                var schedule = await _dbContext.DoctorSchedules.FindAsync(id);
                if (schedule is null || schedule.DelFlg != false)
                    return Result<DoctorScheduleResModel>.NotFound("Schedule not found");

                schedule.DoctorId = dto.DoctorId;
                schedule.ScheduleDate = dto.ScheduleDate;
                schedule.MaxPatients = dto.MaxPatients;

                await _dbContext.SaveChangesAsync();

                return Result<DoctorScheduleResModel>.Success(MapToResModel(schedule), "Schedule Update Successfully");
            }
            catch (Exception ex)
            {
                return Result<DoctorScheduleResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region GetSchedules
        public async Task<Result<PageResult<DoctorScheduleResModel>>> GetSchedulesAsyn(
            int page = 1, int pageSize = 10, string? search = null
        )
        {
            try
            {
                var query = _dbContext.DoctorSchedules
                            .Include(ds => ds.Doctor)
                            .Where(ds => ds.DelFlg != true);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(ds => ds.Doctor.Name.Contains(search));
                }



                var total = await query.CountAsync();

                var result = await query
                                .OrderByDescending(ds => ds.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

                var schedules = result.Select(MapToResModel);
                var pageResult = new PageResult<DoctorScheduleResModel>
                {
                    Items = schedules,
                    TotalCount = total
                };

                return schedules.Any()
                ? Result<PageResult<DoctorScheduleResModel>>.Success(pageResult, $"Success. Total: {total}")
                : Result<PageResult<DoctorScheduleResModel>>.NotFound("No schedules found");

            }
            catch (Exception ex)
            {
                return Result<PageResult<DoctorScheduleResModel>>.SystemError(ex.Message);
            }
        }

        #endregion

        #region DeleteSchedule
        public async Task<Result<bool>> DeleteScheduleAsync(int id)
        {
            try
            {
                var schedule = await _dbContext.DoctorSchedules.FindAsync(id);
                if (schedule is null || schedule.DelFlg != false)
                    return Result<bool>.NotFound("Schedule not found");

                schedule.DelFlg = true;
                await _dbContext.SaveChangesAsync();

                return Result<bool>.DeleteSuccess("Schedule deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.SystemError(ex.Message);
            }
        }
        #endregion

        #region GetAvailableSchedules
        public async Task<Result<IEnumerable<DoctorScheduleResModel>>> GetAvailableSchedules()
        {
            try
            {
                Result<IEnumerable<DoctorScheduleResModel>> model = new Result<IEnumerable<DoctorScheduleResModel>>();

                var query = @"SELECT 
                     ds.Id, 
                     d.Name AS DoctorName, 
					 s.Name As Specializations,
                     ds.ScheduleDate, 
                     ds.MaxPatients, 
                     COUNT(a.Id) AS AppointmentCount
                 FROM DoctorSchedules ds
                 INNER JOIN Doctors d ON ds.DoctorId = d.Id
				 inner join Specializations S on s.Id = d.SpecializationId
                 LEFT JOIN Appointments a ON ds.Id = a.ScheduleId
                 WHERE ds.ScheduleDate > GETDATE() 
                 --and Status <> 'Cancelled'
                 GROUP BY ds.Id, d.Name, ds.ScheduleDate, ds.MaxPatients,s.Name
                 ORDER BY ds.ScheduleDate;";

                var result = _dapperService.Query<DoctorScheduleResModel>(query);

                string message = (result.Count == 0) ? "No available schedule found" : "Success";

                model = Result<IEnumerable<DoctorScheduleResModel>>.Success(result, message);

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DoctorScheduleResModel>>.SystemError(ex.Message);
            }

        }
        #endregion

        #region GetDoctors
        public async Task<IEnumerable<DoctorModel>> GetDoctorsAsync()
        {
            return await _dbContext.Doctors
                        .Where(d => !string.IsNullOrWhiteSpace(d.Name))
                        .OrderBy(d => d.Name)
                        .Select(d => new DoctorModel
                        {
                            Id = d.Id,
                            Name = d.Name
                        })
                        .ToListAsync();
        }
        #endregion
    }
}
