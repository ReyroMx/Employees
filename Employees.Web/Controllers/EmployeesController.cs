using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employees.Web.Data;
using Employees.Web.Models;
using System.Text.RegularExpressions;

namespace Employees.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DataContext _context;

        public EmployeesController(DataContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.OrderBy(e => e.BornDate).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,RFC,BornDate,Status")] Employee employee)
        {



            if (ModelState.IsValid)
            {
                try
                {
                    employee.RFC = employee.RFC.ToUpper();
                    string errorMsg = string.Empty;
                    if (IsValidRFC(employee.RFC, employee.BornDate, out errorMsg))
                    {
                        _context.Add(employee);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
        
                    ModelState.AddModelError(string.Empty, errorMsg);
                  
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There are a record with the same RFC.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }

              
            }
            return View(employee);
        }

        private bool IsValidRFC(string rFC,DateTime BornDate, out string errorMsg)
        {

            if (Regex.IsMatch(rFC, "[A-z]{4}[0-9]{6}[A-z0-9]{3}") || Regex.IsMatch(rFC, "[A-z]{3}[0-9]{6}[A-z0-9]{3}"))
            {
                int year = int.Parse(rFC.Substring(rFC.Length - 9, 2));
                int month = int.Parse(rFC.Substring(rFC.Length - 7, 2));
                int day = int.Parse(rFC.Substring(rFC.Length - 5, 2));

                int BornDay = BornDate.Day;
                int BornMonth = BornDate.Month;
                int BornYear = int.Parse(BornDate.Year.ToString().Substring(2));

                if (day.Equals(BornDay) && month.Equals(BornMonth) && (year.Equals(BornYear)))
                {
                    errorMsg = "The RFC is correct";
                    return true;
                }
                errorMsg = "The RFC does not match the born date";
                return false;
            }
            else
            {
                errorMsg = "The RFC format is not valid";
                return false;
            }
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,LastName,RFC,BornDate,Status")] Employee employee)
        {
            if (id != employee.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    string errorMsg = string.Empty;
                    if (IsValidRFC(employee.RFC, employee.BornDate, out errorMsg))
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, errorMsg);
                    }
                
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "There are a record with the same RFC.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }
}
