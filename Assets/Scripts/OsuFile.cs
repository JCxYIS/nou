using System.IO;
using UnityEngine;

public class OsuFile
{
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

    public OsuFile()
    {
        
    }

    public OsuFile(string path)
    {
        OsuFile output = new OsuFile();
        StreamReader reader = new StreamReader(path);


        // Count all lines
        while (!reader.EndOfStream)
        {
            string[] s = reader.ReadLine().Split(':');
            Debug.Log(s[0]);
            switch(s[0])
            {
                case "AudioFilename":
                break;
            }
        }

        reader.Close();
    }
}