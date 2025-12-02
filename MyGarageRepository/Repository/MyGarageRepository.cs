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

            string query = "SELECT * FROM Vehicles WHERE immatriculation = @immatriculation";
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

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            string query = "INSERT INTO Vehicles (marque, modele, immatriculation, kilometrage) VALUES (@marque, @modele, @immatriculation, @kilometrage); " +
                           "SELECT last_insert_rowid();";
            using var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddWithValue("@marque", vehicle.marque ?? string.Empty);
            cmd.Parameters.AddWithValue("@modele", vehicle.modele ?? string.Empty);
            cmd.Parameters.AddWithValue("@kilometrage", vehicle.kilometrage ?? 0);
            cmd.Parameters.AddWithValue("@immatriculation", vehicle.immatriculation ?? string.Empty);
            var result = await cmd.ExecuteScalarAsync();
            if (result != null && int.TryParse(result.ToString(), out int newId))
            {
                vehicle.ID = newId;
                return vehicle;
            }
            return null;
        }

        public async Task<Vehicle> DeleteVehicleAsync(string immatriculation)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            // Récupérer le véhicule avant suppression
            string selectQuery = "SELECT * FROM Vehicles WHERE immatriculation = @immatriculation";
            using var selectCmd = new SqliteCommand(selectQuery, conn);
            selectCmd.Parameters.AddWithValue("@immatriculation", immatriculation);

            Vehicle vehicleToDelete = null;
            using (var reader = await selectCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    vehicleToDelete = new Vehicle
                    {
                        ID = reader.GetInt32(0),
                        marque = reader.GetString(1),
                        modele = reader.GetString(2),
                        immatriculation = reader.GetString(3),
                    };
                }
            }

            if (vehicleToDelete == null)
                return null; // Véhicule non trouvé

            // Supprimer le véhicule
            string deleteQuery = "DELETE FROM Vehicles WHERE id = @id";
            using var deleteCmd = new SqliteCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@id", vehicleToDelete.ID);
            await deleteCmd.ExecuteNonQueryAsync();

            return vehicleToDelete;
        }
    }
}
