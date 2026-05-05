using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public VenueController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            // Show any error messages from TempData (e.g. blocked deletion)
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,Name,Location,Capacity")] Venue venue, IFormFile? ImageFile)
        {
            venue.CreatedAt = DateTime.Now;

            // Remove ImageUrl from validation since we handle it manually
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                // Upload image to Azurite if provided
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(ImageFile.ContentType))
                    {
                        ModelState.AddModelError("ImageFile", "Only image files (jpg, png, gif, webp) are allowed.");
                        return View(venue);
                    }

                    venue.ImageUrl = await _blobService.UploadImageAsync(ImageFile);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,Name,Location,Capacity,ImageUrl")] Venue venue, IFormFile? ImageFile)
        {
            if (id != venue.VenueId) return NotFound();

            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image is uploaded, replace old one
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                        if (!allowedTypes.Contains(ImageFile.ContentType))
                        {
                            ModelState.AddModelError("ImageFile", "Only image files (jpg, png, gif, webp) are allowed.");
                            return View(venue);
                        }

                        // Delete old blob if it exists
                        if (!string.IsNullOrEmpty(venue.ImageUrl))
                            await _blobService.DeleteImageAsync(venue.ImageUrl);

                        venue.ImageUrl = await _blobService.UploadImageAsync(ImageFile);
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Block deletion if venue has active bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Delete blob image if it exists
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                    await _blobService.DeleteImageAsync(venue.ImageUrl);

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}