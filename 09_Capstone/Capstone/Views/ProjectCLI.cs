
using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Views
{
    public abstract class ProjectCLI
    {
        protected const string Command_AllParks = "1";
        protected const string Command_ParkMenu = "2";
        protected const string Command_Quit = "q";

        //const string Command_EmployeeSearch = "3";
        //const string Command_EmployeesWithoutProjects = "4";
        //const string Command_ProjectList = "5";
        //const string Command_CreateDepartment = "6";
        //const string Command_UpdateDepartment = "7";
        //const string Command_CreateProject = "8";
        //const string Command_AssignEmployeeToProject = "9";
        //const string Command_RemoveEmployeeFromProject = "10";

        protected IParkDAO parkDAO;
        protected ICampgroundDAO campgroundDAO;
        protected ISiteDAO siteDAO;
        protected IReservationDAO reservationDAO;

        public ProjectCLI(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO)
        {
            this.parkDAO = parkDAO;
            this.campgroundDAO = campgroundDAO;
            this.siteDAO = siteDAO;
            this.reservationDAO = reservationDAO;
        }

        public string Title { get; set; }



        //abstract protected void ExecuteSelection(string choice);


        abstract public void RunCLI();



        abstract protected void PrintHeader();


        abstract protected void PrintMenu();
        

    } 
}