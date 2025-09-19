using HCAS.WebApp.Components.Common;
using MudBlazor;

namespace HCAS.WebApp.Services;

public class InjectService
{
    private readonly IDialogService _dialogService;

    public InjectService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> ShowConfirmAsync(string message, string title = "Confirm", string buttonText = "Confirm", Color color = Color.Primary)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", message },
            { "ButtonText", buttonText },
            { "Color", color }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = await _dialogService.ShowAsync<ConfirmDialog>(title, parameters, options);
        var result = await dialog.Result;

        return !result.Canceled;
    }
}
