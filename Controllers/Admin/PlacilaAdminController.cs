using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data;

namespace SmartPark.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    [Route("admin/placila")]
    public class PlacilaAdminController : Controller
    {
        private readonly SmartParkContext _context;

        public PlacilaAdminController(SmartParkContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var placila = await _context.Placila
                .Include(p => p.Rezervacija)
                    .ThenInclude(r => r!.Parkirisce)
                .OrderByDescending(p => p.Datum)
                .ToListAsync();

            return View(placila);
        }
    }
}