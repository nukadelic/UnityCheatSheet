using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WebCamCanvasRender : MonoBehaviour
{
    public string path = "C:/temp/UnityScreenShots";

    public string fileNamePrefix = "webcam_";

    public int width = 320;
    public int height = 240;
    public int frameRate = 30;

    public bool setThisMatrialTexture = false;

    public static int fileCounter = 0;

    WebCamTexture webCamTexture;

    void Start()
    {
        CaptureWebCam();

        if(setThisMatrialTexture && GetComponent<Renderer>() != null)
            GetComponent<Renderer>().material.mainTexture = webCamTexture;

        CreateCanvasAndRawImage().texture = webCamTexture;
    }

    void CaptureWebCam()
    {
        var devices = WebCamTexture.devices;
        
        if (devices.Length < 1) throw new System.Exception("No webcams was found");
        
        var device = devices[0];
        
        webCamTexture = new WebCamTexture(device.name);
        webCamTexture.requestedFPS = frameRate;
        webCamTexture.requestedWidth = width;
        webCamTexture.requestedHeight = height;
        webCamTexture.Play();

        if (webCamTexture.width < 1 || webCamTexture.height < 1) throw new System.Exception("Invalid resolution");
    }

    RawImage CreateCanvasAndRawImage()
    {
        if (FindObjectOfType<EventSystem>() == null)

            new GameObject("EventSystem", typeof( EventSystem ) );

        GameObject canvasGO = new GameObject("WebCam Canvas", typeof(Canvas), typeof(CanvasScaler) );
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = FindObjectOfType<Camera>();

        var scalar = canvas.GetComponent<CanvasScaler>();

        scalar.referenceResolution = new Vector2( width, height );
        scalar.matchWidthOrHeight = 0;
        scalar.referencePixelsPerUnit = 100;
        scalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var blackGO = new GameObject("black", typeof(CanvasRenderer), typeof(RawImage), typeof(RectTransform));
        blackGO.transform.parent = canvasGO.transform;

        var max = Mathf.Max(width, height);

        var blackT = blackGO.GetComponent<RectTransform>();
        blackT.sizeDelta = new Vector2(max * 3, max * 3 );
        blackT.pivot = new Vector2(0.5f, 0.5f);
        blackT.anchoredPosition = new Vector2(0, 0);

        var blackImg = blackGO.GetComponent<RawImage>();
        blackImg.color = Color.black;
        blackImg.raycastTarget = false;

        var imageGO = new GameObject("rawImage", typeof(CanvasRenderer), typeof(RawImage), typeof(RectTransform));
        imageGO.transform.parent = canvasGO.transform;

        var imageT = imageGO.GetComponent<RectTransform>();
        imageT.sizeDelta = new Vector2(width, height);
        imageT.pivot = new Vector2(0.5f, 0.5f);
        imageT.anchoredPosition = new Vector2(0, 0);

        return imageGO.GetComponent<RawImage>();
    }

    public void SaveToPNG()
    {
        string zeros =
            ( fileCounter < 10000 ? "0000" : 
                ( fileCounter < 1000 ? "000" : 
                    (fileCounter < 100 ? "00" : 
                        (fileCounter < 10 ? "0" : ""))));

        File.WriteAllBytes(path + $"/{ fileNamePrefix + zeros + fileCounter }.png", ScreenshotCamera());

        fileCounter ++ ;
    }
    static byte[] ScreenshotCamera()
    {
        var size = new Vector2(Screen.width, Screen.height);
        Texture2D screenTex = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false, true);
        screenTex.ReadPixels(new Rect(Vector2.zero, size), 0, 0, false);
        screenTex.Apply();
        return screenTex.EncodeToPNG();
    }
}
