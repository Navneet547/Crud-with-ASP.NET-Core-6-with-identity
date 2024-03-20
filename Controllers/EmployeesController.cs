using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPIdentityApp.Areas.Identity.Data;
using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using log4net;
using Microsoft.CodeAnalysis.Differencing;

namespace ASPIdentityApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;
        private static readonly ILog _log = LogManager.GetLogger(typeof(EmployeesController));

        //public string ErrorMessage { get; private set; }


        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                //var employees = await _context.Employees.Skip(0).Take(5).ToListAsync();
                var employees = await _context.Employees.ToListAsync();
                if (employees != null && employees.Count > 0)
                {
                    _log.Info("Employee list fetched successfully.");
                    _logger.LogInformation("Employee list fetched successfully.");
                }
                else
                {
                    _log.Warn("No employees found.");
                    _logger.LogWarning("No employees found.");
                    ModelState.AddModelError(string.Empty, "No employees found.");
                }
                return View(employees);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _logger.LogError(ex, "An error occurred while fetching the employee list.");
                ModelState.AddModelError(string.Empty, ex.Message);
                throw;
            }
        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(m => m.EmpId == id);

                if (employee == null)
                {
                    _log.Warn($"Employee with ID {id} not found.");
                    _logger.LogWarning($"Employee with ID {id} not found.");
                    throw new Exception("Execption occured");
                }

                _log.Info("Employee Details fetched successfully.");
                _logger.LogInformation("Employee Details fetched successfully.");
                return View(employee);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _logger.LogError(ex, $"An error occurred while fetching details for employee ID {id}.");
                //TempData["ErrorMessage"] = $"An error occurred while fetching details for employee ID {id}.";
                throw;
            }
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("EmpId,EmpName,EmpAge,EmpGender,EmpAddress")] Employee employee)
        {
            var ErrorMessage = "Invalid model state during employee creation.";

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    _log.Info($"Employee with ID {employee.EmpId} created successfully.");
                    _logger.LogInformation($"Employee with ID {employee.EmpId} created successfully.");
                    TempData["SuccessMessage"] = $"New Employee {employee.EmpName} added successfully!";
                    return RedirectToAction(nameof(Index));
                }

                _log.Warn(ErrorMessage);
                _logger.LogWarning(ErrorMessage);
                ModelState.AddModelError(string.Empty, ErrorMessage);
                throw new Exception("Execption occured");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while creating the employee.");
                _logger.LogError(ex, "An error occurred while creating the employee.");
                ModelState.AddModelError(string.Empty, ErrorMessage);
                throw;
            }
        }


        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                var ErrorMessage = $"Employee with ID {id} not found.";
                if (employee == null)
                {
                    _log.Warn(ErrorMessage);
                    _logger.LogWarning(ErrorMessage);
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                    throw new Exception("Execption occured");
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                _log.Error($"An error occurred while fetching details for employee ID {id} during edit.");
                _logger.LogError(ex, $"An error occurred while fetching details for employee ID {id} during edit.");
                throw;
            }
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            var ErrorMessage = "Sorry! Employee does not exists.";

            try
            {
                if (id != employee.EmpId)
                {
                    _log.Error("Invalid request parameters. ID is null or Employees set is null.");
                    _logger.LogError("Invalid request parameters. ID is null or Employees set is null.");
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeExists(employee.EmpId))
                        {
                            _log.Error("Invalid request parameters. ID is null or Employees set is null.");
                            _logger.LogError("Invalid request parameters. ID is null or Employees set is null.");
                            ModelState.AddModelError(string.Empty, ErrorMessage);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    _log.Info("Employee Edited Successful.");
                    _logger.LogInformation("Employee Edited Successful.");
                    TempData["SuccessMessage"] = $"Employee {employee.EmpName} Information Updated Successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _log.Error($"An error occurred while editing the employee with ID {id}.");
                _logger.LogError(ex, $"An error occurred while editing the employee with ID {id}.");
                throw;
            }
            
        }


        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Employee {employee.EmpName} deleted Successfully.";
                }

                return RedirectToAction("Index", "Employees");
            }
            catch (Exception ex)
            {
                _log.Error($"An error occurred while deleting the employee with ID {id}.");
                _logger.LogError(ex, $"An error occurred while deleting the employee with ID {id}.");

                ModelState.AddModelError(string.Empty, "Error on deleting employee.");
                throw;
            }
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmpId == id)).GetValueOrDefault();
        }
    }
}
