
using System.Text.Json.Serialization;
using BlazorComponentBus;
using Foundry.Helpers;
using FoundryBlazor;
using FoundryBlazor.Shape;
using FoundryBlazor.Shared;
using FoundryBlazor.Solutions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Units;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using Radzen;


using Visio2023Foundry.Model;
using Visio2023Foundry.Simulation;
using Visio2023Foundry.Targets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


var envConfig = new EnvConfig("./.env");
builder.Services.AddSingleton<IEnvConfig>(provider => envConfig);

builder.Services.AddScoped<ComponentBus>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddScoped<IToast, Toast>();


builder.Services.AddScoped<IPanZoomService, PanZoomService>();
builder.Services.AddScoped<IHitTestService, HitTestService>();
builder.Services.AddScoped<ISelectionService, SelectionService>();
builder.Services.AddScoped<IPageManagement, PageManagementService>();
builder.Services.AddScoped<IStageManagement, StageManagementService>();
builder.Services.AddScoped<ICommand, CommandService>();


builder.Services.AddScoped<IDrawing, NDC_Drawing2D>();
builder.Services.AddScoped<IArena, NDC_Arena3D>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();


builder.Services.AddScoped<IWorkspace, FoWorkspace>();
builder.Services.AddScoped<IFoundryService, FoundryService>();
builder.Services.AddScoped<IUnitSystem, UnitSystem>();
builder.Services.AddScoped<ICodeDisplayService, CodeDisplayService>();



builder.WebHost.UseStaticWebAssets();

// builder.Services.AddControllers().AddJsonOptions(options =>
// {
//     var ser = options.JsonSerializerOptions;
//     ser.IgnoreReadOnlyFields = true;
//     ser.IncludeFields = true;
//     ser.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
// });

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(38);
    hubOptions.EnableDetailedErrors = true;
    hubOptions.StreamBufferCapacity = 1;
    hubOptions.MaximumReceiveMessageSize = null;
})
.AddJsonProtocol((options) =>
{
    var ser = options.PayloadSerializerOptions;
    //ser.IncludeFields = false;
    ser.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}).AddMessagePackProtocol();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            // .WithOrigins(new[] { "http://localhost:8080", "http://localhost:8081" })
            //.AllowCredentials()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI 
// at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

"All Scopes are added".WriteNote();

var app = builder.Build();

app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI();

envConfig.EstablishAllFolders().ForEach(folder =>
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, folder)),
        RequestPath = new PathString($"/{folder}"),
        EnableDirectoryBrowsing = true,
        StaticFileOptions = {
            ContentTypeProvider = ContentTypeHelpers.MIMETypeProvider()
        }
    });
});


app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "storage")),
    RequestPath = new PathString($"/storage"),
    EnableDirectoryBrowsing = true,
    StaticFileOptions = {
        ContentTypeProvider = ContentTypeHelpers.MIMETypeProvider()
    }
});


//app.UseFileServer(true);

var serviceScope = ((IApplicationBuilder)app).ApplicationServices
   .GetRequiredService<IServiceScopeFactory>()
   .CreateScope();

var unitsystem = serviceScope.ServiceProvider.GetService<IUnitSystem>();
unitsystem?.Apply(UnitSystemType.MKS);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();


app.MapBlazorHub();
app.MapControllers();

app.MapHub<DrawingSyncHub>("/DrawingSyncHub");

app.MapFallbackToPage("/_Host");


//var Workspace = serviceScope?.ServiceProvider.GetService<IWorkspace>();

//knowledge base

// Workspace!.EstablishWorkbook<Playground>().Name = "playground";
// Workspace.EstablishWorkbook<Stencil>().Name = "stencil";
// Workspace.EstablishWorkbook<TargetManager>().Name = "network";
// Workspace.EstablishWorkbook<BoidManager>().Name = "boid"; 
// Workspace.EstablishWorkbook<Composition>().Name = "composition"; 
// Workspace.EstablishWorkbook<MoSimulation>().Name = "simulation"; 
// Workspace.EstablishWorkbook<Process>().Name = "process"; 
// Workspace.EstablishWorkbook<SignalRDemo>().Name = "SignalR"; 

app.Run();
