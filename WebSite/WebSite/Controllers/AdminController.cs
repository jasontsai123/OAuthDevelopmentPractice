#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebSite.Database;
using WebSite.Repositories.LineNotifySubscriber;

namespace WebSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly MemberContext _context;

        public AdminController(MemberContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.LineNotifySubscribers.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineNotifySubscriber = await _context.LineNotifySubscribers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lineNotifySubscriber == null)
            {
                return NotFound();
            }

            return View(lineNotifySubscriber);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LineUserId,AccessToken")] LineNotifySubscriber lineNotifySubscriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lineNotifySubscriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lineNotifySubscriber);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineNotifySubscriber = await _context.LineNotifySubscribers.FindAsync(id);
            if (lineNotifySubscriber == null)
            {
                return NotFound();
            }
            return View(lineNotifySubscriber);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LineUserId,AccessToken")] LineNotifySubscriber lineNotifySubscriber)
        {
            if (id != lineNotifySubscriber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lineNotifySubscriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LineNotifySubscriberExists(lineNotifySubscriber.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lineNotifySubscriber);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineNotifySubscriber = await _context.LineNotifySubscribers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lineNotifySubscriber == null)
            {
                return NotFound();
            }

            return View(lineNotifySubscriber);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lineNotifySubscriber = await _context.LineNotifySubscribers.FindAsync(id);
            _context.LineNotifySubscribers.Remove(lineNotifySubscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LineNotifySubscriberExists(int id)
        {
            return _context.LineNotifySubscribers.Any(e => e.Id == id);
        }
    }
}
