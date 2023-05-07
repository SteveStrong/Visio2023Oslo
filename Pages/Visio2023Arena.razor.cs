using BlazorComponentBus;
using FoundryBlazor.Extensions;
using FoundryBlazor.PubSub;
using FoundryBlazor.Shared;
using FoundryBlazor.Solutions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Visio2023Foundry.Model;
using Visio2023Foundry.Services;

namespace Visio2023Foundry.Pages;

public partial class Visio2023ArenaPage : ComponentBase
{
    [Inject] public NavigationManager? Navigation { get; set; }
    [Inject] protected IJSRuntime? JsRuntime { get; set; }
    [Inject] private ComponentBus? PubSub { get; set; }
    [Inject] private IToast? Toast { get; set; }
    [Inject] public IWorkspace? Workspace { get; init; }
    [Inject] public IRestAPIServiceDTAR? RestAPI { get; init; }

    [Parameter]
    public string? LoadWorkPiece { get; set; }

    protected override void OnInitialized()
    {
        Workspace?.SetBaseUrl(Navigation?.BaseUri ?? "");
    }

    protected override async Task OnInitializedAsync()
    {
        if (Workspace != null)
        {
            Workspace.ClearAllWorkbook();
            Workspace.EstablishWorkbook<Gamer>();

            var url = RestAPI?.GetServerUrl() ?? "";
            Workspace.CreateCommands(Workspace, JsRuntime!, Navigation!, url);

            Workspace.CreateMenus(Workspace, JsRuntime!, Navigation!);



            var defaultHubURI = Navigation!.ToAbsoluteUri("/DrawingSyncHub").ToString();
            await Workspace.InitializedAsync(defaultHubURI!);

            //Toast?.Info(LoadWorkPiece ?? "No WorkPiece");
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
           // PubSub!.SubscribeTo<RefreshUIEvent>(OnRefreshUIEvent);
            PubSub!.SubscribeTo<ViewStyle>(OnViewStyleChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnRefreshUIEvent(RefreshUIEvent e)
    {
        InvokeAsync(StateHasChanged);
         $"Visio2023Page OnRefreshUIEvent StateHasChanged {e.note}".WriteInfo();
    }

    private void OnViewStyleChanged(ViewStyle e)
    {
        Workspace?.SetViewStyle(e);
        InvokeAsync(StateHasChanged);
    }
}
