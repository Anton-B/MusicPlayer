using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerAPI
{
    public enum UpdateBehavior
    {
        NoUpdate = 0,
        UpdateNavigationList = 1,
        UpdateSongsList = 2,
        UpdateAll = 3,
        DeleteSongFromList = 4,
        DeleteSongAndUpdateAll = 5
    }
}
