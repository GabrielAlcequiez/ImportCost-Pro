using FluentValidation;
using ImportCostPro.BusinessLogic.Services.Implementations;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.BusinessLogic.Validators.Country;
using ImportCostPro.Database;
using ImportCostPro.Database.Repositories.Implementations;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Ya se encarga de instanciar todos los repos internamente
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// lo mismo acá, similar a la cuestión del db context
builder.Services.AddValidatorsFromAssemblyContaining<CreateCountryDtoValidator>(ServiceLifetime.Transient);
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IImporterService, ImporterService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ITaxConfigurationService, TaxConfigurationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
