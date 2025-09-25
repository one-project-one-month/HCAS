using HCAS.Domain.Features.Model.Appoinment;
using HCAS.Domain.Features.Model.DoctorSchedule;
using HCAS.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Appoinment
{
    public class AppoimentService
    {
        private readonly DapperService _dapperService;

        public AppoimentService(DapperService dapperService)
        {
            _dapperService = dapperService;
        }

        //public async Task<Result<IEnumerable<AppoinmentResModel>>> GetAllAppoinment()
        //{
        //    try
        //    {
        //        Result<IEnumerable<AppoinmentResModel>> model = new Result<IEnumerable<AppoinmentResModel>>();

        //        var query = @"
        //        SELECT 
        //            a.Id, 
        //            d.Name AS DoctorName, 
        //            p.Name AS PatientName, 
        //            a.AppointmentDate, 
        //            a.AppointmentNumber,
        //            a.Status
        //        FROM Appointments a
        //        INNER JOIN Doctors d ON a.DoctorId = d.Id
        //        INNER JOIN Patients p ON a.PatientId = p.Id";

        //        var appointment = _dapperService.QueryAsync<AppoinmentResModel>(query);

        //         string message = (appointment is null) ? "No Schedule Found" : "Success";

        //         model =  Result<AppoinmentResModel>.Success(appointment, "");
        //        return model;
        //    }
        //    catch (Exception ex) 
        //    {
        //        return Result<IEnumerable<AppoinmentResModel>>.SystemError(ex.Message);
        //    }
        //}
    }
}
