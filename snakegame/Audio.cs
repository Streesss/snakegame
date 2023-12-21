using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace snakegame
{
    internal class Audio
    {
        private readonly static MediaPlayer GameOver = LoadAudio("game-over.wav");
        private static MediaPlayer LoadAudio(string filename)
        {
            MediaPlayer player = new();
            player.Open(new Uri($"Assets/{filename}", UriKind.Relative));
            return player;
        }
    }
}
