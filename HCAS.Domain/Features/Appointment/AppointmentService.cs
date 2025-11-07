using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Doctors;
using HCAS.Shared;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Domain.Features.Appointment;

public static class AppointmentQuery
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
                (DoctorId, PatientId, ScheduleId, AppointmentDate, AppointmentNumber, Status, del_flg) 
            OUTPUT INSERTED.Id
            VALUES (@DoctorId, @PatientId, @ScheduleId, @AppointmentDate, @AppointmentNumber, @Status, @DelFlag)";

    public const string UpdateStatus = @"
            UPDATE Appointments 
            SET Status = @NewStatus 
            WHERE Id = @AppointmentId";

    public const string Delete = @"
            DELETE FROM Appointments WHERE Id = @AppointmentId";
}

public class AppointmentReqModel
{
    //  public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int AppointmentNumber { get; set; }

    public string Status { get; set; } = null!;

    public bool DelFlg { get; set; }

    //   public virtual Doctor Doctor { get; set; } = null!;

    // public virtual Patient Patient { get; set; } = null!;

    //public virtual DoctorSchedule Schedule { get; set; } = null!;
}

public class AppointmentResModel
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int? DoctorId { get; set; }

    public int PatientId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public int AppointmentNumber { get; set; }

    public string Status { get; set; } = null!;

    public bool DelFlg { get; set; }

    // public virtual Doctor Doctor { get; set; } = null!;
}

public class AppointmentResponseModel
{
    public int Id { get; set; }
    public string DoctorName { get; set; }
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int AppointmentNumber { get; set; }
    public string Status { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int ScheduleId { get; set; }
}

public class AppointmentService
{
    private readonly DapperService _dapper;
    private readonly AppDbContext _appDbContext;

    public AppointmentService(DapperService dapperService, AppDbContext appDbContext)
    {
        _dapper = dapperService;
        _appDbContext = appDbContext;
    }

    public async Task<Result<IEnumerable<AppointmentResModel>>> GetAllAppointments()
    {
        try
        {
            var result = await _dapper.QueryAsync<AppointmentResModel>(AppointmentQuery.GetAll);
            var message = result.Any() ? "Success" : "No appointments found";
            return Result<IEnumerable<AppointmentResModel>>.Success(result, message);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<AppointmentResModel>>.SystemError(
                $"Error retrieving appointments: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<AppointmentResponseModel>>> GetAppointmentsAsync(
        int page = 1, int pageSize = 10, 
        string? doctorName = null, string? patientName = null)
    {
        try
        {
            var query = _appDbContext.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(a =>
                    a.Doctor.Name.Contains(doctorName));
            }
            
            if (!string.IsNullOrWhiteSpace(patientName))
            {
                query = query.Where(a =>
                    a.Patient.Name.Contains(patientName));
            }

            var total = await query.CountAsync();

            var appointments = await query
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentResponseModel
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    ScheduleId = a.ScheduleId,
                    DoctorName = a.Doctor.Name,
                    PatientName = a.Patient.Name,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentNumber = a.AppointmentNumber,
                    Status = a.Status
                })
                .ToListAsync();

            var pagedResult = new PagedResult<AppointmentResponseModel>
            {
                Items = appointments,
                TotalCount = total
            };

            return appointments.Any()
                ? Result<PagedResult<AppointmentResponseModel>>.Success(pagedResult)
                : Result<PagedResult<AppointmentResponseModel>>.NotFound("No appointments found");
        }
        catch (Exception ex)
        {
            return Result<PagedResult<AppointmentResponseModel>>.SystemError(ex.Message);
        }
    }
    
    public async Task<Result<AppointmentResModel>> GetAppointmentById(int id)
    {
        try
        {
            var appointment =
                await _dapper.QueryFirstOrDefaultAsync<AppointmentResModel>(AppointmentQuery.GetById,
                    new { Id = id });
            var message = appointment == null ? "No appointment found" : "Success";
            return Result<AppointmentResModel>.Success(appointment, message);
        }
        catch (Exception ex)
        {
            return Result<AppointmentResModel>.SystemError($"Error retrieving appointment: {ex.Message}");
        }
    }

    public async Task<Result<AppointmentResModel>> CreateAppointment(int patientId, int scheduleId)
    {
        try
        {
            if (patientId <= 0)
                return Result<AppointmentResModel>.ValidationError("Patient doesn't exist.");

            /*var scheduleQuery = @"
                SELECT Id, DoctorId, ScheduleDate, MaxPatients 
                FROM DoctorSchedules 
                WHERE Id = @ScheduleId AND del_flg = 0";*/

            /*var schedule =
                await _dapper.QueryFirstOrDefaultAsync<DoctorScheduleResModel>(scheduleQuery,
                    new { ScheduleId = scheduleId });*/

            var schedule = await _appDbContext.DoctorSchedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
                return Result<AppointmentResModel>.ValidationError("Invalid schedule");

            var appointmentCount = await _dapper.QueryFirstOrDefaultAsync<int>(AppointmentQuery.CountBySchedule,
                new { ScheduleId = scheduleId });

            if (appointmentCount >= schedule.MaxPatients)
                return Result<AppointmentResModel>.ValidationError("This schedule is already full");

            int appointmentNumber = appointmentCount + 1;

            var parameters = new
            {
                DoctorId = schedule.DoctorId,
                PatientId = patientId,
                ScheduleId = scheduleId,
                AppointmentDate = schedule.ScheduleDate,
                AppointmentNumber = appointmentNumber,
                Status = "Pending",
                DelFlag = false
            };

            var newId = await _dapper.QueryFirstOrDefaultAsync<int>(AppointmentQuery.Insert, parameters);

            var result = new AppointmentResModel
            {
                Id = newId,
                DoctorId = schedule.DoctorId,
                PatientId = patientId,
                ScheduleId = scheduleId,
                AppointmentDate = schedule.ScheduleDate,
                AppointmentNumber = appointmentNumber,
                Status = "Pending",
                DelFlg = false
            };

            return Result<AppointmentResModel>.Success(result, "Appointment created successfully");
        }
        catch (Exception ex)
        {
            return Result<AppointmentResModel>.SystemError($"Error creating appointment: {ex.Message}");
        }
    }

    public async Task<Result<AppointmentResModel>> UpdateAppointment(int appointmentId, string newStatus)
    {
        try
        {
            if (appointmentId <= 0)
                return Result<AppointmentResModel>.ValidationError("Invalid AppointmentId");

            if (string.IsNullOrWhiteSpace(newStatus))
                return Result<AppointmentResModel>.ValidationError("Invalid status");

            var res = await _dapper.ExecuteAsync(AppointmentQuery.UpdateStatus,
                new { AppointmentId = appointmentId, NewStatus = newStatus });

            if (res != 1)
                return Result<AppointmentResModel>.SystemError("Failed to update appointment");

            var result = new AppointmentResModel
            {
                Id = appointmentId,
                Status = newStatus
            };

            return Result<AppointmentResModel>.Success(result, "Appointment updated successfully");
        }
        catch (Exception ex)
        {
            return Result<AppointmentResModel>.SystemError($"Error updating appointment: {ex.Message}");
        }
    }

    public async Task<Result<AppointmentResModel>> DeleteAppointment(int appointmentId)
    {
        try
        {
            var res = await _dapper.ExecuteAsync(AppointmentQuery.Delete, new { AppointmentId = appointmentId });

            if (res != 1)
                return Result<AppointmentResModel>.ValidationError("Failed to delete appointment");

            return Result<AppointmentResModel>.Success(new AppointmentResModel { Id = appointmentId },
                "Appointment deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<AppointmentResModel>.SystemError($"Error deleting appointment: {ex.Message}");
        }
    }
}

public enum EnumAppointmentStatus
{
    Pending,
    Complete,
    Cancelled
}