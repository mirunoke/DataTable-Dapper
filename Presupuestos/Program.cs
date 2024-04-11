using Presupuestos.Filters;
using Presupuestos.Infraestructure;
using Presupuestos.Services;

var builder = WebApplication.CreateBuilder(args);
// Agrega servicios al contenedor.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//Registra el filtro de excepciones personalizadas
builder.Services.AddScoped<GlobalExceptionFilter>();

//Se registra el filtro dentro del controlador
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//configuramos todos los servicios del middleware
builder.Services.AddTransient<ITiposCuentasServices, TiposCuentasServices>();
builder.Services.AddTransient<IUsuariosServices, ServiciosUsuarios>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();