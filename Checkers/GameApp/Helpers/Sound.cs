using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Checkers.GameApp.Helpers
{
    public static class Sound
    {
        private static SoundEffect? _winSound; // referensen till ljudet

        // ladda ljudfil
        public static void LoadContent(ContentManager content)
        {
            _winSound = content.Load<SoundEffect>("win");
        }
        public static void PlayWinSound()
        {
            _winSound?.Play();
        }
    }
}
