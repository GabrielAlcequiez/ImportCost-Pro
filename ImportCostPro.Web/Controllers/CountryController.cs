using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.Country;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryService _service;
        public CountryController(ICountryService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllCountriesAsync();



            var viewModels = dtos.Select(country => new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive
            }).ToList();
            return View(viewModels);
        }

        public IActionResult Create()
        {
            var viewModel = new CreateCountryViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCountryViewModel viewModel)
        {
            // Validaciones rápidas del cliente (Data Annotations en el ViewModel)
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                // Mapeamos los datos del ViewModel de la UI hacia el DTO que espera tu servicio
                var createDto = new CreateCountryDto
                {
                    Name = viewModel.Name,
                    ISOCode = viewModel.ISOCode
                };

                // Enviamos el DTO a la capa de negocio
                await _service.CreateCountryAsync(createDto);

                // Si todo sale bien, lo mandamos de vuelta al listado con un mensaje implícito de éxito
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex) // Atrapamos los errores arrojados por FluentValidation
            {
                // Desglosamos los errores del validador y los inyectamos en el ModelState de la vista
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string name)
        {
            return View();
        }

    }
}