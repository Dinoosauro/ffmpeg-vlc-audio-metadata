using System.Runtime.InteropServices;
using Spectre.Console;

internal class Program
{
    static string library = "libopus";
    static string bitrate = "tobedecided";
    static string extension = "ogg";
    static string beforeFfmpeg = "";
    static string additionalinfo = " -map_metadata -1 -vn";
    static string getCommandType = "cmd";
    static string commandIntro = "/c ";
    static string possibleVlcPath = "\"%PROGRAMFILES%\\VideoLAN\\VLC\\vlc.exe\"";
    static bool useVlc = false;
    static int timeOut = 0;
    static int customTimeout = 120;

    private static void Main(string[] args)
    {
        var autoConvert = false;
        var newFilePath = "";
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--codec":
                    library = args[i + 1];
                    break;
                case "--bitrate":
                    bitrate = args[i + 1];
                    break;
                case "--extension":
                    extension = args[i + 1];
                    break;
                case "--additional":
                    additionalinfo = additionalinfo + " " + args[i + 1];
                    break;
                case "--additionalcustom":
                    additionalinfo = " " + args[i + 1];
                    break;
                case "--printversion":
                    AnsiConsole.WriteLine("ffmpeg-audio-metadata | 1.1.0");
                    return;
                case "--opensource":
                    AnsiConsole.WriteLine("This application uses third-party libraries. You can see the licenses there: https://github.com/Dinoosauro/ffmpeg-audio-metadata/blob/main/OpenSourceLicenses.md");
                    return;
                case "--license":
                    AnsiConsole.WriteLine("You can find a copy of ffmpeg-audio-metadata's license here: https://github.com/Dinoosauro/ffmpeg-audio-metadata/blob/main/LICENSE");
                    return;
                case "--fromfile":
                    autoConvert = true;
                    newFilePath = args[i + 1];
                    break;
                case "--everything":
                    autoConvert = true;
                    newFilePath = "--everything " + args[i + 1];
                    break;
                case "--vlc":
                    useVlc = true;
                    if (library == "libopus") library = "opus";
                    break;
                case "--customtimeout":
                    customTimeout = Convert.ToInt32(args[i + 1]);
                    break;
                case "--codecguide":
                    AnsiConsole.WriteLine("You can find a guide for the common audio codecs here: https://github.com/Dinoosauro/ffmpeg-audio-metadata/blob/main/EncodersNames.md");
                    break;

            }

        }
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !useVlc)
        {
            if (!AnsiConsole.Confirm("Is ffmpeg installed from your package manager (Y) or is in the same folder of this app (N)?"))
            {
                System.Diagnostics.Process.Start("/bin/bash", "-c \"chmod 777 ffmpeg\"");
                beforeFfmpeg = "./";
            }
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && possibleVlcPath == "\"%PROGRAMFILES%\\VideoLAN\\VLC\\vlc.exe\"") possibleVlcPath = "/Applications/VLC.app/Contents/MacOS/VLC";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && possibleVlcPath == "\"%PROGRAMFILES%\\VideoLAN\\VLC\\vlc.exe\"") possibleVlcPath = "vlc";

        if (autoConvert)
        {
            check(newFilePath);
            return;
        }

        applicationGo();
    }
    static void applicationGo()
    {
        AnsiConsole.Write(
    new FigletText("ffmpeg-audio-metadata")
        .LeftAligned()
        .Color(Spectre.Console.Color.White));
        var file = AnsiConsole.Ask<string>("Write the file you want to convert using " + library + ". If you want to convert every item of a folder, write --everything *fileextension*.");
        check(file);
        AnsiConsole.MarkupLine("[bold green]Conversion ended.[/]");
        if (AnsiConsole.Confirm("Do you want to convert another file?"))
        {
            applicationGo();
        }
    }
    static void check(string file)
    {
        if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            getCommandType = "/bin/bash";
            commandIntro = "-c \"";
        }
        if (!file.Contains("--everything"))
        {
            convert(file);
        }
        else
        {
            var directory = Directory.GetFiles(Directory.GetCurrentDirectory(), "*." + file.Replace("--everything ", ""));
            for (int i = 0; i < directory.Length; i++)
            {
                try
                {
                    convert(directory[i]);
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                }
            }
        }
    }

    static void convert(string getUserFile)
    {
        AnsiConsole.Status()
.Start("Preparing for conversion...", ctx =>
{
    ctx.Status("Getting file tag...");
    TagLib.File tag = TagLib.File.Create(getUserFile);
    if (bitrate == "tobedecided")
    {
        bitrate = Convert.ToString(tag.Properties.AudioBitrate);
    }
    if (extension == "m4a" || extension == "mp4" || extension == "aac")
    {
        additionalinfo = " -vcodec copy";
    }
    var process = "";
    if (bitrate.ToLower().Contains("k"))
    {
        bitrate = bitrate.ToLower().Substring(0, bitrate.ToLower().IndexOf("k"));
    }
    var vlcextension = extension;
    if (vlcextension == "m4a" || vlcextension == "aac") vlcextension = "mp4";
    if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
    {
        if (useVlc)
        {
            var newFileName = "tempNameForFfmpegAudioMetadata." + getUserFile.Substring(getUserFile.LastIndexOf(".") + 1);
            File.Move(getUserFile, System.IO.Path.GetFullPath(getUserFile).Substring(0, System.IO.Path.GetFullPath(getUserFile).LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + newFileName);
            process = commandIntro + possibleVlcPath + " -I dummy '" + newFileName + "' ':sout=#transcode{acodec=" + library + ",ab=" + bitrate + "}:std{dst='outputFfmpegAudioMetadata." + vlcextension + "',access=file}' vlc://quit";
        }
        else
        {
            process = commandIntro + beforeFfmpeg + "ffmpeg -y -hide_banner -i '" + getUserFile + "' -acodec " + library + additionalinfo + " -b:a " + bitrate + "k '" + getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + "." + extension + "'\"";
        }
    }
    else
    {
        if (useVlc)
        {
            var newFileName = "tempNameForFfmpegAudioMetadata." + getUserFile.Substring(getUserFile.LastIndexOf(".") + 1);
            File.Move(getUserFile, System.IO.Path.GetFullPath(getUserFile).Substring(0, System.IO.Path.GetFullPath(getUserFile).LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + newFileName);
            process = commandIntro + "\"" + possibleVlcPath + " -I dummy " + newFileName + " \":sout=#transcode{acodec=" + library + ",ab=" + bitrate + "}:std{dst=outputFfmpegAudioMetadata." + extension + ",access=file}\" vlc://quit\"";
        }
        else
        {
            process = commandIntro + beforeFfmpeg + "ffmpeg -y -hide_banner -i \"" + getUserFile + "\" -acodec " + library + additionalinfo + " -b:a " + bitrate + "k \"" + getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + "." + extension + "\"";
        }
    }
    var fileName = getUserFile.Substring(0, getUserFile.LastIndexOf(".")).Replace("[", "(").Replace("]", ")");
    if (fileName.Contains(Path.DirectorySeparatorChar))
    {
        fileName = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
    }
    // Sometimes VLC immediately exits, but it continues to convert the file.
    void loop()
    {
        try
        {
            FileStream stream = File.Open("outputFfmpegAudioMetadata." + vlcextension, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            stream.Dispose();
            File.Move("tempNameForFfmpegAudioMetadata." + getUserFile.Substring(getUserFile.LastIndexOf(".") + 1), System.IO.Path.GetFullPath(getUserFile).Substring(0, System.IO.Path.GetFullPath(getUserFile).LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + getUserFile);
            File.Move("outputFfmpegAudioMetadata." + vlcextension, fileName + "." + extension);
        }
        catch (Exception ex)
        {
            Thread.Sleep(1000);
            AnsiConsole.WriteException(ex);
            timeOut++;
            if (timeOut < customTimeout) loop(); else AnsiConsole.MarkupLine("[olive]Timeout reached.[/]");
        }
    }
    ctx.Status("Encoding " + fileName + "...");
    System.Diagnostics.Process.Start(getCommandType, process).WaitForExit();
    if (useVlc) loop();
    timeOut = 0;
    Thread.Sleep(500);
    TagLib.File newFile = TagLib.File.Create(getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + "." + extension);
    tag.Tag.CopyTo(newFile.Tag, true);
    newFile.Save();

});
    }

}