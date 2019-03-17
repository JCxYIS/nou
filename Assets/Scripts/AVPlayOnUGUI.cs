using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
 
[RequireComponent(typeof(VideoPlayer))]
public class AVPlayOnUGUI : MonoBehaviour
{
    private RenderTexture movie;
    private Image image;
    private RawImage rawImage;
    private VideoPlayer player;
    public UIMode UI;
    public enum UIMode
    {
        None = 0,
        Image = 1,
        RawImage = 2
    }
    // Use this for initialization
    void Start()
    {
        movie = new RenderTexture(512, 512, 24);
        player = GetComponent<VideoPlayer>();
        if (UI == UIMode.Image)
        {
            image = GetComponent<Image>();
            player.renderMode = VideoRenderMode.RenderTexture;
            player.targetTexture = movie;
        }
        else if(UI == UIMode.RawImage)
        {
            rawImage = GetComponent<RawImage>();
            player.renderMode = VideoRenderMode.APIOnly;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (UI == UIMode.Image)
        {
            //在Image上播放视频
            if (player.targetTexture == null) return;
            int width = player.targetTexture.width;
            int height = player.targetTexture.height;
            Texture2D t = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture.active = player.targetTexture;
            t.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            t.Apply();
            image.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f)) as Sprite;
        }
        if (UI == UIMode.RawImage)
        {
            //在RawImage上播放视频
            if (player.texture == null) return;
            rawImage.texture = player.texture;
        }
    }
}