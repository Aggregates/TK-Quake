using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Audio.OpenAL;
using OpenTK.Audio;
using System.Threading;
using TKQuake.Engine.Infrastructure.Abstract;


//This is not yet a manager, just me using a tutorial and making sure I could get it working.
//If NotSupportedException's occur, the music file probably isn't following the strict format
namespace TKQuake.Engine.Infrastructure.Audio
{
    public class AudioManager : ResourceManager<Audio>, IDisposable
    {
        public AudioManager() : base()
        {
        }

        public void Add(string audioName, string filename)
        {
            Audio audio = LoadAudio(filename);
            base.Add(audioName, audio);
        }

        public override void Add(string key, Audio data)
        {
            base.Add(key, data);
        }

        public override Audio Get(string key)
        {
            return base.Get(key);
        }

        public override bool Registered(string key)
        {
            return base.Registered(key);
        }

        public override void Remove(string key)
        {
            Audio aud = Get(key);
            base.Remove(key);
            AL.DeleteBuffer(aud.Id);
        }


        //Source probably needs to be passed in
        public void Play(String key)
        {
            var audio = this.Get(key);
            int source = AL.GenSource();
            AL.Source(source, ALSourcei.Buffer, audio.Id);
            AL.SourcePlay(source);
            int state;

            do
            {
                Thread.Sleep(1000);
                Console.Write(".");
                AL.GetSource(source, ALGetSourcei.SourceState, out state);
            }
            while ((ALSourceState)state == ALSourceState.Playing);
            AL.SourceStop(source);
            AL.DeleteSource(source);
        }

        //http://www.topherlee.com/software/pcm-tut-wavformat.html << .WAV header format
        private Audio LoadAudio(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new ArgumentException(filename);
            }

            int audioID = AL.GenBuffer();

            Stream stream = File.Open(filename, FileMode.Open);
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (BinaryReader reader = new BinaryReader(stream))
            {
                
                // First comes the RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF") { 
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }
                int riff_chunk_size = reader.ReadInt32();
                string format = new string(reader.ReadChars(4));
                if (format != "WAVE") { 
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }

                // Then the WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ") { 
                    throw new NotSupportedException("Specified wave file is not supported.");
                }

                //Info about how to output the data
                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                //What follows is some hardcoded junk to try and deal with different wave header formats
                if (format_chunk_size == 18)
                {
                    reader.ReadInt16();
                }
            
                string data_signature = new string(reader.ReadChars(4));
                if (data_signature == "LIST")
                {
                    Console.WriteLine(reader.BaseStream.Position);
                    int list_size = reader.ReadInt16();
                    reader.ReadChars(2); //INFO tag
                    reader.ReadChars(list_size);
                    Console.WriteLine(reader.BaseStream.Position);
                    data_signature = new string(reader.ReadChars(4));
                }

                if (data_signature != "data") //Signifying start of data block
                {
                    Console.WriteLine(data_signature);
                    throw new NotSupportedException("Specified wave file is not supported.");
                }
                //More info about the data
                int data_chunk_size = reader.ReadInt32();

                //Finally, the data itself goes from here until EOF
                byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

                //Loading and storing the data
                AL.BufferData(audioID, GetSoundFormat(num_channels, bits_per_sample), data, data.Length, sample_rate);
                return new Audio(audioID, data , num_channels, bits_per_sample, sample_rate);
            }
        }

        public void printHeader(String filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            using (BinaryReader reader = new BinaryReader(stream))
            {
                String s = new string(reader.ReadChars(80));
                Console.WriteLine(s);
            }
        }

        private ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

