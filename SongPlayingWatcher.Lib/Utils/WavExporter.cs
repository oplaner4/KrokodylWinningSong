using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SongPlayingWatcher.Lib.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SongPlayingWatcher.Lib.Utils
{
    internal class WavExporter
    {
        private readonly string _file;
        private readonly bool _infiniteStream;

        internal WavExporter(string file, bool infiniteStream)
        {
            _file = file;
            _infiniteStream = infiniteStream;
        }

        internal void Export(string outputFile)
        {
            using (var mf = new MediaFoundationReader(_file))
            {
                WaveFileWriter.CreateWaveFile(outputFile,
                    new OffsetSampleProvider(mf.ToSampleProvider())
                    {
                        Take = TimeSpan.FromSeconds(_infiniteStream ? LibConstants.TakeSeconds : 0),
                        DelayBy = TimeSpan.FromSeconds(_infiniteStream ? 0 : LibConstants.TrimSeconds),
                    }.ToWaveProvider());
            }
        }


    }
}
