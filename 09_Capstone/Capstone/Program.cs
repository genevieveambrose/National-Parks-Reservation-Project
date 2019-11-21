using Capstone.DAL;
using Capstone.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            // Get the connection string from the appsettings.json file
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Project");

            IReservationDAO reservationDAO = new ReservationSqlDAO(connectionString);
            IParkDAO parkDAO = new ParkSqlDAO(connectionString);
            ISiteDAO siteDAO = new SiteSqlDAO(connectionString);
            ICampgroundDAO campgroundDAO = new CampgroundSqlDAO(connectionString);

            ParkMenu menu = new ParkMenu(parkDAO, campgroundDAO, siteDAO, reservationDAO);
            menu.RunCLI();
            Console.ReadLine();
        }
    }
}

//TODO Create method SearchReservations()
//TODO Create method MakeReservation()
//TODO Create method ViewAvailableParks()
//TODO Create method ViewAvailableCampgrounds()
//TODO Create method DisplayTop5Campsites
//TODO Create method DateAvailability() - name subject to change
//TODO Create method CalculateCost()