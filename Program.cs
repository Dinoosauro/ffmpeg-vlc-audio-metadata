using System.Runtime.InteropServices;
using Spectre.Console;

internal class Program
{
    static string library = "libopus";
    static string bitrate = "tobedecided";
    static string extension = "ogg";
    static string beforeFfmpeg = "";
    static string additionalinfo = " -map_metadata -1 -vn";


    private static void Main(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains("--codec"))
            {
                library = args[i + 1];
            }
            if (args[i].Contains("--bitrate"))
            {
                bitrate = args[i + 1];
            }
            if (args[i].Contains("--extension"))
            {
                extension = args[i + 1];
            }
            if (args[i].Contains("--additional"))
            {
                additionalinfo = additionalinfo + " " + args[i + 1];
            }
            if (args[i].Contains("--additionalcustom"))
            {
                additionalinfo = " " + args[i + 1];
            }
            if (args[i].Contains("--printversion"))
            {
                AnsiConsole.WriteLine("FfmpegAudioMetadata | 1.0.0");
                return;
            }
            if (args[i].Contains("--opensource"))
            {
                AnsiConsole.WriteLine("This application uses third-party libraries. You can see the licenses there: https://github.com/Dinoosauro/FfmpegAudioMetadata/blob/main/OpenSourceLicenses.md");
                return;
            }
            if (args[i].Contains("--license"))
            {
                AnsiConsole.WriteLine("You can find a copy of FfmpegAudioMetadata's license here: https://github.com/Dinoosauro/FfmpegAudioMetadata/blob/main/LICENSE");
                return;
            }
        }
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!AnsiConsole.Confirm("Is ffmpeg installed from your package manager (Y) or is in the same folder of this app (N)?"))
            {
                System.Diagnostics.Process.Start("/bin/bash", "-c \"chmod 777 ffmpeg\"");
                beforeFfmpeg = "./";
            }
        }
        applicationGo();
    }
    static void applicationGo()
    {
        AnsiConsole.Write(
    new FigletText("FfmpegAudioMetadata")
        .LeftAligned()
        .Color(Spectre.Console.Color.White));
        Console.WriteLine("Write the file you want to convert using " + library + ". If you want to convert every item of a folder, write --everything *fileextension*.");
        var file = Console.ReadLine();
        var getCommandType = "cmd";
        var commandIntro = "/c ";
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
        AnsiConsole.MarkupLine("[bold green]Conversion ended.[/]");
        if (AnsiConsole.Confirm("Do you want to convert another file?"))
        {
            applicationGo();
        }

        void convert(string getUserFile)
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
        if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {

            process = commandIntro + beforeFfmpeg + "ffmpeg -y -hide_banner -i '" + getUserFile + "' -acodec " + library + additionalinfo + " -b:a " + bitrate + "k '" + getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + "." + extension + "'\"";
        }
        else
        {
            process = commandIntro + beforeFfmpeg + "ffmpeg -y -hide_banner -i \"" + getUserFile + "\" -acodec " + library + additionalinfo + " -b:a " + bitrate + "k \"" + getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + "." + extension + "\"";
        }
        var fileName = getUserFile.Substring(0, getUserFile.LastIndexOf(".")).Replace("[", "(").Replace("]", ")");
        if (fileName.Contains(Path.DirectorySeparatorChar))
        {
            fileName = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }
        ctx.Status("Encoding " + fileName + "...");
        System.Diagnostics.Process.Start(getCommandType, process).WaitForExit();
        TagLib.File newFile = TagLib.File.Create(getUserFile.Substring(0, getUserFile.LastIndexOf(".")) + ".ogg");
        tag.Tag.CopyTo(newFile.Tag, true);
        newFile.Save();
    });
        }

    }
}