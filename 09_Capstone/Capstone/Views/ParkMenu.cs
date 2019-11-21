using System;
using System.Collections.Generic;
using System.Text;
using Capstone.DAL;
using Capstone.Models;
using System.Globalization;

namespace Capstone.Views
{
    public class ParkMenu : ProjectCLI
    {
        public ParkMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.Title = "View Parks Interface";
        }

        public CampgroundMenu campgroundMenu;
        protected override void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            IList<Park> parks = parkDAO.AvailableParks();
            int count = 1;
            if (parks.Count > 0)
            {
                foreach (Park park in parks)
                {
                    Console.WriteLine($"{count}. {park.Name}");
                    count++;
                }
            }
            Console.WriteLine("q. Quit Program");

            Console.WriteLine();
            Console.Write("Select an option for more information: ");

            string userInputString = Console.ReadLine();
            int userInput;
            bool isInt = int.TryParse(userInputString, out userInput);
            Console.WriteLine();
            IList<Park> parkDetails = parkDAO.ViewParkDetails(userInput);
            if (userInputString.ToLower() == "q")
            {
                QuitProgram();
                return;
            }

            while (true)
            {
                if (parkDetails.Count > 0)
                {
                    Console.Clear();
                        Console.WriteLine("Park Information Screen");
                        foreach (Park detail in parkDetails)
                        {
                            Console.WriteLine($"Name:             {detail.Name}");
                            Console.WriteLine($"Location:         {detail.Location}");
                            Console.WriteLine($"Established on:   {detail.EstablishDate:d}");
                            Console.WriteLine($"Area:             {detail.Area:##,#} sq km");
                            Console.WriteLine($"Annual Visitors:  {detail.Visitors:##,#}");
                            Console.WriteLine($"\nPark Description: \n{detail.Description}");
                        }
                }
                else
                {
                    Console.WriteLine("This is not a valid selection. Please press enter to return to the main menu.");
                    Console.ReadLine();
                    Console.Clear();
                    PrintHeader();
                    PrintMenu();
                    break;
                }
                Console.WriteLine();
                Console.WriteLine("1)   View Campgrounds");
                Console.WriteLine("2)   Search for Campsite Reservation");
                Console.WriteLine("3)   Search for Park-wide Reservation");
                Console.WriteLine("4)   View this month's reservation");
                Console.WriteLine("5)   Return to Main Menu");
                Console.WriteLine();
                Console.WriteLine("Press 'Q' to Quit");
                Console.WriteLine();
                Console.Write("Select an option: ");
                string menuInput = Console.ReadLine();
                if (menuInput.ToLower() == "q")
                {
                    {
                        QuitProgram();
                        return;
                    }
                }
                //int menuInput;
                //bool isAnInt = int.TryParse(userInputString, out menuInput);
                Console.Clear();
                switch (menuInput)
                {
                    case "1":
                        ViewCampground(userInput);
                        Console.WriteLine();
                        Console.Write("Press enter to make a reservation: ");
                        Console.ReadLine();
                        SearchCampground(userInput);
                        break;

                    case "2":
                        ViewCampground(userInput);
                        SearchCampsite();
                        break;

                    case "3":
                        ViewCampground(userInput);
                        SearchCampground(userInput);
                        break;

                    case "4":
                        ViewReservations();
                        break;
                    case "5":
                        PrintHeader();
                        PrintMenu();
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Press enter to return to the main menu");
                Console.ReadLine();
                Console.Clear();
                PrintHeader();
                PrintMenu();
            }
        }

        private void ViewCampground(int userInput)
        {
            IList<Campground> campgrounds = campgroundDAO.ViewCampgrounds(userInput);
            Console.WriteLine("Park Campgrounds");
            Console.WriteLine();
            if (campgrounds.Count > 0)
            {
                Console.WriteLine("     Campground Name                    Open           Close          Daily Fee");
                foreach (Campground campground in campgrounds)
                {
                    string openMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(campground.Open);
                    string closeMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(campground.Close);
                    Console.WriteLine($"{campground.CampgroundId,-5}{campground.Name,-35}{openMonthName,-15}{closeMonthName,-15}{campground.DailyFee,-9:C}");
                }
            }
        }

        private void SearchCampsite()
        {
            Console.WriteLine();
            Console.Write("Please enter the number of the campsite you'd like to search: ");
            string userInputString = Console.ReadLine();
            int campgroundId;
            bool isInt = int.TryParse(userInputString, out campgroundId);
            if (isInt)
            {
                Console.Write("Please enter a start date (mm/dd/yyyy): ");
                userInputString = Console.ReadLine();
                DateTime startDate;
                bool isStartDate = DateTime.TryParse(userInputString, out startDate);
                Console.Write("Please enter an end date (mm/dd/yyyy): ");
                userInputString = Console.ReadLine();
                DateTime endDate;
                bool isEndDate = DateTime.TryParse(userInputString, out endDate);
                if (isEndDate && isStartDate)
                {
                    bool isInSeason = reservationDAO.IsInSeason(campgroundId, startDate, endDate);
                    if (isInSeason)
                    {
                        Console.Clear();
                        Console.WriteLine("Results Matching Your Search Criteria");
                        Console.WriteLine();
                        Console.WriteLine("Site No.  Max Occup.  Accessible? Max RV Length  Utility   Cost");
                        List<int> topFiveSites = new List<int>();
                        IList<Site> sites = siteDAO.AvailableReservations(campgroundId, startDate, endDate);
                        if (sites.Count > 0)
                        {
                            foreach (Site site in sites)
                            {
                                topFiveSites.Add(site.SiteId);
                                decimal cost = ((decimal)(endDate - startDate).TotalDays) * site.DailyFee;
                                string accessibility = "";
                                if (site.Accessible == false)
                                {
                                    accessibility = "No";
                                }
                                else
                                {
                                    accessibility = "Yes";
                                }

                                string utilities = "";
                                if (site.Utilities == false)
                                {
                                    utilities = "No";
                                }
                                else
                                {
                                    utilities = "Yes";
                                }
                                string maxRvLength = "";
                                if (site.MaxRvLength == 0)
                                {
                                    maxRvLength = "N/A";
                                }
                                else
                                {
                                    maxRvLength = Convert.ToString(site.MaxRvLength);
                                }
                                Console.WriteLine($"{site.SiteId,-10}{site.MaxOccupancy,-12}{accessibility,-12}{maxRvLength,-15}{utilities,-10}{cost,-10:c}");
                            }
                        }
                        else
                        {
                            Console.Write("There are no available sites in that date range. Would you like to enter alternative dates (Y/N)?");
                            string response = Console.ReadLine().ToLower();
                            if (response == "y")
                            {
                                ViewCampground(campgroundId);
                                SearchCampsite();
                            }
                            else
                            {
                                Console.Clear();
                                PrintHeader();
                                PrintMenu();
                            }
                        }
                        Console.WriteLine();
                        Console.Write("Which site should be reserved (Enter 0 to cancel)? ");
                        int siteChosen = int.Parse(Console.ReadLine());
                        if (siteChosen == 0)
                        {
                            Console.Clear();
                            PrintHeader();
                            PrintMenu();
                        }
                        Console.Write("What name should the reservation be made under? ");
                        string name = Console.ReadLine();
                        if (topFiveSites.Contains(siteChosen))
                        {
                            int reservationId = reservationDAO.CreateReservation(siteChosen, name, startDate, endDate);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"The reservation has been made. Your confirmation ID is {reservationId}");
                            Console.WriteLine();
                            Console.WriteLine(@"               (                 ,&&&.               ");
                            Console.WriteLine(@"                )                .,.&&               ");
                            Console.WriteLine(@"               (  (              \=__/               ");
                            Console.WriteLine(@"                   )             ,'-'.               ");
                            Console.WriteLine(@"             (    (  ,,      _.__|/ /|               ");
                            Console.WriteLine(@"              ) /\ -((------((_|___/ |               ");
                            Console.WriteLine(@"            (  // | (`'      ((  `'--|               ");
                            Console.WriteLine(@"          _ -.;_/ \\--._      \\ \-._/.              ");
                            Console.WriteLine(@"         (_;-// | \ \-'.\    <_,\_\`--'|             ");
                            Console.WriteLine(@"         ( `.__ _  ___,')      <_,-'__,'             ");
                            Console.WriteLine(@"           `'(_ )_)(_)_)'                            ");
                            Console.WriteLine();         
                            Console.WriteLine("           We hope you enjoy your stay!               ");
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.WriteLine("This is not a valid campsite. Please try again.");
                            Console.ReadLine();
                            Console.Clear();
                            ViewCampground(campgroundId);
                            SearchCampsite();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, this park is not in season on the specified dates. Press Enter to return to the Main Menu.");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine();
                        Console.WriteLine(@"                                          *  .  *                                               ");    
                        Console.WriteLine(@"        o                               . _\/ \/_ .                                   \o/       ");       
                        Console.WriteLine(@"   o    :    o                           \  \ /  /           .      .             _o/.:|:.\o_   ");  
                        Console.WriteLine(@"     '.\'/.'          ..    ..         -==>: X :<==-         _\/  \/_               .\:|:/.     ");  
                        Console.WriteLine(@"     :->@<-:          '\    /'           / _/ \_ \            _\/\/_            -=>>::>o<::<<=- ");  
                        Console.WriteLine(@"     .'/.\'.            \\//            '  /\ /\  '       _\_\_\/\/_/_/_          _ '/:|:\' _   "); 
                        Console.WriteLine(@"   o    :    o     _.__\\\///__._         *  '  *          / /_/\/\_\ \            o\':|:'/o    ");  
                        Console.WriteLine(@"        o           '  ///\\\  '                              _/\/\_                  /o\       ");  
                        Console.WriteLine(@"                        //\\                                  /\  /\                            ");     
                        Console.WriteLine(@"                      ./    \.                               '      '                           ");
                        Console.WriteLine(@"                      ''    ''                                                                  ");
                        Console.ReadLine();
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        PrintHeader();
                        PrintMenu();
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("This is not a valid date. Please hit enter to try again");
                    Console.ReadLine();
                    SearchCampsite();
                }
            }
            else
            {
                Console.WriteLine("This is not a valid campsite. Please press enter to try again.");
                Console.ReadLine();
                SearchCampsite();
            }
            
        }
        private void SearchCampground(int parkId)
        {
            Console.Write("Please enter a start date (mm/dd/yyyy): ");
            string userInputString = Console.ReadLine();
            DateTime startDate;
            bool isStartDate = DateTime.TryParse(userInputString, out startDate);
            Console.Write("Please enter an end date (mm/dd/yyyy): ");
            userInputString = Console.ReadLine();
            DateTime endDate;
            bool isEndDate = DateTime.TryParse(userInputString, out endDate);


            if (isStartDate && isEndDate)
            {
                Console.Clear();
                Console.WriteLine("Results Matching Your Search Criteria");
                Console.WriteLine();
                Console.WriteLine("Campground Name                    Site No.  Max Occup.  Accessible? Max RV Length  Utility   Cost");
                List<int> topFiveSites = new List<int>();
                IList<Site> sites = siteDAO.BonusAvailableReservations(startDate, endDate);
                if (sites.Count > 0)
                {
                    foreach (Site site in sites)
                    {
                        topFiveSites.Add(site.SiteId);
                        decimal cost = ((decimal)(endDate - startDate).TotalDays) * site.DailyFee;
                        string accessibility = "";
                        if (site.Accessible == false)
                        {
                            accessibility = "No";
                        }
                        else
                        {
                            accessibility = "Yes";
                        }

                        string utilities = "";
                        if (site.Utilities == false)
                        {
                            utilities = "No";
                        }
                        else
                        {
                            utilities = "Yes";
                        }
                        string maxRvLength = "";
                        if (site.MaxRvLength == 0)
                        {
                            maxRvLength = "N/A";
                        }
                        else
                        {
                            maxRvLength = Convert.ToString(site.MaxRvLength);
                        }
                        Console.WriteLine($"{site.CampgroundName,-35}{site.SiteId,-10}{site.MaxOccupancy,-12}{accessibility,-12}{maxRvLength,-15}{utilities,-10}{cost,-10:c}");
                    }
                }
                else
                {
                    Console.Write("There are no available sites in that date range. Would you like to enter alternative dates (Y/N)?");
                    string response = Console.ReadLine().ToLower();
                    if (response == "y")
                    {
                        ViewCampground(parkId);
                        SearchCampsite();
                    }
                    else
                    {
                        Console.Clear();
                        PrintHeader();
                        PrintMenu();
                    }
                }
                Console.WriteLine();
                Console.Write("Which site should be reserved (Enter 0 to cancel)? ");
                int siteChosen = int.Parse(Console.ReadLine());
                if (siteChosen == 0)
                {
                    Console.Clear();
                    PrintHeader();
                    PrintMenu();
                }
                else if (siteChosen > 0)
                {
                    List<int> siteIds = new List<int>();
                    foreach (Site site in sites)
                    {
                        siteIds.Add(site.SiteId);
                    }
                    if (!siteIds.Contains(siteChosen))
                    {
                        Console.WriteLine("This is not a valid selection. Please press Enter to try again.");
                        Console.ReadLine();
                        Console.Clear();
                        ViewCampground(parkId);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write("What name should the reservation be made under? ");
                        string name = Console.ReadLine();
                        if (topFiveSites.Contains(siteChosen))
                        {
                            int reservationId = reservationDAO.CreateReservation(siteChosen, name, startDate, endDate);
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"The reservation has been made. Your confirmation ID is {reservationId}");
                            Console.WriteLine();
                            Console.WriteLine(@"               (                 ,&&&.               ");
                            Console.WriteLine(@"                )                .,.&&               ");
                            Console.WriteLine(@"               (  (              \=__/               ");
                            Console.WriteLine(@"                   )             ,'-'.               ");
                            Console.WriteLine(@"             (    (  ,,      _.__|/ /|               ");
                            Console.WriteLine(@"              ) /\ -((------((_|___/ |               ");
                            Console.WriteLine(@"            (  // | (`'      ((  `'--|               ");
                            Console.WriteLine(@"          _ -.;_/ \\--._      \\ \-._/.              ");
                            Console.WriteLine(@"         (_;-// | \ \-'.\    <_,\_\`--'|             ");
                            Console.WriteLine(@"         ( `.__ _  ___,')      <_,-'__,'             ");
                            Console.WriteLine(@"           `'(_ )_)(_)_)'                            ");
                            Console.WriteLine();
                            Console.WriteLine("           We hope you enjoy your stay!               ");
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.WriteLine("This is not a valid campground. Please try again.");
                            Console.ReadLine();
                            Console.Clear();
                            ViewCampground(parkId);
                            SearchCampsite();
                        }
                    }

                }
                
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("This is not a valid date. Please hit enter to try again");
                Console.ReadLine();
                SearchCampground(parkId);
            }
        }

        public override void RunCLI()
        {
            PrintHeader();
            PrintMenu();
        }
        private void AvailableParks()
        {


            IList<Park> parks = parkDAO.AvailableParks();
            int count = 1;
            if (parks.Count > 0)
            {
                foreach (Park park in parks)
                {
                    Console.WriteLine($"{count}. {park.Name}");
                    count++;
                }
            }
        }       

        private void ViewReservations()
        {
            Console.WriteLine("Here are all upcoming reservations for the next 30 days");
            Console.WriteLine();
            IList<Reservation> reservations = reservationDAO.ViewReservations();
            Console.WriteLine("Site Id   From Date      To Date        Name");
            foreach (Reservation reservation in reservations)
            {
                Console.WriteLine($"{reservation.SiteId,-10}{reservation.FromDate,-15:d}{reservation.ToDate,-15:d}{reservation.Name}");
            }
            Console.WriteLine();
            Console.WriteLine("Please press Enter to return to the Main Menu");
            Console.ReadLine();
            Console.Clear();
            PrintHeader();
            PrintMenu();
        }

        protected override void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"                      /\                                                                            "); 
            Console.WriteLine(@"                     /**\                                                                           ");    
            Console.WriteLine(@"                    /****\   /\                                                                     "); 
            Console.WriteLine(@"                   /      \ /**\                                                                    ");  
            Console.WriteLine(@"                  /  /\    /    \        /\    /\  /\      /\            /\/\/\  /\                 "); 
            Console.WriteLine(@"                 /  /  \  /      \      /  \/\/  \/  \  /\/  \/\  /\  /\/ / /  \/  \                ");  
            Console.WriteLine(@"                /  /    \/ /\     \    /    \ \  /    \/ /   /  \/  \/  \  /    \   \               ");   
            Console.WriteLine(@"               /  /      \/  \/\   \  /      \    /   /    \                                        ");       
            Console.WriteLine(@"  ____________/__/_______/___/__\___\_____________________________________________________________  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(@" _____                           _ _         ______                               _   _             ");
            Console.WriteLine(@"/  __ \                         (_) |        | ___ \                             | | (_)            ");
            Console.WriteLine(@"| /  \/ __ _ _ __ ___  _ __  ___ _| |_ ___   | |_/ /___  ___  ___ _ ____   ____ _| |_ _  ___  _ __  ");
            Console.WriteLine(@"| |    / _` | '_ ` _ \| '_ \/ __| | __/ _ \  |    // _ \/ __|/ _ \ '__\ \ / / _` | __| |/ _ \| '_ \ ");
            Console.WriteLine(@"| \__/\ (_| | | | | | | |_) \__ \ | ||  __/  | |\ \  __/\__ \  __/ |   \ V / (_| | |_| | (_) | | | |");
            Console.WriteLine(@" \____/\__,_|_| |_| |_| .__/|___/_|\__\___|  \_| \_\___||___/\___|_|    \_/ \__,_|\__|_|\___/|_| |_|");
            Console.WriteLine(@"                      | |                                                                           ");
            Console.WriteLine(@"                      |_|                                                                           ");
            Console.WriteLine();
        }

        public void QuitProgram()
        {
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("        Thank you for using the Campsite Reservation System");
                Console.WriteLine(@"                     _                                            ");
                Console.WriteLine(@"                   _(_)_                          wWWWw   _       ");
                Console.WriteLine(@"       @@@@       (_)@(_)   vVVVv     _     @@@@  (___) _(_)_     ");
                Console.WriteLine(@"      @@()@@ wWWWw  (_)\    (___)   _(_)_  @@()@@   Y  (_)@(_)    ");
                Console.WriteLine(@"       @@@@  (___)     `|/    Y    (_)@(_)  @@@@   \|/   (_)\     ");
                Console.WriteLine(@"        /      Y       \|    \|/    /(_)    \|      |/      |     ");
                Console.WriteLine(@"     \ |     \ |/       | / \ | /  \|/       |/    \|      \|/    ");
                Console.WriteLine(@"     \\|//   \\|///  \\\|//\\\|/// \|///  \\\|//  \\|//  \\\|//   ");
                Console.WriteLine(@" ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.ForegroundColor = ConsoleColor.Green;
                return;
            }
        }
    }
}
