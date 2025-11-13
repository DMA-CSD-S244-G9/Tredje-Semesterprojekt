using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInfluence.DataAccessLibrary.Model;


public abstract class BaseUser
{
    #region Properties
    public int UserId { get; set; }
    public string LoginEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; 
    #endregion
}