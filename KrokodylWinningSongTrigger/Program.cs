using SoundFingerprinting.Audio;
using SoundFingerprinting;
using SoundFingerprinting.Builder;
using SoundFingerprinting.InMemory;
using SoundFingerprinting.Data;
using KrokodylWinningSongTrigger;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

Console.ForegroundColor = ConsoleColor.White;

IEnumerator<string> argsEnumerator = Environment.GetCommandLineArgs().AsEnumerable().GetEnumerator();

bool keepResambled = false;
bool help = false;
string songToBePlayed = null;

argsEnumerator.MoveNext();

while (argsEnumerator.MoveNext())
{
    if ((argsEnumerator.Current == "-k") || (argsEnumerator.Current == "keep"))
    {
        keepResambled = true;
    }
    else if ((argsEnumerator.Current == "-h") || (argsEnumerator.Current == "help"))
    {
        help = true;
        break;
    }
    else
    {
        songToBePlayed = argsEnumerator.Current;
    }
}

if (songToBePlayed == null || help)
{
    Environment.ExitCode = help ? 0 : 1;
    Console.WriteLine($"Usage:");
    Console.WriteLine($"<source_song> [-k|keep]");
    Console.WriteLine($"\t\t <source_song> \t Song to be played.");
    Console.WriteLine($"\t\t -k|keep \t Do not delete resambled song file.");
    Console.WriteLine($"\t\t -h|help \t Show this usage message.");
    return;
}

var utility = new SongPrepareUtility(songToBePlayed);
Console.WriteLine("Preparing song for fingerprinting...");
await utility.Prepare();

AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) => {
    File.Delete(Constants.KrokodylWavName);
    if (!keepResambled)
    {
        File.Delete(utility.ResampledWav);
    }
};

Console.WriteLine("");
Console.WriteLine("Periodically checking if the song is currently playing.");

await new SongCheckPlayingTrigger(utility.ResampledWav).PeriodicallyCheck((isPlaying) =>
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"{DateTime.Now:hh:mm:ss} PM: ");
    Console.ForegroundColor = isPlaying ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine($"Song is {(isPlaying ? "playing" : "not playing")}");
});