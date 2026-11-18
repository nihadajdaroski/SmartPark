using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using SmartPark.Data;
using SmartPark.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Controllers;

public class HomeController : Controller
{
    private const string ParkirisceCacheKey = ParkirisceCache.Key;
    private static readonly DistributedCacheEntryOptions ParkirisceCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    // DTO brez povratne reference (ParkirnoMesto.Parkirisce), da se da serializirati.
    private sealed record MestoDto(int Id, int Stevilka, bool Zasedeno, TipMesta Tip);
    private sealed record ParkirisceDto(int Id, string Naslov, int SteviloMest, decimal CenaNaUro,
        string DelovniCas, double Latitude, double Longitude, List<MestoDto> Mesta);

    private readonly SmartParkContext _context;
    private readonly IDistributedCache _cache;
    private readonly ParkirisceCache _parkirisceCache;

    public HomeController(SmartParkContext context, IDistributedCache cache, ParkirisceCache parkirisceCache)
    {
        _context = context;
        _cache = cache;
        _parkirisceCache = parkirisceCache;
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

        var json = await _cache.GetStringAsync(ParkirisceCacheKey);
        if (json is null)
        {
            // Naložimo parkirišča skupaj z njihovimi parkirnimi mesti
            var parkirisca = await _context.Parkirisca
                .Include(p => p.ParkirnaMesta)
                .ToListAsync();

            var dtos = parkirisca.Select(p => new ParkirisceDto(
                p.Id, p.Naslov, p.SteviloMest, p.CenaNaUro, p.DelovniCas, p.Latitude, p.Longitude,
                p.ParkirnaMesta.Select(m => new MestoDto(m.Id, m.Stevilka, m.Zasedeno, m.Tip)).ToList())).ToList();

            json = JsonSerializer.Serialize(dtos);
            await _cache.SetStringAsync(ParkirisceCacheKey, json, ParkirisceCacheOptions);
        }

        var cached = JsonSerializer.Deserialize<List<ParkirisceDto>>(json)!;
        var model = cached.Select(d => new Parkirisce
        {
            Id = d.Id,
            Naslov = d.Naslov,
            SteviloMest = d.SteviloMest,
            CenaNaUro = d.CenaNaUro,
            DelovniCas = d.DelovniCas,
            Latitude = d.Latitude,
            Longitude = d.Longitude,
            ParkirnaMesta = d.Mesta.Select(m => new ParkirnoMesto
            {
                Id = m.Id,
                ParkirisceId = d.Id,
                Stevilka = m.Stevilka,
                Zasedeno = m.Zasedeno,
                Tip = m.Tip
            }).ToList()
        }).ToList();

        return View(model); // Poslano v Index.cshtml kot @model IEnumerable<Parkirisce>
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
        await _parkirisceCache.InvalidateAsync();

        return Json(new { success = true });
    }
}
