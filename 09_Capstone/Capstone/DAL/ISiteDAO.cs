using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface ISiteDAO
    {
        IList<Site> ViewCampsites(int campgroundId, DateTime startDate, DateTime endDate);
        List<Site> AvailableReservations(int campgroundId, DateTime startDate, DateTime endDate);
        IList<Site> BonusAvailableReservations(DateTime startDate, DateTime endDate);
    }
}
