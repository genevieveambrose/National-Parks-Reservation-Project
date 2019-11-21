using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Capstone.Views
{
    public class CampgroundMenu : ProjectCLI
    {
        public CampgroundMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.Title = "View Parks Interface";
        }
        public override void RunCLI()
        {
            throw new NotImplementedException();
        }

        protected override void PrintHeader()
        {
            throw new NotImplementedException();
        }

        protected override void PrintMenu()
        {
            throw new NotImplementedException();
        }
        
    }
}
