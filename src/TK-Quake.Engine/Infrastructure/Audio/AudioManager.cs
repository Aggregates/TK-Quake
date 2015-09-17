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
using OpenTK;


//This is not yet a manager, just me using a tutorial and making sure I could get it working.
//If NotSupportedException's occur, the music file probably isn't following the strict format
namespace TKQuake.Engine.Infrastructure.Audio
{
    public class AudioManager : ResourceManager<Audio>, IDisposable
    {
<<<<<<< HEAD
       

        public AudioManager() : base()
        {
        }

        public void Add(string audioName, string filename)
        {
            Audio audio = BindAudio(filename);
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

        public void UpdateListenerPosition(Vector3 listenerPos)
        {
            AL.Listener(ALListener3f.Position, ref listenerPos);
        }

        public void PlayAsAmbient(String key)
        {
            var audio = this.Get(key);
            if (audio.FileName.Contains(".wav"))
            {
                PlayWavAsAmbient(audio);
            }
            else if (audio.FileName.Contains(".ogg"))
            {
                PlayOggAsAmbient(audio);
            }
        }

        public void PlayAtSource(String key, Vector3 sourcePos)
        {
            var audio = this.Get(key);
            if (audio.FileName.Contains(".wav"))
            {
                PlayWavAtSource(audio, sourcePos);
            }
            else if (audio.FileName.Contains(".ogg"))
            {
                throw new NotSupportedException(".ogg files cannot yet be played from a position, only as ambient.");
            }
        }

        private void PlayOggAsAmbient(Audio audioIn)
        {
            using (var vorbis = new NAudio.Vorbis.VorbisWaveReader(audioIn.FileName))
            using (var waveOut = new NAudio.Wave.WaveOut())
            {
                waveOut.Init(vorbis);
                waveOut.Play();

                while (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    Thread.Sleep(100);
                }

            }
        }

        private void PlayOggAtSource(Audio audioIn, Vector3 sourcePos)
        {

            //byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            using (var vorbis = new NAudio.Vorbis.VorbisWaveReader(audioIn.FileName))
            using (var waveOut = new NAudio.Wave.WaveOut())
            {
                waveOut.Init(vorbis);
                waveOut.Play();

                while (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    Thread.Sleep(100);
                }

            }
        }
        
        private void PlayWavAsAmbient(Audio audio)
        {
            audio = LoadWav(audio);
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

        //Audio file must be mono
        private void PlayWavAtSource(Audio audio, Vector3 sourcePos)
        {
            audio = LoadWav(audio);
            int source = AL.GenSource();
            //AL.Source(source, ALSourcei.Buffer, audio.Id);
            //AL.SourcePlay(source);

            AL.Source(source, ALSourcei.Buffer, audio.Id);
            AL.Source(source, ALSourcef.Pitch, 1.0f);
            AL.Source(source, ALSourcef.Gain, 0.85f);
            AL.Source(source, ALSource3f.Position, ref sourcePos);
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

        private Audio BindAudio(string filename)
        {
            if (filename.Contains(".wav"))
            {
                return BindWav(filename);
            }
            else if (filename.Contains(".ogg"))
            {
                return BindOgg(filename);
            }
            else throw new NotSupportedException("Audio format not supported.");
        }

        private Audio BindOgg(string filename)
        {
            return new Audio(filename);
        }

        private Audio BindWav(string filename)
        {
            return new Audio(filename);
        }

        //http://www.topherlee.com/software/pcm-tut-wavformat.html << .WAV header format
        private Audio LoadWav(Audio audio)
        {
            String filename = audio.FileName;
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
                if (signature != "RIFF")
                {
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }
                int riff_chunk_size = reader.ReadInt32();
                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                {
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }

                // Then the WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                {
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
                audio = new Audio(audioID, data, num_channels, bits_per_sample, sample_rate, filename);
                return audio;
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

