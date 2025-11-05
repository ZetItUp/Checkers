using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens.Interfaces
{
    ///<summary>
    /// Interface för att byta screen
    /// </summary>
    public interface IScreenChanger
    {
        void ChangeScreen(ScreenID screenID);
    }
}
