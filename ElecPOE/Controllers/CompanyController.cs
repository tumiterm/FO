// <copyright file="CompanyController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    20/01/2023 13:09:27 PM
// Purpose:         Defines the CompanyController class

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;

#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Represents the controller responsible for managing companies, their addresses, contact persons, and related data.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class CompanyController : Controller
    {
        #region Private Fields

        private readonly ILogger<CompanyController> _logger;

        /// <summary>
        /// Provides access to the utility methods.
        /// </summary>
        private readonly IHelperService _helperService;

        /// <summary>
        /// Provides access to the company-related data operations.
        /// </summary>
        private readonly IUnitOfWork _context;

        /// <summary>
        /// Provides access to the host environment, used for file storage and other environmental configurations.
        /// </summary>
        private IWebHostEnvironment _hostEnvironment;

        /// <summary>
        /// Provides configuration settings for the application.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Provides access to the user-related data operations.
        /// </summary>
        private readonly IUserService _userService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyController"/> class.
        /// </summary>
        /// <param name="context">The company data context.</param>
        /// <param name="placementContext">The placement data context.</param>
        /// <param name="hostEnvironment">The web host environment.</param>
        /// <param name="configuration">The application configuration settings.</param>
        public CompanyController(IUnitOfWork context,
                                    IWebHostEnvironment hostEnvironment,
                                    IConfiguration configuration, IHelperService helperService,
                                    IUserService userService, ILogger<CompanyController> logger
                                    )
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
            _helperService = helperService;
            _userService = userService;
            _logger = logger;

        }

        /// <summary>
        /// Displays the companies management page.
        /// </summary>
        /// <returns>A view of the companies management page.</returns>
        public IActionResult Companies()
        {
            return View();
        }

        /// <summary>
        /// Processes the creation of a new company, including its address and contact person.
        /// </summary>
        /// <param name="companyDTO">The company data transfer object containing details for the new company.</param>
        /// <returns>A view with the operation result.</returns>
        [HttpPost]
        public async Task<IActionResult> Companies(CompanyAddressContactViewModel companyDTO)
        {
            Guid scopedGuid = Helper.GenerateGuid();

            companyDTO.CompanyId = scopedGuid;

            try
            {
                var currentUser = _userService.OnGetCurrentUser();
                var createdBy = $"{currentUser?.Name} {currentUser?.LastName}";
                var currentTime = _helperService.GetCurrentTime();

                var company = new Company
                {
                    CompanyId = scopedGuid,
                    CreatedBy = createdBy,
                    IsActive = true,
                    CreatedOn = currentTime.ToShortDateString(), 
                    CompanyName = companyDTO.CompanyName,
                    Phone = companyDTO.Phone,
                    Speciality = companyDTO.Speciality,
                    Address = new Address
                    {
                        AddressId = Guid.NewGuid(),
                        Line1 = companyDTO.Line1,
                        StreetName = companyDTO.StreetName,
                        City = companyDTO.City,
                        Province = companyDTO.Province,
                        PostalCode = companyDTO.PostalCode,
                        AssociativeId = scopedGuid
                    },
                    Contact = new ContactPerson
                    {
                        ContactId = Guid.NewGuid(),
                        AssociativeId = scopedGuid,
                        Name = companyDTO.Name,
                        LastName = companyDTO.LastName,
                        Email = companyDTO.Email,
                        Cellphone = companyDTO.Cellphone
                    }
                };

                await _context.Company.AddAsync(company);
                int rowsAffected = await _context.SaveAsync();

                if (rowsAffected > 0)
                {
                    TempData["success"] = "Company successfully saved";
                    return RedirectToAction(nameof(HostCompanies)); 
                }
                else
                {
                    TempData["error"] = "Error: Company could not be saved.";
                }

            }
            catch(Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while saving company");
                TempData["error"] = "An unexpected error occurred while saving the company.";

            }

            return View(companyDTO);
        }

        /// <summary>
        /// Displays a list of active companies hosted in the system.
        /// </summary>
        /// <returns>A view containing the list of active companies.</returns>
        public async Task<IActionResult> HostCompanies()
        {
            var companies = await _context.Company.GetAllAsync();

            var filterCompanies = from n in companies

                                  where n.IsActive == true

                                  select n;

            return View(filterCompanies.ToList());
        }

        /// <summary>
        /// Retrieves the details of a specific company for modification.
        /// </summary>
        /// <param name="CompanyId">The unique identifier of the company to modify.</param>
        /// <returns>A view containing the company's details.</returns>
        [HttpGet]
        public async Task<IActionResult> OnModifyHostCompany(Guid CompanyId)
        {
            if(CompanyId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var company = await _context.Company.GetAsync(filter: c => c.CompanyId == CompanyId);

            var contacts = await _context.ContactPerson.GetAllAsync();

            var addressList = await _context.Address.GetAllAsync(); 

            var list = from n in contacts

                       where n.AssociativeId == company.CompanyId

                       select n;

            var addresses = from n in addressList

                       where n.AssociativeId == company.CompanyId

                       select n;

            CompanyAddressContactViewModel cacDTO = new()
            {
                Phone = company.Phone,

                Speciality = company.Speciality,

                CompanyName = company.CompanyName, 

                CompanyId = CompanyId,

                Email = list.First().Email, 

                Cellphone = list.First().Cellphone,

                Name = list.First().Name,

                LastName= list.First().LastName,

                City = addresses.First().City,

                Line1 = addresses.First().Line1,  
                
                StreetName = addresses.First().StreetName,

                PostalCode= addresses.First().PostalCode,

                Province= addresses.First().Province,

                IsActive = company.IsActive,
                   

            };

            return View(cacDTO);
        }

        /// <summary>
        /// Processes the modification of a company's details.
        /// </summary>
        /// <param name="model">The company data transfer object containing updated details.</param>
        /// <returns>A view with the operation result.</returns>
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyHostCompany(CompanyAddressContactViewModel model)
        {
            Company company = null;

            if(ModelState.IsValid)
            {
                company = new Company
                {
                    Phone = model.Phone,
                    Speciality = model.Speciality,
                    CompanyName = model.CompanyName,
                    CompanyId = model.CompanyId,

                    Address = new Address
                    {
                        City = model.City,
                        Line1 = model.Line1,
                        StreetName = model.StreetName,
                        PostalCode = model.PostalCode,
                        Province = model.Province,
                    },

                    Contact = new ContactPerson
                    {
                        Cellphone= model.Cellphone,
                        Email = model.Email,
                        Name = model.Name,
                        LastName = model.LastName,  
                    },

                    IsActive = model.IsActive,
                    ModifiedBy = $"{_userService.OnGetCurrentUser()?.Name} {_userService.OnGetCurrentUser()?.LastName}",
                    ModifiedOn = Helper.OnGetCurrentDateTime(),

                };

                if(company != null && company.Contact != null && company.Address != null)
                {
                    var compModel = await _context.Company.UpdateCompanyAsync(company);  

                    if (compModel != null)
                    {
                        TempData["success"] = "Company successfully saved";
                        return RedirectToAction(nameof(HostCompanies));
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// Retrieves a list of learners associated with a specific company.
        /// </summary>
        /// <param name="CompanyId">The unique identifier of the company.</param>
        /// <returns>A view containing the list of learners associated with the company.</returns>
        public async Task<IActionResult> OnGetLearners(Guid CompanyId)
        {
            var company = await OnConvertToCompany(CompanyId);

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            var placements = await _context.Placement.GetAllAsync();

            var filteredPlacements = placements.Where(p => p.CompanyId == CompanyId);

            var students = new List<LearnerViewModel>();

            var today = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

            foreach (var placementRecord in filteredPlacements)
            {
                eStatus? status = placementRecord.Status; 

                if (placementRecord.EndDate.HasValue && today > placementRecord.EndDate.Value.Date)
                {
                    status = eStatus.Completed; 
                }

                status ??= eStatus.StartingSoon; 

                var learner = new LearnerViewModel
                {
                    LearnerName = placementRecord.Student,
                    End = placementRecord.EndDate.HasValue? placementRecord.EndDate.Value.ToShortDateString(): "N/A",
                    Start = placementRecord.StartDate.HasValue? placementRecord.StartDate.Value.ToShortDateString(): "N/A",
                    Status = status.ToString() 
                };

                students.Add(learner);
            }

            var addresses = await _context.Address.GetAllAsync();
            var address = addresses.FirstOrDefault(a => a.AssociativeId == company.CompanyId);

            ViewData["comp"] = $"{company.CompanyName} ({company.Speciality})";
            ViewData["address"] = address != null ? $"{address.Line1} {address.StreetName} {address.City} {address.Province} {address.PostalCode}": "No address available";

            return View(students);
        }


        /// <summary>
        /// Converts a company identifier to a <see cref="Company"/> object.
        /// </summary>
        /// <param name="CompanyId">The unique identifier of the company.</param>
        /// <returns>The company object associated with the identifier.</returns>
        private async Task<Company> OnConvertToCompany(Guid CompanyId)
        {
            return await _context.Company.GetAsync(filter: c => c.CompanyId == CompanyId);
        }

    }
}
