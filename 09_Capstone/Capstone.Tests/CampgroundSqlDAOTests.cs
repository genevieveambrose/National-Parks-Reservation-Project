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
    public class CampgroundSqlDAOTests
    {
        List<Campground> campgrounds = new List<Campground>();
        private TransactionScope transaction;
        const string connectionString = @"Server=.\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        [TestInitialize]
        public void Setup()
        {
            this.transaction = new TransactionScope();
            string script;

            using (StreamReader reader = new StreamReader("CampgroundSqlDAOTestSetup.sql"))
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
                    foreach (Campground campground in campgrounds)
                    {
                        campground.Name = Convert.ToString(reader["name"]);
                        campgrounds.Add(campground);
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
        public void ViewCampgroundsTest()
        {
            //arrange
            CampgroundSqlDAO dao = new CampgroundSqlDAO(connectionString);
            //act
            IList<Campground> listOfCampgrounds = dao.ViewCampgrounds(1);
            //assert
            Assert.AreEqual(3, listOfCampgrounds.Count);
        }
    }
}
