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
