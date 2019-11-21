using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSqlDAO : IReservationDAO
    {
        private string connectionString;
        public ReservationSqlDAO(string connectionString)
        {
            this.connectionString = "Server=.\\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        }

        public int CreateReservation(int siteChosen, string name, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date, create_date) VALUES (@siteId, @name, @startDate, @endDate, @now)", connection);
                    command.Parameters.AddWithValue("@siteId", siteChosen);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    command.Parameters.AddWithValue("@now", DateTime.Now);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"SELECT MAX (reservation_id) FROM reservation", connection);
                    int reservationId = Convert.ToInt32(command.ExecuteScalar());
                    return reservationId;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        public bool IsInSeason(int siteChosen, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int startMonth = Convert.ToInt32(startDate.Month);
                    int endMonth = Convert.ToInt32(endDate.Month);
                    SqlCommand command = new SqlCommand("SELECT open_from_mm, open_to_mm, campground_id FROM campground WHERE (open_from_mm <= @startMonth OR open_to_mm >= @endMonth) AND campground_id = @siteChosen", connection);
                    command.Parameters.AddWithValue("@startMonth", startMonth);
                    command.Parameters.AddWithValue("@endMonth", endMonth);
                    command.Parameters.AddWithValue("@siteChosen", siteChosen);
                    command.ExecuteNonQuery();

                    SqlDataReader reader = command.ExecuteReader();
                    List<Reservation> reservations = new List<Reservation>();
                    while (reader.Read())
                    {
                        Reservation reservation = new Reservation();
                        reservation.OpenMonth = Convert.ToInt32(reader["open_from_mm"]);
                        reservation.CloseMonth = Convert.ToInt32(reader["open_to_mm"]);
                        if (reservation.OpenMonth > startMonth || reservation.CloseMonth < endMonth)
                        {
                            return false;
                        }
                        else
                        {
                            reservations.Add(reservation);
                        }
                    }

                    if (reservations.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        public IList<Reservation> ViewReservations()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("select site_id, name, from_date, to_date from reservation WHERE from_date BETWEEN @now AND @end", connection);
                    command.Parameters.AddWithValue("@now", DateTime.Now);
                    DateTime end = DateTime.Now.AddDays(30);
                    command.Parameters.AddWithValue("@end", end);

                    SqlDataReader reader = command.ExecuteReader();
                    IList<Reservation> reservations30Days = new List<Reservation>();
                    while (reader.Read())
                    {
                        Reservation reservation = new Reservation();
                        reservation.Name = Convert.ToString(reader["name"]);
                        reservation.SiteId = Convert.ToInt32(reader["site_id"]);
                        reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
                        reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
                        reservations30Days.Add(reservation);
                    }
                    return reservations30Days;
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
