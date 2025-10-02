using HCAS.Domain.Features.DoctorSchedule;
using HCAS.Domain.Features.Model.Appoinment;
using HCAS.Domain.Models.Appoinment;
using HCAS.Domain.Models.DoctorSchedule;
using HCAS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCAS.Domain.Features.Appoinment
{
    public static class AppoinmentQuery
    {
        public const string GetAll = @"
            SELECT 
                a.Id, 
                a.DoctorId,
                d.Name AS DoctorName, 
                a.PatientId,
                p.Name AS PatientName, 
                a.ScheduleId,
                a.AppointmentDate, 
                a.AppointmentNumber,
                a.Status
            FROM Appointments a
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Patients p ON a.PatientId = p.Id";

        public const string GetById = @"
            SELECT 
                a.Id, 
                a.DoctorId,
                d.Name AS DoctorName, 
                a.PatientId,
                p.Name AS PatientName, 
                a.ScheduleId,
                a.AppointmentDate, 
                a.AppointmentNumber,
                a.Status
            FROM Appointments a
            INNER JOIN Doctors d ON a.DoctorId = d.Id
            INNER JOIN Patients p ON a.PatientId = p.Id
            WHERE a.Id = @Id";

        public const string CountBySchedule = @"
            SELECT COUNT(*) 
            FROM Appointments 
            WHERE ScheduleId = @ScheduleId AND Status <> 'Cancelled'";

        public const string Insert = @"
            INSERT INTO Appointments 
                (DoctorId, PatientId, ScheduleId, AppointmentDate, AppointmentNumber, Status) 
            OUTPUT INSERTED.Id
            VALUES (@DoctorId, @PatientId, @ScheduleId, @AppointmentDate, @AppointmentNumber, @Status)";

        public const string UpdateStatus = @"
            UPDATE Appointments 
            SET Status = @NewStatus 
            WHERE Id = @AppointmentId";

        public const string Delete = @"
            DELETE FROM Appointments WHERE Id = @AppointmentId";
    }

    public class AppoinmentService
    {
        private readonly DapperService _dapper;

        public AppoinmentService(DapperService dapperService)
        {
            _dapper = dapperService;
        }

        public async Task<Result<IEnumerable<AppoinmentResModel>>> GetAllAppointments()
        {
            try
            {
                var result = await _dapper.QueryAsync<AppoinmentResModel>(AppoinmentQuery.GetAll);
                var message = result.Any() ? "Success" : "No appointments found";
                return Result<IEnumerable<AppoinmentResModel>>.Success(result, message);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AppoinmentResModel>>.SystemError($"Error retrieving appointments: {ex.Message}");
            }
        }

        public async Task<Result<AppoinmentResModel>> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _dapper.QueryFirstOrDefaultAsync<AppoinmentResModel>(AppoinmentQuery.GetById, new { Id = id });
                var message = appointment == null ? "No appointment found" : "Success";
                return Result<AppoinmentResModel>.Success(appointment, message);
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError($"Error retrieving appointment: {ex.Message}");
            }
        }

        public async Task<Result<AppoinmentResModel>> CreateAppointment(int patientId, int scheduleId)
        {
            try
            {
                if (patientId <= 0)
                    return Result<AppoinmentResModel>.ValidationError("Invalid PatientId");

                var scheduleQuery = @"
                    SELECT Id, DoctorId, ScheduleDate, MaxPatients 
                    FROM DoctorSchedules 
                    WHERE Id = @ScheduleId AND del_flg = 0";

                var schedule = await _dapper.QueryFirstOrDefaultAsync<DoctorScheduleResModel>(scheduleQuery, new { ScheduleId = scheduleId });

                if (schedule == null)
                    return Result<AppoinmentResModel>.ValidationError("Invalid schedule");

                var appointmentCount = await _dapper.QueryFirstOrDefaultAsync<int>(AppoinmentQuery.CountBySchedule, new { ScheduleId = scheduleId });

                if (appointmentCount >= schedule.MaxPatients)
                    return Result<AppoinmentResModel>.ValidationError("This schedule is already full");

                int appointmentNumber = appointmentCount + 1;

                var parameters = new
                {
                    DoctorId = schedule.DoctorId,
                    PatientId = patientId,
                    ScheduleId = scheduleId,
                    AppointmentDate = schedule.ScheduleDate,
                    AppointmentNumber = appointmentNumber,
                    Status = "Pending"
                };

                var newId = await _dapper.QueryFirstOrDefaultAsync<int>(AppoinmentQuery.Insert, parameters);

                var result = new AppoinmentResModel
                {
                    Id = newId,
                    DoctorId = schedule.DoctorId,
                    PatientId = patientId,
                    ScheduleId = scheduleId,
                    AppointmentDate = schedule.ScheduleDate,
                    AppointmentNumber = appointmentNumber,
                    Status = "Pending"
                };

                return Result<AppoinmentResModel>.Success(result, "Appointment created successfully");
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError($"Error creating appointment: {ex.Message}");
            }
        }

        public async Task<Result<AppoinmentResModel>> UpdateAppointment(int appointmentId, string newStatus)
        {
            try
            {
                if (appointmentId <= 0)
                    return Result<AppoinmentResModel>.ValidationError("Invalid AppointmentId");

                if (string.IsNullOrWhiteSpace(newStatus))
                    return Result<AppoinmentResModel>.ValidationError("Invalid status");

                var res = await _dapper.ExecuteAsync(AppoinmentQuery.UpdateStatus, new { AppointmentId = appointmentId, NewStatus = newStatus });

                if (res != 1)
                    return Result<AppoinmentResModel>.SystemError("Failed to update appointment");

                var result = new AppoinmentResModel
                {
                    Id = appointmentId,
                    Status = newStatus
                };

                return Result<AppoinmentResModel>.Success(result, "Appointment updated successfully");
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError($"Error updating appointment: {ex.Message}");
            }
        }

        public async Task<Result<AppoinmentResModel>> DeleteAppointment(int appointmentId)
        {
            try
            {
                var res = await _dapper.ExecuteAsync(AppoinmentQuery.Delete, new { AppointmentId = appointmentId });

                if (res != 1)
                    return Result<AppoinmentResModel>.ValidationError("Failed to delete appointment");

                return Result<AppoinmentResModel>.Success(new AppoinmentResModel { Id = appointmentId }, "Appointment deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<AppoinmentResModel>.SystemError($"Error deleting appointment: {ex.Message}");
            }
        }
    }
}
