using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class ParkSqlDAO : IParkDAO
    {
        private string connectionString;
        public ParkSqlDAO(string connectionString)
        {
            this.connectionString = "Server=.\\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        }

        public IList<Park> AvailableParks()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT name FROM park ORDER BY name", connection);

                    SqlDataReader reader = command.ExecuteReader();

                    IList<Park> parks = new List<Park>();

                    while (reader.Read())
                    {
                        Park park = new Park();
                        park.Name = Convert.ToString(reader["name"]);
                        parks.Add(park);
                    }
                    return parks;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public IList<Park> ViewParkDetails(int userInput)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand($"SELECT * FROM park WHERE park_id = @userInput", connection);
                    command.Parameters.AddWithValue("@userInput", userInput);

                    command.ExecuteNonQuery();

                    SqlDataReader reader = command.ExecuteReader();

                    List<Park> parks = new List<Park>();
                    while (reader.Read())
                    {
                        Park park = new Park();
                        park.Name = Convert.ToString(reader["name"]);
                        park.Location = Convert.ToString(reader["location"]);
                        park.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
                        park.Area = Convert.ToInt32(reader["area"]);
                        park.Visitors = Convert.ToInt32(reader["visitors"]);
                        park.Description = Convert.ToString(reader["description"]);
                        parks.Add(park);
                    }
                    return parks;
                    
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}
