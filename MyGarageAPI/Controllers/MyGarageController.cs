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
        public async Task<ActionResult<List<Vehicle>>> GetHistVehicle([FromQuery] string immatriculation)
        {
            var vehicles = await _myGarageManager.GetHistVehicleAsync(immatriculation);

            if (vehicles == null)
                return NotFound(); // retourne 404 si rien trouvé

            return Ok(vehicles); // retourne la liste des véhicules trouvés
        }
        #endregion

        #region POST
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AddVehicle ([FromBody] Vehicle vehicle)
        {
            // Logique pour ajouter un véhicule (non implémentée ici)
            return Ok("Véhicule ajouté avec succès (fonctionnalité non implémentée).");
        }
        #endregion

        #region DELETE
        [HttpDelete]
        [Route("[Action]")]
        public async Task<IActionResult> DeleteVehicle ([FromQuery] string immatriculation)
        {
            // Logique pour supprimer un véhicule (non implémentée ici)
            return Ok("Véhicule supprimé avec succès.");
        }
        #endregion
    }
}
