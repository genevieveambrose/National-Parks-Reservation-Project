using Capstone.DAL;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace Capstone.Tests
{
    [TestClass]
    public class ReservationSqlDAOTest
    {
        List<Reservation> reservations = new List<Reservation>();
        private TransactionScope transaction;
        const string connectionString = @"Server=.\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";

        [TestInitialize]
        public void Setup()
        {
            this.transaction = new TransactionScope();
            string script;

            using (StreamReader reader = new StreamReader("ReservationSqlDAOTestSetup.sql"))
            {
                script = reader.ReadToEnd();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT reservation_id FROM reservation WHERE name = @reservationName", connection);
                command.Parameters.AddWithValue("@reservationName", "Voldemort");
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    foreach (Reservation reservation in reservations)
                    {
                        reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
                        reservations.Add(reservation);
                    }
                }
            }

        }

        [TestCleanup]
        public void Cleanup()
        {
            this.transaction.Dispose();
        }

        [TestMethod]
        public void CreateReservationTest()
        {
            //arrange
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);
            //act
            DateTime startDate = new DateTime(2019, 10, 19);
            DateTime endDate = new DateTime(2019, 10, 21);
            int reservationId = dao.CreateReservation(1, "Voldemort", startDate, endDate);
            //assert
            Assert.AreEqual(47, reservationId);
        }

        [TestMethod]
        public void IsInSeasonTest()
        {
            //arrange
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);
            //act
            DateTime startDate = new DateTime(2019, 10, 19);
            DateTime endDate = new DateTime(2019, 10, 21);
            bool isInSeason = dao.IsInSeason(1, startDate, endDate);
            //assert
            Assert.IsTrue(isInSeason);

        }
    }
}
