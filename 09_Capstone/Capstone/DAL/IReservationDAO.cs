using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface IReservationDAO
    {
        int CreateReservation(int siteChosen, string name, DateTime startDate, DateTime endDate);
        IList<Reservation> ViewReservations();
        bool IsInSeason(int siteChosen, DateTime startdate, DateTime endDate);
    }
}
