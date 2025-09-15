using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Model.Doctors;
using HCAS.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Doctors
{
    public class DoctorService
    {
        private readonly DapperService _dapperService;

        public DoctorService(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        #region  GetAllDoctorsList
        public async Task<Result<IEnumerable<DoctorsResModel>>> GetAllDoctorsList()
        {
            try
            {
                Result<IEnumerable<DoctorsResModel>> model = new Result<IEnumerable<DoctorsResModel>>();

                var query = @"SELECT D.*, S.Id as SpecializationId, S.Name as SpecializationName, S.Name as Specialization FROM Doctors D
            INNER JOIN Specializations S on D.SpecializationId = S.Id";

                var result = _dapperService.Query<DoctorsResModel>(query);
                //var result = await Task.Run(() => _dapperService.Query<DoctorsResModel>(query));

                if (result.Count < 0)
                {
                    model = Result<IEnumerable<DoctorsResModel>>.SystemError("No Data Found");
                    goto Result;
                }
               model = Result<IEnumerable<DoctorsResModel>>.Success(result, query);

            Result:
                return model;

            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DoctorsResModel>>.SystemError(ex.Message);
            }
        }
        #endregion

        #region RegisterDoctor
        public async Task<Result<DoctorsResModel>> RegisterDoctor(DoctorsReqModel dto)
        {
            try
            {
                Result<DoctorsResModel> model = new Result<DoctorsResModel>();

                if (string.IsNullOrEmpty(dto.Name))
                {
                    model = Result<DoctorsResModel>.ValidationError("Name is required");
                    goto Result;
                }

                if (dto.SpecializationId < 0)
                {
                    model = Result<DoctorsResModel>.ValidationError("Invalid SpecializationId");
                    goto Result;
                }

                var query = $@"INSERT INTO [dbo].[Doctors]
                               ([Name]
                               ,[SpecializationId]
                               ,[del_flg])
                         VALUES
                              ( @Name
		                       ,@SpecializationId
                               ,0
                            )";

                var result = new DoctorsResModel
                {
                    Name = dto.Name,
                    SpecializationId = dto.SpecializationId
                };

                var createDoctor = _dapperService.Execute(query, result);



                if (createDoctor != 1)
                {
                    model = Result<DoctorsResModel>.SystemError("Register Failed");
                    goto Result;
                }

                model = Result<DoctorsResModel>.Success(result, "Doctor Registered Successfully");

            Result:
                return model;

            }
            catch (Exception ex)
            {
                return Result<DoctorsResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region UpdateDoctor
        public async Task<Result<DoctorsReqModel>> UpdateDoctor(DoctorsReqModel dto)
        {
            try
            {
                Result<DoctorsReqModel> model = new Result<DoctorsReqModel>();

                var doctorExistsQuery = "SELECT COUNT(1) FROM Doctors WHERE Id = @Id";

                 if(doctorExistsQuery is null)
                {
                    Result<DoctorsReqModel>.ValidationError("Doctor not found");
                }

                var query = @"UPDATE [dbo].[Doctors]
                               SET [Name] = @Name
                                  ,[SpecializationId] = @SpecializationId
                                  ,[del_flg] = 0
                             WHERE Id = @Id";

                var result = new DoctorsReqModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    SpecializationId = dto.SpecializationId
                };

                var updateDoctor = await Task.Run(() => _dapperService.Execute(query, result));

                if (updateDoctor != 1)
                {
                    model = Result<DoctorsReqModel>.SystemError("Update Failed");
                    return model;
                }

                model = Result<DoctorsReqModel>.Success(result, "Doctor Updated Successfully");
                return model;
            }
            catch (Exception ex)
            {
               
                return Result<DoctorsReqModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region DeleteDoctor
        public async Task<Result<DoctorsReqModel>> DeleteDoctor(int id)
        {
            try
            {
                Result<DoctorsReqModel> model = new Result<DoctorsReqModel>();

                var query = @"UPDATE [dbo].[Doctors] SET del_flg = 1
                      WHERE Id = @Id";

                var parameters = new { Id = id };

                var deleteDoctor = await Task.Run(() => _dapperService.Execute(query, parameters));

                if (deleteDoctor != 1)
                {
                    model = Result<DoctorsReqModel>.ValidationError("Cannot be deleted");
                    goto Result;
                }

                model = Result<DoctorsReqModel>.Success(new DoctorsReqModel { Id = id }, "Successfully deleted the doctor");

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<DoctorsReqModel>.SystemError(ex.Message);
            }
        }
        #endregion


    }
}; 


