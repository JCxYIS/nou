using System.IO;
using UnityEngine;

public class OsuFile
{
    public string path;

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
    //OverallDifficulty:8
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

    public string BGpath;
    public string BGmoviePath;


    public OsuFile(string osupath)
    {
        path = osupath;
        StreamReader reader = new StreamReader(osupath);

        // Count all lines
        while (!reader.EndOfStream)
        {
            string[] s = reader.ReadLine().Split(':');
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
            }
        }
        reader.Close();

        foreach( var f in Directory.GetFiles(Path.GetDirectoryName(osupath)) )
        {
            string ext = Path.GetExtension(f);
            if(ext == ".png" || ext == ".jpg")
            {
                if(BGpath != null)
                    Debug.LogWarning("發現多的背景圖片！將覆蓋Old\nOld="+BGpath+"\nNew="+f);
                BGpath = f;
            }
            else if (ext == ".mp4" || ext == ".avi" || ext == ".mov")
            {
                if(BGmoviePath != null)
                    Debug.LogWarning("發現多的背景影片！將覆蓋Old\nOld="+BGmoviePath+"\nNew="+f);
                BGmoviePath = f;
            }
        }
    }


}