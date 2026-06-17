using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UmiCeramics.Data;
using UmiCeramics.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace UmiCeramics.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public BookingsController(
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings.Include(b => b.Workshop);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Workshop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create(int workshopId)
        {
            var booking = new Booking
            {
                WorkshopId = workshopId
            };
            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerEmail,NumberOfSeats,WorkshopId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var workshop = await _context.Workshops
                       .Include(w => w.Bookings)
                        .FirstOrDefaultAsync(w => w.Id == booking.WorkshopId);

                if (workshop == null)
                {
                    return NotFound();
                }

                var bookedSeats = workshop.Bookings.Sum(b => b.NumberOfSeats);

                if (bookedSeats + booking.NumberOfSeats > workshop.Capacity)
                {
                    ModelState.AddModelError("", "Det är fullbokat");
                    return View(booking);
                }
                booking.CancellationToken = Guid.NewGuid();
                _context.Add(booking);
                await _context.SaveChangesAsync();

                var cancelUrl = $"{Request.Scheme}://{Request.Host}/bookings/cancel?token={booking.CancellationToken}";
                Console.WriteLine(cancelUrl);
                await _emailSender.SendEmailAsync(
                    booking.CustomerEmail,
                    "Boknings Bekreftälse",
                       $"""
                    <h1>Tack för din bokning!</h1>

                    <p>Name: {booking.CustomerName}</p>

                    <p>Seats booked: {booking.NumberOfSeats}</p>

                    <p>
                        <a href="{cancelUrl}">
                            Cancel Booking
                        </a>
                    </p>
                    """
                );

                return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
            }
            ViewData["WorkshopId"] = new SelectList(_context.Workshops, "Id", "Id", booking.WorkshopId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["WorkshopId"] = new SelectList(_context.Workshops, "Id", "Id", booking.WorkshopId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerEmail,NumberOfSeats,WorkshopId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["WorkshopId"] = new SelectList(_context.Workshops, "Id", "Id", booking.WorkshopId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Workshop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
            .Include(b => b.Workshop)
            .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        public async Task<IActionResult> Cancel(Guid token)
        {
            var booking = await _context.Bookings
                    .Include(b => b.Workshop)
                    .FirstOrDefaultAsync(b => b.CancellationToken == token);

            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancel(Guid token)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.CancellationToken == token);

            if (booking == null)
            {
                return NotFound();
            }
            _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();

            return View("Cancelled");


        }
    }
}
