using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Checkers.GameApp.Helpers
{
    /// <summary>
    /// Ljudhanterar klass
    /// </summary>
    public static class Sound
    {
        private static SoundEffect? _winSound; // Referens till ljudet
        private static SoundEffectInstance? _winSoundInstance;

        /// <summary>
        /// Ladda in ljudresurset
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            _winSound = content.Load<SoundEffect>("win");
            _winSoundInstance = _winSound.CreateInstance();
        }

        /// <summary>
        /// Spela upp ljudet
        /// </summary>
        public static void PlayWinSound()
        {
            if(_winSoundInstance == null)
                return;

            _winSoundInstance.Pitch = 0.5f;
        }
    }
}
