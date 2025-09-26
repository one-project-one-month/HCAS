using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using HCAS.Domain.Models.Patient;
using HCAS.Shared;

namespace HCAS.Domain.Features.Patient
{
    public class PatientService
    {
        private readonly DapperService _dapperService;

        public PatientService(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        #region GetAllPatient
        public async Task<Result<List<PatientResModel>>> GetAllPatient()
        {
            try
            {
                Result<List<PatientResModel>> model = new Result<List<PatientResModel>>();
                string query = "SELECT * FROM Patients where del_flg = 0";

                var patientList = await Task.Run(() => _dapperService.Query<PatientResModel>(query));

                if( patientList.Count < 0 )
                {
                    model = Result<List<PatientResModel>>.SystemError("Something went wrong");
                    goto Result;
                }

                string message = (patientList.Count == 0) ? "No Data Found" : "Success";
                model = Result<List<PatientResModel>>.Success(patientList, message);

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<List<PatientResModel>>.SystemError(ex.Message);    
            }
        }
        #endregion
       
        #region GetById
        public async Task<Result<PatientResModel>> GetById(int id)
        {
            try
            {
                Result <PatientResModel> model = new Result<PatientResModel>();

                string query = @"";

                var patient = await Task.Run(() => _dapperService.Query<PatientResModel>(query, new
                {
                    @Id = id,
                }));

                if(patient.Count < 0 )
                {
                    model = Result<PatientResModel>.SystemError("Something went wrong");
                    goto Result;
                }

                string message = (patient.Count == 0) ? "No Data Found" : "Success";
                model = Result<PatientResModel>.Success(patient[0], message);

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<PatientResModel>.SystemError (ex.Message);
            }
        }
        #endregion
      
        #region UpdatePatient
        public async Task<Result<PatientResModel>> UpdatePatient(PatientReqModel patientReqModel,int id)
        {
            try
            {
                Result<PatientResModel> model = new Result<PatientResModel> ();

                

                string query = "SELECT * FROM Patients WHERE Id = @Id AND del_flg = 0";

                var patient = await Task.Run(() => _dapperService.Query<PatientResModel>(query, new
                {
                    @Id = id ,
                }));

                if (patient.Count < 0 )
                {
                    model = Result<PatientResModel>.SystemError("Something went wrong");
                    goto Result;
                }

                string UpdateQuery = @"
UPDATE [dbo].[Patients]
   SET [Name] = @Name
      ,[DateOfBirth] = @DateOfBirth
      ,[Gender] = @Gender
      ,[Phone] = @Phone
      ,[Email] = @Email
 WHERE Id = @Id";

                int result = await Task.Run(() => _dapperService.Execute(UpdateQuery, new
                {
                    Id = id,
                    Name = patientReqModel.Name,
                    DateOfBirth = patientReqModel.DateOfBirth,
                    Gender = patientReqModel.Gender,
                    Phone = patientReqModel.Phone,
                    Email = patientReqModel.Email
                }));
                if (result < 0)
                {
                    model = Result<PatientResModel>.SystemError("Something went wrong");
                    goto Result;
                }

                var res = new PatientResModel();

                res.Name = patientReqModel.Name;
                res.DateOfBirth = patientReqModel.DateOfBirth;
                res.Gender = patientReqModel.Gender;
                res.Phone = patientReqModel.Phone;
                res.Email = patientReqModel.Email;

                model = Result<PatientResModel>.Success(res,"Update Success");

            Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<PatientResModel>.SystemError(ex.Message);
            }
        }
        #endregion
     
        #region CreatePatient
        public async Task<Result<PatientReqModel>> RegisterPatient(PatientReqModel reqModel)
        {
            try
            {
                Result <PatientReqModel> model = new Result<PatientReqModel>();
                if (string.IsNullOrEmpty(reqModel.Name))
                {
                    model = Result<PatientReqModel>.ValidationError("Name is required");
                    goto Result;
                }

                string query = @"INSERT INTO [dbo].[Patients]
           ([Name]
           ,[DateOfBirth]
           ,[Gender]
           ,[Phone]
           ,[Email]
           ,[del_flg])
     VALUES
           ( @Name
           ,@DateOfBirth
           ,@Gender
           ,@Phone
           ,@Email
           ,0)";

                var result = await Task.Run(() => _dapperService.Execute(query, reqModel));

                if ( result != 1)
                {
                    model = Result<PatientReqModel>.SystemError("Something went wong");
                    goto Result;
                }

                model = Result<PatientReqModel>.Success(reqModel, "Patient Create Success");

            Result:
                return model;
            }
            catch (Exception ex)
            {

                return Result<PatientReqModel>.SystemError(ex.ToString());
            }
            
        }
        #endregion
      
        #region DeletePatient
        public async Task<Result<PatientReqModel>> DeletePatient(int  id)
        {
            try
            {
                Result<PatientReqModel> model = new Result<PatientReqModel>();

                string findQuery = @"SELECT * From Patients WHERE Id = @Id AND del_flg = 0";

                var patient =await Task.Run(() => _dapperService.Query<PatientReqModel>(findQuery, new
                {
                    Id = id
                }));

                if (patient.Count < 0)
                {
                    model = Result<PatientReqModel>.SystemError("Something Went Wrong");
                    goto Result;
                }
                if(patient.Count == 0)
                {
                    model = Result<PatientReqModel>.Success(new PatientReqModel(),"No Data Found");
                }

                

                string delQuery = @"UPDATE [dbo].[Patients]
                                  SET [del_flg] = 1
                                    WHERE Id = @Id";

                int result = await Task.Run(() => _dapperService.Execute(delQuery, new  { 
                    Id = id
                }));

                if(result < 0)
                {
                    model = Result<PatientReqModel>.SystemError("Something Went Wrong");
                }

                model = Result<PatientReqModel>.Success(patient[0], "Success");
                Result:
                return model;
            }
            catch (Exception ex)
            {
                return Result<PatientReqModel>.SystemError(ex.Message);
            }
        }
    }

    #endregion
}
