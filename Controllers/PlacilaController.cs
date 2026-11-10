using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data;
using SmartPark.Models;

namespace SmartPark.Controllers
{
    public class PlacilaController : Controller
    {
        private readonly SmartParkContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PlacilaController> _logger;

        public PlacilaController(
            SmartParkContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PlacilaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlaciloRequest request)
        {
            try
            {
                if (request == null || request.rezervacijaId <= 0)
                    return BadRequest(new { success = false, message = "Neveljaven zahtevek za plačilo." });

                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Uporabnik ni prijavljen." });

                var rezervacija = await _context.Rezervacije
                    .Include(r => r.Parkirisce)
                    .FirstOrDefaultAsync(r => r.Id == request.rezervacijaId && r.ApplicationUserId == userId);

                if (rezervacija == null)
                    return NotFound(new { success = false, message = "Rezervacija ne obstaja ali ne pripada uporabniku." });

                // Pomembno: validacija časov
                var zacetek = rezervacija.Zacetek;
                var konec = rezervacija.Konec;

                if (konec <= zacetek)
                    return BadRequest(new { success = false, message = "Neveljaven časovni interval rezervacije." });

                var trajanjeUr = (konec - zacetek).TotalHours;

                var cenaNaUro = rezervacija.Parkirisce?.CenaNaUro ?? 2.5m;
                var znesek = Math.Round((decimal)trajanjeUr * cenaNaUro, 2);

                // lep “display” način plačila
                var nacin = (request.nacinPlacila ?? "card").ToLowerInvariant();
                var nacinDisplay = nacin switch
                {
                    "card" => "Bančna kartica",
                    "nalog" => "Plačilni nalog",
                    _ => request.nacinPlacila ?? "Bančna kartica"
                };

                var placilo = new Placilo
                {
                    RezervacijaId = rezervacija.Id,
                    ApplicationUserId = userId,
                    Znesek = znesek,
                    NacinPlacila = nacinDisplay,
                    Datum = DateTime.Now
                };

                _context.Placila.Add(placilo);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, rezervacijaId = rezervacija.Id, znesek });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a payment for reservation {ReservationId}.", request?.rezervacijaId);
                return StatusCode(500, new { success = false, message = "Pri obdelavi plačila je prišlo do napake." });
            }
        }
    }

    public class PlaciloRequest
    {
        public int rezervacijaId { get; set; }
        public string? nacinPlacila { get; set; }
    }
}
