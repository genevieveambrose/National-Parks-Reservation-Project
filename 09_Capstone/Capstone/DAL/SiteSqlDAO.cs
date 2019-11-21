using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class SiteSqlDAO : ISiteDAO
    {
        private string connectionString;
        public SiteSqlDAO(string connectionString)
        {
            this.connectionString = "Server=.\\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        }

        public IList<Site> ViewCampsites(int campgroundId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select TOP 5 site_id, max_occupancy, accessible, max_rv_length, utilities, daily_fee, name from site s JOIN campground c ON c.campground_id = s.campground_id", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    List<Site> sites = new List<Site>();
                    while (reader.Read())
                    {
                        Site site = new Site();
                        Campground campground = new Campground();
                        site.SiteId = Convert.ToInt32(reader["site_id"]);
                        site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                        site.Accessible = Convert.ToBoolean(reader["accessible"]);
                        site.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
                        site.Utilities = Convert.ToBoolean(reader["utilities"]);
                        //TODO how to add daily fee
                        site.DailyFee = Convert.ToDecimal(reader["daily_fee"]);
                        sites.Add(site);
                    }
                    return sites;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
        public List<Site> AvailableReservations(int campgroundId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select TOP 5 c.name AS name, s.site_id AS id, s.max_occupancy AS occupancy, s.accessible AS accessible, s.max_rv_length AS length, s.utilities AS utilities, c.daily_fee AS fee from site s LEFT JOIN reservation r ON s.site_id = r.site_id LEFT JOIN campground c ON c.campground_id = s.campground_id WHERE(c.campground_id = @campgroundId) AND((from_date NOT BETWEEN @startDate AND @endDate) AND(to_date NOT BETWEEN @startDate AND @endDate) OR reservation_id IS NULL) GROUP BY s.site_id, c.name, s.max_occupancy, s.accessible, s.max_rv_length, s.utilities, c.daily_fee", connection);
                    command.Parameters.AddWithValue("@campgroundId", campgroundId);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataReader reader = command.ExecuteReader();
                    List<Site> sites = new List<Site>();
                    while (reader.Read())
                    {
                        Site site = new Site();
                        site.SiteId = Convert.ToInt32(reader["id"]);
                        site.MaxOccupancy = Convert.ToInt32(reader["occupancy"]);
                        site.Accessible = Convert.ToBoolean(reader["accessible"]);
                        site.MaxRvLength = Convert.ToInt32(reader["length"]);
                        site.Utilities = Convert.ToBoolean(reader["utilities"]);
                        site.DailyFee = Convert.ToDecimal(reader["fee"]);
                        sites.Add(site);

                    }
                    return sites;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public IList<Site> BonusAvailableReservations(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select TOP 5 c.name AS name, s.site_id AS id, s.max_occupancy AS occupancy, s.accessible AS accessible, s.max_rv_length AS length, s.utilities AS utilities, c.daily_fee AS fee from site s LEFT JOIN reservation r ON s.site_id = r.site_id LEFT JOIN campground c ON c.campground_id = s.campground_id WHERE ((from_date NOT BETWEEN @startDate AND @endDate) AND(to_date NOT BETWEEN @startDate AND @endDate) OR reservation_id IS NULL) GROUP BY s.site_id, c.name, s.max_occupancy, s.accessible, s.max_rv_length, s.utilities, c.daily_fee", connection);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataReader reader = command.ExecuteReader();
                    List<Site> sites = new List<Site>();
                    while (reader.Read())
                    {
                        Site site = new Site();
                        site.CampgroundName = Convert.ToString(reader["name"]);
                        site.SiteId = Convert.ToInt32(reader["id"]);
                        site.MaxOccupancy = Convert.ToInt32(reader["occupancy"]);
                        site.Accessible = Convert.ToBoolean(reader["accessible"]);
                        site.MaxRvLength = Convert.ToInt32(reader["length"]);
                        site.Utilities = Convert.ToBoolean(reader["utilities"]);
                        site.DailyFee = Convert.ToDecimal(reader["fee"]);
                        sites.Add(site);

                    }
                    return sites;
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
