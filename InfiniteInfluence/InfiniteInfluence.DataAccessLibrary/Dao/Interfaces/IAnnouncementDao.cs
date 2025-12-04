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
    /// Returns all of the announcements from the database or an empty list of announcements if none were available
    /// </summary>
    IEnumerable<Announcement> GetAll();


    /// <summary>
    /// Returns one single announcement based on its id or returns null if no announcement was found
    /// </summary>
    Announcement? GetOne(int announcementId);


    /// <summary>
    /// Returns true if an influencer application was added to the announcement by inserting a row into the InfluencerAnnouncements table
    /// </summary>
    bool AddInfluencerApplication(int announcementId, int influencerUserId);

    /// <summary>
    /// Deletes an announcement from the database tables based on its AnnouncementId and returns true if the announcement was deleted elsewise returns false
    /// <returns></returns>
    bool Delete(int announcementId);
}