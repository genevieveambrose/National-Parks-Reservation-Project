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
    public class ParkSqlDAOTests
    {

        List<Park> parks = new List<Park>();
        private TransactionScope transaction;
        const string connectionString = @"Server=.\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        [TestInitialize]
        public void Setup()
        {
            this.transaction = new TransactionScope();
            string script;

            using (StreamReader reader = new StreamReader("ParkSqlDAOTestSetup.sql"))
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
                    foreach(Park park in parks)
                    {
                        park.ParkID = Convert.ToInt32(reader["park_id"]);
                        parks.Add(park);
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
        public void AvailableParkTest()
        {
            //arrange
            ParkSqlDAO dao = new ParkSqlDAO(connectionString);
            
            //act
            IList<Park> listOfParks = dao.AvailableParks();
            
            //assert
            Assert.AreEqual(3, listOfParks.Count);
        }

        [TestMethod]
        public void ViewParkDetailsTest()
        {
            //arrange
            ParkSqlDAO dao = new ParkSqlDAO(connectionString);

            //act
            IList<Park> listOfParks = dao.ViewParkDetails(1);


            //assert
            Assert.AreEqual(1, listOfParks.Count);
        }
    }
}
