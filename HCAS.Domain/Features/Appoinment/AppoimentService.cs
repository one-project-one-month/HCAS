using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Model.Appoinment;
using HCAS.Domain.Features.Model.DoctorSchedule;
using HCAS.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Result<IEnumerable<AppoinmentResModel>>> GetAllAppoinment()
        {
            try
            {
                var query = @"
                    SELECT 
                        a.Id, 
                        d.Name AS DoctorName, 
                        p.Name AS PatientName, 
                        a.AppointmentDate, 
                        a.AppointmentNumber,
                        a.Status
                    FROM Appointments a
                    INNER JOIN Doctors d ON a.DoctorId = d.Id
                    INNER JOIN Patients p ON a.PatientId = p.Id";

                var appointments = await _dapperService.QueryAsync<AppoinmentResModel>(query);

                string message = (appointments == null || !appointments.Any())
                    ? "No Schedule Found"
                    : "Success";

                return Result<IEnumerable<AppoinmentResModel>>.Success(appointments ?? new List<AppoinmentResModel>(), message);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AppoinmentResModel>>.SystemError(ex.Message);
            }
        }

        public async Task<Result<AppoinmentResModel>> GetAppoinmentById(int id)
        {
            try
            {
                var query = @"
                    SELECT 
                        a.Id, 
                        d.Name AS DoctorName, 
                        p.Name AS PatientName, 
                        a.AppointmentDate, 
                        a.AppointmentNumber,
                        a.Status
                    FROM Appointments a
                    INNER JOIN Doctors d ON a.DoctorId = d.Id
                    INNER JOIN Patients p ON a.PatientId = p.Id
                    WHERE a.Id = @Id";
                var appointment = await _dapperService.QueryFirstOrDefaultAsync<AppoinmentResModel>(query, new { Id = id });
                string message = (appointment == null)
                    ? "No Schedule Found"
                    : "Success";
                return Result<AppoinmentResModel>.Success(appointment, message);
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError(ex.Message);
            }
        }

        public async Task<Result<AppoinmentResModel>> CreateAppointment(int patientId, int scheduleId)
        {
            try
            {
                // Fetch the schedule
                var scheduleQuery = @"
            SELECT DoctorId, ScheduleDate, MaxPatients 
            FROM DoctorSchedules 
            WHERE Id = @ScheduleId";

                var schedule = await _dapperService.QueryFirstOrDefaultAsync<DoctorScheduleResModel>(
                    scheduleQuery, new { ScheduleId = scheduleId });

                if (schedule == null)
                {
                    return Result<AppoinmentResModel>.ValidationError("Invalid schedule selected.");
                }

                // Count existing appointments
                var countQuery = @"
            SELECT COUNT(*) 
            FROM Appointments 
            WHERE ScheduleId = @ScheduleId AND Status <> 'Cancelled'";

                int appointmentCount = await _dapperService.QueryFirstOrDefaultAsync<int>(
                    countQuery, new { ScheduleId = scheduleId });

                if (appointmentCount >= schedule.MaxPatients)
                {
                    return Result<AppoinmentResModel>.ValidationError("This schedule is already full.");
                }

                int appointmentNumber = appointmentCount + 1;

                // Insert new appointment
                var insertQuery = @"
            INSERT INTO Appointments 
                (DoctorId, PatientId, ScheduleId, AppointmentDate, AppointmentNumber, Status) 
            OUTPUT INSERTED.Id
            VALUES (@DoctorId, @PatientId, @ScheduleId, @AppointmentDate, @AppointmentNumber, @Status)";

                var parameters = new
                {
                    DoctorId = schedule.DoctorId,
                    PatientId = patientId,
                    ScheduleId = scheduleId,
                    AppointmentDate = schedule.ScheduleDate,
                    AppointmentNumber = appointmentNumber,
                    Status = "Pending"
                };

                var newAppointmentId = await _dapperService.QueryFirstOrDefaultAsync<int>(insertQuery, parameters);

                var result = new AppoinmentResModel
                {
                    Id = newAppointmentId,
                    DoctorId = schedule.DoctorId,
                    PatientId = patientId,
                    ScheduleId = scheduleId,
                    AppointmentDate = schedule.ScheduleDate,
                    AppointmentNumber = appointmentNumber,
                    Status = "pending"
                };

                return Result<AppoinmentResModel>.Success(result, "Appointment created successfully.");
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError(ex.Message);
            }
        }


        public async Task<Result<AppoinmentResModel>> UpdateAppoinment(int appointmentId, string newStatus)
        {
            try
            {
                string query = string.Empty;

                if (newStatus == "Cancelled")
                {
                    query = "delete from Appointments where Id = @AppointmentId";
                }
                if (newStatus == "Complete")
                {
                    query = "UPDATE Appointments SET Status = @NewStatus WHERE Id = @AppointmentId";
                } 

                var result  = _dapperService.Execute(query, new { AppointmentId = appointmentId, NewStatus = newStatus });

                return Result<AppoinmentResModel>.Success(new AppoinmentResModel(), "Update functionality not implemented yet.");
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError(ex.Message);
            }
        }

    }
}

