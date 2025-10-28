using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens.Interfaces
{
    // Interface för att byta screen
    public interface IScreenChanger
    {
        void ChangeScreen(ScreenID screenID);
    }
}
