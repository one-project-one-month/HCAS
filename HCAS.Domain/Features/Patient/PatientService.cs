using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Models.Patient;
using HCAS.Shared;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Domain.Features.Patient;

public static class PatientQuery
{
    public const string GetAll = "SELECT * FROM Patients WHERE del_flg = 0";

    public const string GetById = "SELECT * FROM Patients WHERE Id = @Id AND del_flg = 0";

    public const string Insert = @"
        INSERT INTO Patients (Name, DateOfBirth, Gender, Phone, Email, del_flg)
        VALUES (@Name, @DateOfBirth, @Gender, @Phone, @Email, 0)";

    public const string Update = @"
        UPDATE Patients
        SET Name = @Name,
            DateOfBirth = @DateOfBirth,
            Gender = @Gender,
            Phone = @Phone,
            Email = @Email
        WHERE Id = @Id";

    public const string SoftDelete = "UPDATE Patients SET del_flg = 1 WHERE Id = @Id";
}

public class PatientService
{
    private readonly DapperService _dapper;
    private readonly AppDbContext _appDbContext;

    public PatientService(DapperService dapperService, AppDbContext appDbContext)
    {
        _dapper = dapperService;
        _appDbContext = appDbContext;
    }

    public async Task<Result<List<PatientResModel>>> GetAllPatient()
    {
        try
        {
            // var result = await _dapper.QueryAsync<PatientResModel>(PatientQuery.GetAll);
            var result = await _appDbContext.Patients
                .Select(p => new PatientResModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Phone = p.Phone
                }).ToListAsync();
            
            var data = result.ToList();

            var message = data.Count == 0 ? "No data found" : "Success";
            return Result<List<PatientResModel>>.Success(data, message);
        }
        catch (Exception ex)
        {
            return Result<List<PatientResModel>>.SystemError($"Error retrieving patients: {ex.Message}");
        }
    }

    public async Task<Result<PatientResModel>> GetById(int id)
    {
        try
        {
            var patient =
                await _dapper.QueryFirstOrDefaultAsync<PatientResModel>(PatientQuery.GetById, new { Id = id });

            if (patient is null)
                return Result<PatientResModel>.NotFound("Patient not found.");

            return Result<PatientResModel>.Success(patient, "Success");
        }
        catch (Exception ex)
        {
            return Result<PatientResModel>.SystemError($"Error retrieving patient: {ex.Message}");
        }
    }

    public async Task<Result<PatientResModel>> UpdatePatient(PatientReqModel dto, int id)
    {
        try
        {
            var exists = await _dapper.QueryFirstOrDefaultAsync<PatientResModel>(PatientQuery.GetById, new { Id = id });

            if (exists is null)
                return Result<PatientResModel>.NotFound("Patient not found.");

            var res = await _dapper.ExecuteAsync(PatientQuery.Update, new
            {
                Id = id,
                dto.Name,
                dto.DateOfBirth,
                dto.Gender,
                dto.Phone,
                dto.Email
            });

            if (res != 1)
                return Result<PatientResModel>.SystemError("Failed to update patient.");

            var updated = new PatientResModel
            {
                Id = id,
                Name = dto.Name,
                DateOfBirth = (DateTime)dto.DateOfBirth,
                Gender = dto.Gender,
                Phone = dto.Phone,
                Email = dto.Email
            };

            return Result<PatientResModel>.Success(updated, "Update successful");
        }
        catch (Exception ex)
        {
            return Result<PatientResModel>.SystemError($"Error updating patient: {ex.Message}");
        }
    }

    public async Task<Result<PatientReqModel>> RegisterPatient(PatientReqModel dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<PatientReqModel>.ValidationError("Name is required.");

            var res = await _dapper.ExecuteAsync(PatientQuery.Insert, dto);

            if (res != 1)
                return Result<PatientReqModel>.SystemError("Failed to create patient.");

            return Result<PatientReqModel>.Success(dto, "Patient created successfully");
        }
        catch (Exception ex)
        {
            return Result<PatientReqModel>.SystemError($"Error creating patient: {ex.Message}");
        }
    }

    public async Task<Result<PatientReqModel>> DeletePatient(int id)
    {
        try
        {
            var patient =
                await _dapper.QueryFirstOrDefaultAsync<PatientReqModel>(PatientQuery.GetById, new { Id = id });

            if (patient is null)
                return Result<PatientReqModel>.NotFound("Patient not found.");

            var res = await _dapper.ExecuteAsync(PatientQuery.SoftDelete, new { Id = id });

            if (res != 1)
                return Result<PatientReqModel>.SystemError("Failed to delete patient.");

            return Result<PatientReqModel>.Success(patient, "Patient deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<PatientReqModel>.SystemError($"Error deleting patient: {ex.Message}");
        }
    }
}