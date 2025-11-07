using HCAS.Domain.Features.DoctorSchedule;
using HCAS.Domain.Features.Patient;
using MudBlazor;

namespace HCAS.WebApp.Components.Pages.Appointment;

public partial class CreateAppointment
{
    List<DoctorScheduleResponseModel> scheduleLst = new();
    List<PatientResModel> patientList = new();

    int? selectedPatientId = null;
    string selectedPatientName = string.Empty;
    int? selectedScheduleId = null;

    int page = 1;
    int pageSize = 10;
    int totalCount = 0;
    bool isCreating = false;

    private MudTable<DoctorScheduleResponseModel> mudTable;

    private bool IsCreateButtonEnabled => selectedPatientId is > 0 && selectedScheduleId is > 0;

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll(PatientList(), AvailableScheduleList());
    }

    async Task AvailableScheduleList()
    {
        var result = await _doctorScheduleService.GetDoctorSchedulesAsync();
        if (result.IsSuccess)
        {
            scheduleLst = result.Data.Items.ToList();
            totalCount = result.Data.TotalCount;
        }
        else
        {
            scheduleLst = new List<DoctorScheduleResponseModel>();
            totalCount = 0;
            Snackbar.Add("Failed to load doctor schedules", Severity.Error);
        }

        StateHasChanged();
    }

    async Task PatientList()
    {
        var result = await _patientService.GetAllPatient();
        if (result.IsSuccess)
        {
            patientList = result.Data;
        }
        else
        {
            patientList = new List<PatientResModel>();
            Snackbar.Add("Failed to load patients", Severity.Error);
        }

        StateHasChanged();
    }

    void PageChanged(int newPage)
    {
        page = newPage;
        _ = AvailableScheduleList();
    }

    void OnPatientSelectionChanged(object value)
    {
        selectedPatientId = value as int?;
        var selectedPatient = patientList.FirstOrDefault(p => p.Id == selectedPatientId);
        selectedPatientName = selectedPatient?.Name ?? string.Empty;
        StateHasChanged();
    }

    private void HandleRowClick(TableRowClickEventArgs<DoctorScheduleResponseModel> args)
    {
        var selectedRow = args.Item;
        selectedScheduleId = selectedRow.Id;
    }


    private string GetRowClass(DoctorScheduleResponseModel schedule, int rowNumber)
    {
        var classes = new List<string>();
        classes.Add("cursor-pointer");
        if (schedule.AvailableSlots <= 0)
        {
            classes.Add("disabled-row");
        }

        if (selectedScheduleId == schedule.Id)
        {
            classes.Add("selected-row");
        }

        return string.Join(" ", classes);
    }

    private async Task CreateAppointmentForPatient()
    {
        if (!IsCreateButtonEnabled)
        {
            Snackbar.Add("Please select both a patient and a schedule", Severity.Warning);
            return;
        }

        var selectedSchedule = scheduleLst.FirstOrDefault(s => s.Id == selectedScheduleId);
        if (selectedSchedule == null || selectedSchedule.AvailableSlots <= 0)
        {
            Snackbar.Add("The selected schedule is no longer available!", Severity.Error);
            await AvailableScheduleList();
            return;
        }

        isCreating = true;
        StateHasChanged();

        var result = await _appointmentService.CreateAppointment(selectedPatientId.Value, selectedScheduleId.Value);

        if (result.IsSuccess)
        {
            Snackbar.Add("Appointment created successfully!", Severity.Success);

            selectedPatientId = null;
            selectedPatientName = string.Empty;
            selectedScheduleId = null;

            await AvailableScheduleList();
            StateHasChanged();
        }
        else
        {
            Snackbar.Add($"Failed to create appointment: {result.Message}", Severity.Error);
        }

        isCreating = false;
        StateHasChanged();
    }
}