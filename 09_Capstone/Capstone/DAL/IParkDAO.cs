using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface IParkDAO
    {
        IList<Park> AvailableParks();
        IList<Park> ViewParkDetails(int userInput);
    }
}
