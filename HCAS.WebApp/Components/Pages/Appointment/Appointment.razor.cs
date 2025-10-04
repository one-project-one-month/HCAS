using HCAS.Domain.Features.Doctors;
using HCAS.Domain.Models.Appointment;
using HCAS.Domain.Models.Doctors;
using HCAS.WebApp.Components.Pages.Doctor;
using MudBlazor;

namespace HCAS.WebApp.Components.Pages.Appointment;

public partial class Appointment
{
    List<AppointmentResponseModel> appointmentLst = new();
    List<SpecializationModel> specializations = new();

    int page = 1;
    int pageSize = 10;
    int totalCount = 0;
    string doctorName = string.Empty;
    string patientName = string.Empty;
    int? selectedSpecialization = null;

    private bool isNavigating = false;

    private async Task NavigateToCreateAppointment()
    {
        try
        {
            isNavigating = true;
            StateHasChanged();

            await Task.Delay(100);

            Navigation.NavigateTo("/create-appointment");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error navigating: {ex.Message}", Severity.Error);
        }
        finally
        {
            isNavigating = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // await LoadSpecializations();
        await AppointmentList();
    }

    async Task AppointmentList()
    {
        var result = await _appointmentService.GetAppointmentsAsync
            (page, pageSize, doctorName, patientName);
        if (result.IsSuccess)
        {
            appointmentLst = result.Data.Items.ToList();
            totalCount = result.Data.TotalCount;
        }
        else
        {
            appointmentLst = new List<AppointmentResponseModel>();
            totalCount = 0;
        }

        StateHasChanged();
    }

    /*async Task LoadSpecializations()
    {
        var result = await _appointmentService.GetSpecializationsAsync();
        specializations = result?.ToList() ?? new();
    }*/

    void PageChanged(int newPage)
    {
        page = newPage;
        _ = AppointmentList();
    }

    void OnDoctorNameSearchChanged(object value)
    {
        doctorName = value?.ToString() ?? string.Empty;
        page = 1;
        _ = AppointmentList();
    }

    void OnPatientNameSearchChanged(object value)
    {
        patientName = value?.ToString() ?? string.Empty;
        page = 1;
        _ = AppointmentList();
    }
}