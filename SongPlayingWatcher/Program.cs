using SoundFingerprinting.Audio;
using SoundFingerprinting;
using SoundFingerprinting.Builder;
using SoundFingerprinting.InMemory;
using SoundFingerprinting.Data;
using SongPlayingWatcher;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using SongPlayingWatcher.Lib;
using SongPlayingWatcher.Configuration;
using System.Threading;
using System.Diagnostics;
using SongPlayingWatcher.Lib.Configuration;

var settings = Settings.GetFromEnvArgs();

if (settings.SongToBePlayed == null || settings.ShowUsageMessage)
{
    Environment.ExitCode = settings.ShowUsageMessage ? 0 : 1;
    Console.WriteLine("Usage:");
    Console.WriteLine();
    Console.WriteLine("<sourceSong> [-h|help] [-f|forever] [-u <realtimeStreamUri>]");
    Console.WriteLine("\t\t [-r|resampled] [-s <resampledSongName>] [-p <periods>] [-e <execName> <execArguments>]");
    Console.WriteLine();
    Console.WriteLine("Settings:");
    Console.WriteLine();
    Console.WriteLine("\t<sourceSong> \t\t\t Song which should be playing on a real time stream.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\t-h|help \t\t\t Show this usage message.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\t-f|forever \t\t\t Periodically check forever. Implicitely just today.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\t-r|resampled \t\t The source song is already resampled.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\t-s <resampledSongName> \t\t Save resampled song into the defined file.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine($"\t-u <realtimeStreamUri> \t\t Realtime stream to use. Implicitely {LibConstants.KrokodylMp3SourceUri}.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine($"\t-p <periods> \t\t\t Minimum count of periods for song to be considered playing. Implicitely {Settings.DefaultMinPeriodsPlaying}.");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\t-e <execName> <execArguments> \t Start executable with arguments when the song is considered playing. Use \"\" for no arguments.");
    Console.WriteLine();
    return;
}

Console.WriteLine($"Loading song \"{Path.GetFileName(settings.SongToBePlayed)}\"...");
SongPreprocessor utility = settings.IsResampled ?
    new(settings.SongToBePlayed, settings.IsResampled) :
    new(settings.SongToBePlayed, settings.ResambledSongName);

Console.WriteLine("Preparing song for fingerprinting...");
await utility.Prepare();

Console.WriteLine($"Periodically checking if the song is currently playing on {settings.RealtimeStreamUri}...");

var checkPlaying = new CheckPlaying(utility.ResampledSongName, settings.RealtimeStreamUri);

checkPlaying.Events.OnListening = () => {
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.Write("Listening...");
    Console.ResetColor();
};

checkPlaying.Events.OnChecked = (_) => {
    Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
    Console.Write($"{DateTime.Now:hh:mm:ss} PM: ");
    return true;
};

checkPlaying.Events.OnPlaying = (playingPeriods) => {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Song is playing for {playingPeriods} period(s).");
    Console.ResetColor();

    if (playingPeriods == settings.MinPeriodsPlaying && settings.StartExecName != null)
    {
        Process.Start(settings.StartExecName, settings.StartExecArguments);
    }
};

checkPlaying.Events.OnNotPlaying = () => {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Song is not playing.");
    Console.ResetColor();
};

await checkPlaying.PeriodicallyCheck(settings.CheckForever);