using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Manager.Abstractions;

namespace MyGarageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyGarageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMyGarageManager _myGarageManager;

        public MyGarageController(IConfiguration configuration, IMyGarageManager myGarageManager)
        {
            _configuration = configuration;
            _myGarageManager = myGarageManager;
        }


        #region GET
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetDbConnection() 
        {
            string? sql = _configuration.GetConnectionString("SQLiteConnection");
            return Ok(sql);
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<ActionResult<List<Vehicle>>> GetVehicle([FromQuery] string immatriculation)
        {
            var vehicles = await _myGarageManager.GetVehicleAsync(immatriculation);

            if (vehicles == null)
                return NotFound(); // retourne 404 si rien trouvé

            return Ok(vehicles); // retourne la liste des véhicules trouvés
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetHistVehicle (string immatriculation)
        {
            var res = await _myGarageManager.GetHistVehicleAsync(immatriculation);
            if (res is not null) 
                return Ok(res);
            return NotFound("Véhicule non trouvé.");
        }
        #endregion

        #region POST
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AddVehicle ([FromBody] Vehicle vehicle)
        {
            var res = await _myGarageManager.AddVehicleAsync(vehicle);
            if (res is not null) 
                return Ok("Véhicule ajouté avec succès.");

            return BadRequest("Erreur lors de l'ajout du véhicule.");
        }
        #endregion

        #region DELETE
        [HttpDelete]
        [Route("[Action]")]
        public async Task<IActionResult> DeleteVehicle ([FromQuery] string immatriculation)
        {
            var res = await _myGarageManager.DeleteVehicleAsync(immatriculation);
            if (res is not null) 
                return Ok("Véhicule supprimé avec succès.");

            return BadRequest("Erreur lors de la suppression du véhicule.");
        }
        #endregion
    }
}
