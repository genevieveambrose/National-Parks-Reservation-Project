using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class CampgroundSqlDAO : ICampgroundDAO
    {
        private string connectionString;
        public CampgroundSqlDAO(string connectionString)
        {
            this.connectionString = "Server=.\\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        }

        public IList<Campground> ViewCampgrounds(int parkId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT campground_id, name, open_from_mm, open_to_mm, daily_fee FROM campground WHERE park_id = @parkId", connection);
                    command.Parameters.AddWithValue("@parkId", parkId);
                    SqlDataReader reader = command.ExecuteReader();
                    List<Campground> campgrounds = new List<Campground>();
                    while (reader.Read())
                    {
                        Campground campground = new Campground();
                        campground.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                        campground.Name = Convert.ToString(reader["name"]);
                        campground.Open = Convert.ToInt32(reader["open_from_mm"]);
                        campground.Close = Convert.ToInt32(reader["open_to_mm"]);
                        campground.DailyFee = Convert.ToDecimal(reader["daily_fee"]);
                        campgrounds.Add(campground);
                    }
                    return campgrounds;
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}
