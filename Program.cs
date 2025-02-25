using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models; // Utilisation de votre mod�le Client
using MonParfumApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Connexion � la base de donn�es
var connectionString = builder.Configuration.GetConnectionString("parfumerie");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Configuration d'Identity avec le mod�le Client
builder.Services.AddIdentity<Client, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Configuration des cookies d'authentification
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Page de connexion
    options.AccessDeniedPath = "/Home/AccessDenied"; // Page d'acc�s refus�
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Assurez-vous que cela ne cr�e pas de boucle de redirection
        if (context.Request.Path == "/Home/AccessDenied" || context.Response.StatusCode == 403)
        {
            return Task.CompletedTask; // �viter une nouvelle redirection
        }

        context.Response.Redirect($"/Home/AccessDenied?returnUrl={context.Request.Path}");
        return Task.CompletedTask;
    };
});

// 4. Ajout des services personnalis�s
builder.Services.AddScoped<ParfumService>();
builder.Services.AddScoped<CategorieService>();
builder.Services.AddScoped<PanierService>();
builder.Services.AddScoped<CommandeService>();

// 5. Ajout des contr�leurs, vues, et configuration des routes
builder.Services.AddControllersWithViews();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Construire l'application
var app = builder.Build();

// 6. Initialisation des r�les et de l'administrateur par d�faut
await InitializeRolesAndAdminAsync(app.Services);

// 7. Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ajout de l'authentification et de l'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Configuration des routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Admin}/{action=ParfumIndex}/{id?}");

// Ex�cution de l'application
app.Run();
app.UseStaticFiles();

/// <summary>
/// Initialise les r�les et l'utilisateur administrateur par d�faut.
/// </summary>
/// <param name="serviceProvider">Service provider de l'application.</param>
async Task InitializeRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Client>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Cr�er le r�le Admin s'il n'existe pas
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Cr�er le r�le User s'il n'existe pas
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    // Ajouter un administrateur par d�faut
    var adminEmail = "admin@parfumshop.com";
    var adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new Client
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nom = "Admin",
            Prenom = "Parfum",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
