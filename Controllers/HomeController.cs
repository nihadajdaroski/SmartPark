using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartPark.Data;
using SmartPark.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Controllers;

public class HomeController : Controller
{
    private readonly SmartParkContext _context;

    public HomeController(SmartParkContext context)
    {
        _context = context;
    }

    // Home Page
    [AllowAnonymous] // anonimni uporabniki lahko vidijo redirect
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            // Če ni prijavljen, ga preusmeri na login
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Naložimo parkirišča skupaj z njihovimi parkirnimi mesti
        var parkirisca = await _context.Parkirisca
            .Include(p => p.ParkirnaMesta)
            .ToListAsync();

        return View(parkirisca); // Poslano v Index.cshtml kot @model IEnumerable<Parkirisce>
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        });
    }

    // POST: /Home/Rezerviraj?parkirisceId=1
    [Authorize]
    [HttpPost]
    public async Task<JsonResult> Rezerviraj(int parkirisceId)
    {
        var parkirisce = await _context.Parkirisca
            .Include(p => p.ParkirnaMesta)
            .FirstOrDefaultAsync(p => p.Id == parkirisceId);

        if (parkirisce == null)
            return Json(new { success = false, message = "Parkirišče ne obstaja." });

        var prostoMesto = parkirisce.ParkirnaMesta.FirstOrDefault(m => !m.Zasedeno);
        if (prostoMesto == null)
            return Json(new { success = false, message = "Ni prostih mest." });

        // Označimo mesto kot zasedeno
        prostoMesto.Zasedeno = true;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}
