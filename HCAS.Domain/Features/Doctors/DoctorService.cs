using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Model.Doctors;
using HCAS.Domain.Features.Model.Patient;
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

                string message = (result.Count == 0) ? "No Doctor found" : "Success";
                model = Result<IEnumerable<DoctorsResModel>>.Success(result, message);

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
        public async Task<Result<DoctorsResModel>> UpdateDoctor(DoctorsReqModel dto, int id)
        {
            try
            {
                Result<DoctorsResModel> model = new Result<DoctorsResModel>();


                var doctorExistsQuery = "SELECT Id FROM Doctors WHERE Id = @Id";
                var doctorExists = await Task.Run(() =>
                    _dapperService.QueryFirstOrDefault<int?>(doctorExistsQuery, new { Id = id })
                );

                if (doctorExists == null)
                {
                    model = Result<DoctorsResModel>.ValidationError("Doctor not found");
                    goto Result;
                }


                var updateQuery = @"
            UPDATE [dbo].[Doctors]
            SET [Name] = @Name,
                [SpecializationId] = @SpecializationId,
                [del_flg] = 0
            WHERE Id = @Id";

                var parameters = new
                {
                    Id = id,
                    Name = dto.Name,
                    SpecializationId = dto.SpecializationId
                };

                var updatedRows = await Task.Run(() => _dapperService.Execute(updateQuery, parameters));

                if (updatedRows != 1)
                {
                    model = Result<DoctorsResModel>.SystemError("Update Failed");
                    goto Result;
                }


                model = Result<DoctorsResModel>.Success(new DoctorsResModel
                {
                    Id = id,
                    Name = dto.Name,
                    SpecializationId = dto.SpecializationId
                }, "Doctor Updated Successfully");

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<DoctorsResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region DeleteDoctor
        public async Task<Result<DoctorsReqModel>> DeleteDoctor(int id)
        {
            try
            {
                Result<DoctorsReqModel> model = new Result<DoctorsReqModel>();

                var query = @"UPDATE [dbo].[Doctors] SET del_flg = 1 WHERE Id = @Id";
                var parameters = new { Id = id };

                var deletedRows = _dapperService.Execute(query, parameters);

                if (deletedRows != 1)
                {
                    model = Result<DoctorsReqModel>.ValidationError("Cannot delete doctor");
                    goto Result;
                }

                model = Result<DoctorsReqModel>.Success(new DoctorsReqModel(), "Successfully deleted the doctor");

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<DoctorsReqModel>.SystemError(ex.Message);
            }

            #endregion
        }
    }
 }; 


