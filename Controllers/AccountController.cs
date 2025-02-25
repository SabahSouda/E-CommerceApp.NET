using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonParfumApp.Models; // Remplacez par le namespace correct
using MonParfumApp.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<Client> _userManager;
    private readonly SignInManager<Client> _signInManager;
    private readonly IPasswordHasher<Client> _passwordHasher;

    public AccountController(UserManager<Client> userManager, SignInManager<Client> signInManager, IPasswordHasher<Client> passwordHasher)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _passwordHasher = passwordHasher;
    }

    // Afficher le formulaire d'inscription
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Soumettre le formulaire d'inscription
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new Client
            {
                UserName = model.Email,
                Email = model.Email,
                Nom = model.Nom,
                Prenom = model.Prenom
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Connexion automatique après inscription
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Parfum");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return View(model);
    }




    // Affiche la page de connexion
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Traite les données du formulaire de connexion
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                // Récupérer l'utilisateur
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Vérifier si l'utilisateur est un administrateur
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Rediriger vers la page d'administration
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Rediriger vers la page des parfums pour les utilisateurs standards
                    return RedirectToAction("Index", "Parfum");
                }
            }

            ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
        }

        return View(model);
    }



    // Déconnexion
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // Accès refusé
    public IActionResult AccessDenied()
    {
        return View();
    }
}

