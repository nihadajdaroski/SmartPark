using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using SmartPark.Data;
using SmartPark.Models;

namespace SmartPark.Controllers
{
    public class RezervacijeController : Controller
    {
        private readonly SmartParkContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RezervacijeController(SmartParkContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var rezervacije = await _context.Rezervacije
                .Include(r => r.Parkirisce)
                .Include(r => r.ParkirnoMesto)
                .Where(r => r.ApplicationUserId == userId)
                .OrderByDescending(r => r.Zacetek)
                .ToListAsync();

            return View(rezervacije);
        }

        [HttpGet]
        public async Task<IActionResult> GetProstoMesto(int parkirisceId, DateTime zacetek, DateTime konec)
        {
            if (konec <= zacetek)
                return Json(0);

            var mestoId = await _context.ParkirnaMesta
                .Where(pm => pm.ParkirisceId == parkirisceId)
                .Where(pm => !_context.Rezervacije.Any(r =>
                    r.ParkirnoMestoId == pm.Id &&
                    r.Zacetek < konec &&
                    r.Konec > zacetek))
                .OrderBy(pm => pm.Id)
                .Select(pm => pm.Id)
                .FirstOrDefaultAsync();

            return Json(mestoId); // 0 pomeni "ni prostih"
        }

        [HttpGet]
        public async Task<IActionResult> GetProstaMesta(int parkirisceId, DateTime zacetek, DateTime konec)
        {
            if (konec <= zacetek)
                return Json(Array.Empty<int>());

            var mesta = await _context.ParkirnaMesta
                .Where(pm => pm.ParkirisceId == parkirisceId)
                .Where(pm => !_context.Rezervacije.Any(r =>
                    r.ParkirnoMestoId == pm.Id &&
                    r.Zacetek < konec &&
                    r.Konec > zacetek))
                .OrderBy(pm => pm.Stevilka)
                .Select(pm => pm.Id)
                .ToListAsync();

            return Json(mesta);
        }

        [HttpGet]
        public async Task<IActionResult> DemoQrCode(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var rezervacija = await _context.Rezervacije
                .Include(r => r.Parkirisce)
                .Include(r => r.ParkirnoMesto)
                .FirstOrDefaultAsync(r => r.Id == id && r.ApplicationUserId == userId);

            if (rezervacija == null)
                return NotFound();

            var payload =
                $"SMARTPARK-DEMO|Rezervacija:{rezervacija.Id}|Parkirisce:{rezervacija.Parkirisce?.Naslov}|Mesto:{rezervacija.ParkirnoMesto?.Stevilka}|Od:{rezervacija.Zacetek:O}|Do:{rezervacija.Konec:O}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
            var qrCode = new PngByteQRCode(qrCodeData);

            return File(qrCode.GetGraphic(12), "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> Create(int ParkirisceId, int ParkirnoMestoId, DateTime DatumZacetka, DateTime DatumKonca)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            if (DatumKonca <= DatumZacetka)
                return BadRequest(new { success = false, message = "Neveljaven časovni interval." });

            var pm = await _context.ParkirnaMesta.FirstOrDefaultAsync(x => x.Id == ParkirnoMestoId);
            if (pm == null)
                return BadRequest(new { success = false, message = $"Parkirno mesto {ParkirnoMestoId} ne obstaja." });

            if (pm.ParkirisceId != ParkirisceId)
                return BadRequest(new { success = false, message = "Izbrano parkirno mesto ne pripada izbranemu parkirišču." });

            var zasedenoVTerminu = await _context.Rezervacije.AnyAsync(r =>
                r.ParkirnoMestoId == ParkirnoMestoId &&
                r.Zacetek < DatumKonca &&
                r.Konec > DatumZacetka);

            if (zasedenoVTerminu)
                return BadRequest(new { success = false, message = "Za izbrani termin ni prostih mest. Poskusi drug termin." });

            var rezervacija = new Rezervacija
            {
                ParkirisceId = ParkirisceId,
                ParkirnoMestoId = ParkirnoMestoId,
                Zacetek = DatumZacetka,
                Konec = DatumKonca,
                ApplicationUserId = userId,
                DateCreated = DateTime.Now
            };

            pm.Zasedeno = true;

            _context.Rezervacije.Add(rezervacija);
            await _context.SaveChangesAsync();

            return Json(new { success = true, rezervacijaId = rezervacija.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var rez = await _context.Rezervacije
                .Include(r => r.ParkirnoMesto)
                .FirstOrDefaultAsync(r => r.Id == id && r.ApplicationUserId == userId);

            if (rez == null)
                return NotFound();

            if (rez.ParkirnoMesto != null)
                rez.ParkirnoMesto.Zasedeno = false;

            _context.Rezervacije.Remove(rez);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
