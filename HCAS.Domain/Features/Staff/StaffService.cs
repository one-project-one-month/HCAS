using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.Shared;
using HCAS.Database.AppDbContextModels;

using System.Security.Cryptography.X509Certificates;
using HCAS.Domain.Models.Staff;

namespace HCAS.Domain.Features.Staff
{
    public class StaffService
    {
        private readonly DapperService _dapperService;

        public StaffService(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        #region GetAllStaffAsync
        public async Task<Result<IEnumerable<StaffResModel>>> GetAllStaffAsync()
        {
            try
            {

                Result<IEnumerable<StaffResModel>> ResModel = new Result<IEnumerable<StaffResModel>>();

                string query = "SELECT Id, Name, Email, Phone, Role, Username FROM Staff WHERE del_flg = 0";

                var staffs = await Task.Run(()=> _dapperService.Query<StaffResModel>(query));

                if (staffs is null)
                {
                    ResModel = Result<IEnumerable<StaffResModel>>.SystemError("No staffs found.");
                    goto Result;
                }
                ResModel = Result<IEnumerable<StaffResModel>>.Success(staffs, query);

            Result:
                return ResModel;
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffResModel>>.SystemError("An error occurred while retrieving staffs: " + ex.Message);
            }
        }
        #endregion

        #region RegisterStaffAsync
        public async Task<Result<StaffReqModel>> RegisterStaffAsync(StaffReqModel dto)
        {
            try
            {
                Result<StaffReqModel> ReqModel = new Result<StaffReqModel>();

                if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                {
                    ReqModel = Result<StaffReqModel>.ValidationError("Name, Email, Username, and Password are required.");
                    goto Result;
                }
                var query = $@"INSERT INTO [dbo].[Staff]
                        ([Name]
                        ,[Email]
                        ,[Phone]
                        ,[Role]
                        ,[Username]
                        ,[Password]
                        ,[del_flg])
                VALUES
                        (@Name
                        ,@Email
                        ,@Phone
                        ,@Role
                        ,@Username
                        ,@Password
                        ,0
                        )";
                var result = new StaffReqModel
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Role = dto.Role,
                    Username = dto.Username,
                    Password = dto.Password
                };

                var res = await Task.Run(() => _dapperService.Execute(query, result));

                if (res != 1)
                {
                    ReqModel = Result<StaffReqModel>.SystemError("Failed to register staff.");
                    goto Result;
                }
                ReqModel = Result<StaffReqModel>.Success(result, "Staff Registered Successfully");
            Result:
                return ReqModel;
            }
            catch (Exception ex)
            {
                return Result<StaffReqModel>.SystemError("An error occurred while registering staff: " + ex.Message);
            }
        }
            #endregion

        # region UpdateStaffAsync
        public async Task<Result<StaffReqModel>> UpdateStaffAsync(StaffReqModel dto)
        {
            try
            {
                Result<StaffReqModel> ReqModel = new Result<StaffReqModel>();
                if (dto.Id <= 0)
                {
                    ReqModel = Result<StaffReqModel>.ValidationError("Invalid Staff ID.");
                    goto Result;
                }
                var existingStaffQuery = "SELECT COUNT(1) FROM Staff WHERE Id = @Id AND del_flg = 0";
                if (existingStaffQuery is null)
                {
                    Result<StaffReqModel>.ValidationError("Staff not found.");
                }
                var updateQuery = $@"UPDATE [dbo].[Staff]
                        SET [Name] = @Name
                        ,[Email] = @Email
                        ,[Phone] = @Phone
                        ,[Role] = @Role
                        ,[Username] = @Username
                        ,[Password] = @Password
                        WHERE Id = @Id AND del_flg = 0";

                var result = new StaffReqModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Role = dto.Role,
                    Username = dto.Username,
                    Password = dto.Password
                };
                var res = await Task.Run(()=> _dapperService.Execute(updateQuery, result));
                if (res != 1)
                {
                    ReqModel = Result<StaffReqModel>.SystemError("Failed to update staff.");
                    goto Result;
                }
                ReqModel = Result<StaffReqModel>.Success(result, "Staff Updated Successfully");
            Result:
                return ReqModel;
            }
            catch (Exception ex)
            {
                return Result<StaffReqModel>.SystemError("An error occurred while updating staff: " + ex.Message);
            }
            }
        #endregion

        #region DeleteStaffAsync
        public async Task<Result<StaffReqModel>> DeleteStaffAsync(int id)
        {
            try
            {
                Result<StaffReqModel> ReqModel = new Result<StaffReqModel>();
                if (id <= 0)
                {
                    ReqModel = Result<StaffReqModel>.ValidationError("Invalid Staff ID.");
                    goto Result;
                }
                var existingStaffQuery = "SELECT COUNT(1) FROM Staff WHERE Id = @Id AND del_flg = 0";
                if (existingStaffQuery is null)
                {
                    Result<StaffReqModel>.ValidationError("Staff not found.");
                }
                var deleteQuery = $@"UPDATE [dbo].[Staff]
                        SET del_flg = 1
                        WHERE Id = @Id AND del_flg = 0";
                var result = new StaffReqModel
                {
                    Id = id
                };
                var res = await Task.Run(()=> _dapperService.Execute(deleteQuery, result));
                if (res != 1)
                {
                    ReqModel = Result<StaffReqModel>.SystemError("Failed to delete staff.");
                    goto Result;
                }
                ReqModel = Result<StaffReqModel>.DeleteSuccess("Staff Deleted Successfully");
            Result:
                return ReqModel;
            }
            catch (Exception ex)
            {
                return Result<StaffReqModel>.SystemError("An error occurred while deleting staff: " + ex.Message);
            }
        }
        #endregion
    }
}
