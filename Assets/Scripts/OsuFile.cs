using System.IO;
using UnityEngine;

[System.Serializable]
public class OsuFile
{
    public string path;
    public string dirPath {get {return Path.GetDirectoryName(path);} }

    public string AudioFilename;
    //AudioLeadIn: 500
    public int PreviewTime;//: 58046
    //Countdown: 0
    //SampleSet: Normal
    //StackLeniency: 0.7
    //Mode: 0
    //LetterboxInBreaks: 0

    //[Editor]
    //DistanceSpacing: 1.6
    //BeatDivisor: 4
    //GridSize: 64

    //[Metadata]
    public string Title;
    //TitleUnicode:Weed Circulation
    public string Artist;//:Hanazawa Kana ft. Snoop Dogg
    //ArtistUnicode:Hanazawa Kana ft. Snoop Dogg
    public string Creator;
    public string Version;//:and those roll the best
    //Source:
    //Tags:SapphireGhost Stefan those thenutritiousguy Bakemonogatari rap mash-up
    //BeatmapID:180337
    //BeatmapSetID:59936

    //[Difficulty]
    //HPDrainRate:6
    //CircleSize:4
    public float OverallDifficulty;//:8
    //ApproachRate:9
    //SliderMultiplier:2.4
    //SliderTickRate:1

    //[Events]
    ////Background and Video events
    //0,0,"New Canvas.png"
    //Video,0,"weed circulation.avi"
    ////Break Periods
    //2,26259,27459
    ////Storyboard Layer 0 (Background)
    ////Storyboard Layer 1 (Fail)
    ////Storyboard Layer 2 (Pass)
    ////Storyboard Layer 3 (Foreground)
    ////Storyboard Sound Samples
    ////Background Colour Transformations
    //3,100,0,0,0

    //[TimingPoints]
    //2059,500,4,1,0,3

    public string BGfileName;
    public string BGmovieFileName;




    /// <summary>
    /// 參數二選一即可
    /// </summary>
    /// <param name="osupath"></param>
    /// <param name="data"></param>
    public OsuFile(string osupath, string data = "")
    {
        path = osupath;
        if(data == "")
            data = File.ReadAllText(osupath);

        string[] dataLines = data.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        // Count all lines
        for(int l= 0; l < dataLines.Length; l++)
        {
            string str = dataLines[l];

#region METADATA
            string[] s = str.Split(':');
            //Debug.Log(s[0]);
            string content = "";
            for(int i = 1; i < s.Length; i++)
            {
                if(i > 1)
                    content += ":";
                content += s[i];
            }
            switch(s[0])
            {
                case "AudioFilename":
                    AudioFilename = content.TrimStart(' ');
                break;
                case "PreviewTime":
                    PreviewTime = int.Parse(content);
                break;
                case "Title":
                    Title = content;
                break;
                case "Artist":
                    Artist = content;
                break;
                case "Creator":
                    Creator = content;
                break;
                case "Version":
                    Version = content;
                break;
                case "OverallDifficulty":
                    //Debug.Log(content);
                    OverallDifficulty = float.Parse(content);
                break;
            }
#endregion

#region BG
            if(str == "//Background and Video events")
            {
                str = dataLines[l+1];
                s = str.Split('\"');
                foreach(var p in s)
                    if(p.Contains("."))
                        BGfileName = p;
                //next line: video
                str = dataLines[l+2];
                if(str.Contains("Video"))
                {
                    s = str.Split('\"');
                    foreach(var p in s)
                        if(p.Contains("."))
                            BGmovieFileName = p;
                }
            }
#endregion
        }
        
    }


}