using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Model.Doctors;
using HCAS.Domain.Features.Model.Specialization;
using HCAS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HCAS.Domain.Features.Specialization
{
    public class SpecializationSerivce
    {
        private readonly DapperService _dapperService;

        public SpecializationSerivce(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        #region  GetAllSpecializationsList
        public async Task<Result<IEnumerable<SpecializationResModel>>> GetAllSpecializationList()
        {
            try
            {
                Result<IEnumerable<SpecializationResModel>> model = new Result<IEnumerable<SpecializationResModel>>();

                var query = @"SELECT * FROM Specializations WHERE del_flg = 0";

                var result = _dapperService.Query<SpecializationResModel>(query);

                if (result.Count < 0)
                {
                    model = Result<IEnumerable<SpecializationResModel>>.SystemError("No Data Found.");
                    goto Result;
                }

                string message = (result.Count == 0) ? "No Data Found." : "Load Data Successful.";

                model = Result<IEnumerable<SpecializationResModel>>.Success(result, message);

            Result:
                return model;

            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SpecializationResModel>>.SystemError(ex.Message);
            }
        }
        #endregion

        #region RegisterSpecializations
        public async Task<Result<SpecializationResModel>> RegisterSpecializations(SpecializationResModel dto)
        {
            try
            {
                Result<SpecializationResModel> model = new Result<SpecializationResModel>();

                if (string.IsNullOrEmpty(dto.Name))
                {
                    model = Result<SpecializationResModel>.ValidationError("Name is required");
                    goto Result;
                }

                var query = $@"INSERT INTO [dbo].[Specializations]
                               ([Name]                              
                               ,[del_flg])
                         VALUES
                              ( @Name		                       
                               ,0
                            )";

                var result = new SpecializationResModel
                {
                    Name = dto.Name
                };

                var createSpecializations = _dapperService.Execute(query, result);

                if (createSpecializations != 1)
                {
                    model = Result<SpecializationResModel>.SystemError("Register Failed");
                    goto Result;
                }

                model = Result<SpecializationResModel>.Success(result, "Specializations Registered Successfully");

            Result:
                return model;

            }
            catch (Exception ex)
            {
                return Result<SpecializationResModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region UpdateSpecializations
        public async Task<Result<SpecializationReqModel>> UpdateSpecializations(SpecializationReqModel dto, int id)
        {
            try
            {
                Result<SpecializationReqModel> model = new Result<SpecializationReqModel>();

                var checkSpecializationExitsQuery = "SELECT COUNT(1) FROM Specializations WHERE Id = @Id";

                var resultExits = await Task.Run(() => _dapperService.Query<SpecializationResModel>(checkSpecializationExitsQuery, new { Id = id }));            

                if (resultExits.Count < 0)
                {
                    model =  Result<SpecializationReqModel>.ValidationError("Specialization not found");
                    return model;
                }

                var updateQuery = @"UPDATE [dbo].[Specializations]
                            SET [Name] = @Name                             
                                ,[del_flg] = 0
                            WHERE Id = @Id";

                var parameters = new 
                { Id = id,
                  Name = dto.Name
                };      

                var updateSpecializations = _dapperService.Execute(updateQuery, parameters);

                if (updateSpecializations != 1)
                {
                    model = Result<SpecializationReqModel>.SystemError("Update Failed");
                    return model;                    
                }

                model = Result<SpecializationReqModel>.Success(dto, "Specialization Updated Successfully");
                return model;             

            }
            catch (Exception ex)
            {
                return Result<SpecializationReqModel>.SystemError(ex.Message);
            }
        }
        #endregion

        #region DeleteSpecializations
        public async Task<Result<SpecializationReqModel>> DeleteSpecializations(int id)
        {
            try
            {
                Result<SpecializationReqModel> model = new Result<SpecializationReqModel>();

                var query = @"UPDATE [dbo].[Specializations] SET del_flg = 1
                      WHERE Id = @Id";

                var parameters = new { Id = id };

                var deleteSpecializations = await Task.Run(() => _dapperService.Execute(query, parameters));

                if (deleteSpecializations != 1)
                {
                    model = Result<SpecializationReqModel>.ValidationError("Cannot be deleted");
                    goto Result;
                }

                model = Result<SpecializationReqModel>.Success(new SpecializationReqModel { Id = id }, "Successfully deleted the Specializations");

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<SpecializationReqModel>.SystemError(ex.Message);
            }
        }
        #endregion
    }
}
