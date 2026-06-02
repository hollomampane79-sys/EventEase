using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index(int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? availableOnly)
        {
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            // Populate EventType dropdown for the filter UI
            ViewBag.EventTypeId = new SelectList(_context.EventTypes, "EventTypeId", "Name", eventTypeId);
            ViewBag.CurrentEventTypeId = eventTypeId;
            ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.AvailableOnly = availableOnly ?? false;

            var events = _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Bookings)
                .AsQueryable();

            // Filter by EventType
            if (eventTypeId.HasValue)
                events = events.Where(e => e.EventTypeId == eventTypeId.Value);

            // Filter by date range
            if (startDate.HasValue)
                events = events.Where(e => e.StartDate >= startDate.Value);

            if (endDate.HasValue)
                events = events.Where(e => e.EndDate <= endDate.Value);

            // Filter: only events with no bookings (venue availability)
            if (availableOnly == true)
                events = events.Where(e => !e.Bookings!.Any());

            return View(await events.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Name,Description,StartDate,EndDate,CreatedAt,EventTypeId")] Event @event)
        {
            if (@event.EndDate < @event.StartDate)
                ModelState.AddModelError("EndDate", "End date cannot be earlier than the start date.");

            if (@event.StartDate < DateTime.Today)
                ModelState.AddModelError("StartDate", "Start date cannot be in the past.");

            if (string.IsNullOrWhiteSpace(@event.Name))
                ModelState.AddModelError("Name", "Event name is required.");

            if (ModelState.IsValid)
            {
                @event.CreatedAt = DateTime.Now;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
            return View(@event);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,Description,StartDate,EndDate,CreatedAt,EventTypeId")] Event @event)
        {
            if (id != @event.EventId) return NotFound();

            if (@event.EndDate < @event.StartDate)
                ModelState.AddModelError("EndDate", "End date cannot be earlier than the start date.");

            if (string.IsNullOrWhiteSpace(@event.Name))
                ModelState.AddModelError("Name", "Event name is required.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                ViewBag.HasBookings = true;
                ViewBag.Warning = "This event has active bookings and cannot be deleted.";
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}