using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Audio
{
    public struct Audio
    {
        public int Id { get; set; }
        public byte[] AudioData { get; set; }
        public int Channels { get; set; }
        public int Bits { get; set; }
        public int Rate { get; set; }
        public string FileName { get; set; }

        public Audio (int id, byte[] data, int channels, int bits, int rate, string filename)
        {
            this.Id = id;
            this.AudioData = data;
            this.Channels = channels;
            this.Bits = bits;
            this.Rate = rate;
            this.FileName = filename;
        }

        public Audio(string filename) //for OGG using DragonOGG library
        {
            Id = -1;
            AudioData = null;
            Channels = -1;
            Bits = -1;
            Rate = -1;
            this.FileName = filename;
        }
        //public int Channels { get; set; }
        //public int Bits { get; set; }
        //public int Rate { get; set; }
        
        //public Audio (int channels, int bits, int rate)
        //{
        //    this.Channels = channels;
        //    this.Bits = bits;
        //    this.Rate = rate;
        //}

    }
}
