using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Audio
{
    public class Audio
    {
        public int Id { get; set; }
        public byte[] AudioData { get; set; }
        public int Channels { get; set; }
        public int Bits { get; set; }
        public int Rate { get; set; }
        public string FileName { get; set; }
        public bool Loop {
            get;
            set; }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="channels"></param>
        /// <param name="bits"></param>
        /// <param name="rate"></param>
        /// <param name="filename"></param>
        /// <param name="loop"></param>
        public Audio (int id, byte[] data, int channels, int bits, int rate, string filename, bool loop)
        {
            this.Id = id;
            this.AudioData = data;
            this.Channels = channels;
            this.Bits = bits;
            this.Rate = rate;
            this.FileName = filename;
            this.Loop = loop;
        }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="filename">Filepath to audio file</param>
        /// <param name="loop">Whether or not the audio file should repeat</param>
        public Audio(string filename, bool loop) 
        {
            Id = -1;
            AudioData = null;
            Channels = -1;
            Bits = -1;
            Rate = -1;
            this.FileName = filename;
            this.Loop = loop;
        }

    }
}
