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
    public class SiteSqlDAOTests
    {
        List<Site> sites = new List<Site>();
        private TransactionScope transaction;
        const string connectionString = @"Server=.\SQLEXPRESS;Database=npcampground;Trusted_Connection=True;";
        [TestInitialize]
        public void Setup()
        {
            this.transaction = new TransactionScope();
            string script;

            using (StreamReader reader = new StreamReader("SiteSqlDAOTestSetup.sql"))
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
                    foreach (Site site in sites)
                    {
                        site.SiteId = Convert.ToInt32(reader["site_id"]);
                        sites.Add(site);
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
        public void ViewCampsitesTest()
        {
            //arrange
            SiteSqlDAO dao = new SiteSqlDAO(connectionString);

            //act
            DateTime startDate = new DateTime(2019, 10, 19);
            DateTime endDate = new DateTime(2019, 10, 21);
            IList<Site> listOfSites = dao.ViewCampsites(1, startDate, endDate);

            //assert
            Assert.AreEqual(5, listOfSites.Count);
        }

        [TestMethod]
        public void AvailableReservationTest()
        {
            //arrange
            SiteSqlDAO dao = new SiteSqlDAO(connectionString);

            //act
            DateTime startDate = new DateTime(2019, 10, 19);
            DateTime endDate = new DateTime(2019, 10, 21);
            IList<Site> listOfSites = dao.ViewCampsites(1, startDate, endDate);

            //assert
            Assert.AreEqual(5, listOfSites.Count);

        }
    }
}
