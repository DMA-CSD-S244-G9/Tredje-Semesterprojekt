using InfiniteInfluence.DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;

public interface IAnnouncementDao
{
    /// <summary>
    /// Creates a new Announcement in the database and returns the generated AnnouncementId.
    /// </summary>
    int Create(Announcement announcement);


    /// <summary>
    /// Returns all of the announcements from the database.
    /// </summary>
    IEnumerable<Announcement> GetAll();
}