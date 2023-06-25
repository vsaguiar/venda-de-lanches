using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Registrando o serviço
        string connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

        // Serviço do Identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 1;
        });

        builder.Services.AddTransient<ILancheRepository, LancheRepository>();
        builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
        builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

        // Configurando Session
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }


        void CriarPerfisUsuarios(WebApplication app)
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopedFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
                // Cria os perfis 
                service.SeedRoles();

                // Cria os usuários e atribui ao perfil
                service.SeedUsers();
            }
        }
        CriarPerfisUsuarios(app);


        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Ativando o Session
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "categoriaFiltro",
            pattern: "Lanche/{action}/{categoria?}",
            defaults: new { Controller = "Lanche", action = "List" });


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
