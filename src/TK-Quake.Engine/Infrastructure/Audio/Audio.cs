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

        public Audio (int id, byte[] data, int channels, int bits, int rate)
        {
            this.Id = id;
            this.AudioData = data;
            this.Channels = channels;
            this.Bits = bits;
            this.Rate = rate;
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
