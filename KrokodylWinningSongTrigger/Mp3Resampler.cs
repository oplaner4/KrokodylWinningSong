using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;

namespace KrokodylWinningSongTrigger
{
    internal class Mp3Resampler
    {
        private readonly string _file;
        private readonly bool _infiniteStream;

        internal Mp3Resampler(string file, bool infiniteStream)
        {
            _file = file;
            _infiniteStream = infiniteStream;
        }

        internal byte[] GetWavBytes()
        {
            using (var mf = new MediaFoundationReader(_file))
            {
                var outFormat = new WaveFormat(Constants.ResampleRate, mf.WaveFormat.Channels);

                using (var resampler = new MediaFoundationResampler(mf, outFormat))
                {
                    var wavStream = new MemoryStream();
                    WaveFileWriter.WriteWavFileToStream(wavStream,
                        new OffsetSampleProvider(resampler.ToSampleProvider())
                        {
                            Take = TimeSpan.FromSeconds(_infiniteStream ? Constants.TakeSeconds : 0),
                            DelayBy = TimeSpan.FromSeconds(_infiniteStream ? 0 : Constants.TrimSeconds),
                        }.ToWaveProvider());
                    return wavStream.ToArray();
                }
            }
        }
    }
}
