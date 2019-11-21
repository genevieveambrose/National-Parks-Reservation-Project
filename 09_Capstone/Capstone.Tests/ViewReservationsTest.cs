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
    public class ViewReservationsTest
    {
        List<Reservation> reservations = new List<Reservation>();
        private TransactionScope transaction;
        const string connectionString = @"Server=.\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";

        [TestInitialize]
        public void Setup()
        {
            this.transaction = new TransactionScope();
            string script;

            using (StreamReader reader = new StreamReader("ViewReservationTestSetup.sql"))
            {
                script = reader.ReadToEnd();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(script, connection);
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
        public void ViewReservationsTest1()
        {
            //arrange
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);
            //act
            IList<Reservation> listOfReservations = dao.ViewReservations();
            //assert
            Assert.AreEqual(30, listOfReservations.Count);
        }
    }
}
