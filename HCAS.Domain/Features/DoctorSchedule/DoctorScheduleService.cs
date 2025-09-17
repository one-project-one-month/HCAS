using HCAS.Domain.Features.Model.Doctors;
using HCAS.Domain.Features.Model.DoctorSchedule;
using HCAS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.DoctorSchedule
{
    public class DoctorScheduleService
    {
        private readonly DapperService _dapperService;

        public DoctorScheduleService(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        #region CreateSchedule
        public async Task<Result<DoctorScheduleResModel>> CreateSchedule(DoctorScheduleReqModel dto)
        {
            try
            {
                Result<DoctorScheduleResModel> model = new Result<DoctorScheduleResModel>();

                if (dto.DoctorId < 0)
                {
                    model = Result<DoctorScheduleResModel>.ValidationError("Invalid DoctorId");
                    goto Result;
                }

                if (dto.ScheduleDate < DateTime.Now)
                {
                    model = Result<DoctorScheduleResModel>.ValidationError("Schdedule date cannot be in past");
                    goto Result;
                }

                var query = $@"INSERT INTO [dbo].[DoctorSchedules]
                                ([DoctorId]
                                ,[ScheduleDate]
                                ,[MaxPatients]
                                ,[del_flg])
                            VALUES
                                ( @DoctorId 
                                ,@ScheduleDate
                                ,@MaxPatients
                                ,0
                                )";

                var result = new DoctorScheduleResModel
                {
                    DoctorId = dto.DoctorId,
                    ScheduleDate = dto.ScheduleDate,
                    MaxPatients = dto.MaxPatients
                };

                var createDoctorSchedule = _dapperService.Execute(query, result);

                if (createDoctorSchedule != 1)
                {
                    model = Result<DoctorScheduleResModel>.SystemError("Register Failed");
                    goto Result;
                }


                model = Result<DoctorScheduleResModel>.Success(result, "Register Successfully");


            Result:
                return model;

            }
            catch (Exception ex)
            {
                return Result<DoctorScheduleResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region UpdateSchedule
        public async Task<Result<DoctorScheduleResModel>> UpdateSchedule(int id, DoctorScheduleReqModel dto)
        {
            try
            {
                Result<DoctorScheduleResModel> model = new Result<DoctorScheduleResModel>();

                var scheduleExistQuery = @"SELECT COUNT(1) FROM DoctorSchedules WHERE Id = @Id";

                var scheduleExist = _dapperService.QueryFirstOrDefault<int?>(scheduleExistQuery, new { Id = id });

                if (scheduleExist == null)
                {
                    model = Result<DoctorScheduleResModel>.SystemError("Schedule is not exist");
                    goto Result;
                }

                var query = $@"UPDATE [dbo].[DoctorSchedules] SET [DoctorId] = @DoctorId, [ScheduleDate] = @ScheduleDate, [MaxPatients] = @MaxPatients, [del_flg] = 0 WHERE Id = @Id";

                var parameters = new
                {
                    Id = id,
                    DoctorId = dto.DoctorId,
                    ScheduleDate = dto.ScheduleDate,
                    MaxPatients = dto.MaxPatients,
                };

                var updateDoctorSchedule = _dapperService.Execute(query, parameters);

                if (updateDoctorSchedule != 1)
                {
                    model = Result<DoctorScheduleResModel>.SystemError("Fail to update schedule");
                    goto Result;
                }

                var result = new DoctorScheduleResModel
                {
                    DoctorId = dto.DoctorId,
                    ScheduleDate = dto.ScheduleDate,
                    MaxPatients = dto.MaxPatients,
                };

                model = Result<DoctorScheduleResModel>.Success(result, "Update Schedule Successfully");
                goto Result;

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<DoctorScheduleResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region GetSchedules
        public async Task<Result<IEnumerable<DoctorScheduleResModel>>> GetAllSchedules()
        {
            try
            {
                Result<IEnumerable<DoctorScheduleResModel>> model = new Result<IEnumerable<DoctorScheduleResModel>>();

                var query = @"SELECT 
                            ds.Id, 
                            ds.DoctorId,
                            d.Name AS DoctorName, 
                            ds.ScheduleDate, 
                            ds.MaxPatients 
                        FROM DoctorSchedules ds
                        INNER JOIN Doctors d ON ds.DoctorId = d.Id";

                var result = _dapperService.Query<DoctorScheduleResModel>(query);

                string message = (result.Count == 0) ? "No Schedule Found" : "Success";

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

        #region DeleteSchedule
        public async Task<Result<DoctorScheduleResModel>> DeleteSchedule(int id)
        {
            try
            {
                Result<DoctorScheduleResModel> model = new Result<DoctorScheduleResModel>();

                var query = $@"UPDATE [dbo].[DoctorSchedules] SET del_flg = 1 WHERE Id = @Id";

                var parameters = new { Id = id };

                var deletedRow = _dapperService.Execute(query, parameters);

                if (deletedRow != 1)
                {
                    model = Result<DoctorScheduleResModel>.ValidationError("Cannot delete schedule");
                    goto Result;
                }

                model = Result<DoctorScheduleResModel>.Success(new DoctorScheduleResModel { Id = id }, "Success");


            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<DoctorScheduleResModel>.SystemError(ex.Message);
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
    }
}
