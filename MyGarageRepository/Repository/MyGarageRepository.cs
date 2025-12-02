using Models.Models;
using Repository.Abstractions;
using Microsoft.Data.Sqlite;

namespace Repository.Repository
{
    public class MyGarageRepository : IMyGarageRepository
    {
        private readonly string _connectionString;

        // Injection de la connection string via le constructeur
        public MyGarageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Vehicle> GetHistVehicleAsync(string immatriculation)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM Vehicles WHERE Immatriculation = @immatriculation";
            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@immatriculation", immatriculation);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Vehicle
                {
                    ID = reader.GetInt32(0),
                    marque = reader.GetString(1),
                    modele = reader.GetString(2),
                    immatriculation = reader.GetString(3),
                };
            }

            return null;
        }
    }
}
