using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HqRenderer : MonoBehaviour
{
    public RenderWindow Renderer;

    public Camera targetCamera;
    public Paralax[] paralaxes;
    public SpriteRenderer Plane;
    public SpriteRenderer FG;
    public int PPU;

    public TrackObject track;

    public Material grass1;
    public Material grass2;
    public Material rumble1;
    public Material rumble2;
    public Material road1;
    public Material road2;
    public Material dashline;

    public int screenWidthRef = 320;
    public int screenHeightRef = 240;
    public float cameraDepth = 0.84f; //camera depth [0..1]
    public int DravingDistance = 300; //segments
    public int quadCapacity = 4000;
    public int cameraHeight = 1500; //pixels?
    public float cameraOffset = 0;
    public float centrifugal = 0.1f;
    public bool drawRoad;
    public bool drawSprites;
    public int rumbleWidth;
    public float SpriteScale;

    [NonSerialized]
    int screenWidth2;
    [NonSerialized]
    int screenHeight2;
    [NonSerialized]
    Mesh[] combined;
    [NonSerialized]
    Dictionary<Material, Quad> dictionary = new Dictionary<Material, Quad>();
    [NonSerialized]
    private Material[] materials;

    //[NonSerialized]
    public int trip = 0; //pixels
    [NonSerialized]
    float playerX = 0;
    [NonSerialized]
    float playerY = 0;
    [NonSerialized]
    float playerZ = 0;

    [NonSerialized]
    private int startPos;
    [NonSerialized]
    private int playerPos;

    [NonSerialized]
    Quad[] quad;
    [NonSerialized]
    private RenderTexture _renderTexture;
    [NonSerialized]
    private float speed;
    [NonSerialized]
    private float prevTrip;
    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Awake();
        }
#endif
        Camera.onPostRender += PostRender;
    }
    private void OnDisable()
    {
        Camera.onPostRender -= PostRender;
    }

    void Awake()
    {
        Renderer = new RenderWindow();

        Texture2D tex1 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex1.filterMode = FilterMode.Point;
        FG.sprite = Sprite.Create(tex1, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f,0.5f), PPU);
        FG.sprite.name = "runtimeFG";
        
        Texture2D tex2 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex2.filterMode = FilterMode.Point;
        Plane.sprite = Sprite.Create(tex2, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f, 0.5f), PPU);
        Plane.sprite.name = "runtimePlane";

        foreach (var p in paralaxes)
        {
            Texture2D tex3 = new Texture2D(p.background.texture.width,  p.background.texture.height, TextureFormat.RGBA32, false);
            tex2.filterMode = FilterMode.Point;
            p.spriteRenderer.sprite = Sprite.Create(tex3, p.background.rect, new Vector2(0.5f, 0.5f), PPU);
            p.spriteRenderer.sprite.name = "runtimeBG";
        }
        
        quad = new Quad[] {
            new Quad(quadCapacity), 
            new Quad(quadCapacity), 
            new Quad(quadCapacity),
            new Quad(quadCapacity),
            new Quad(quadCapacity), 
            new Quad(quadCapacity),
            new Quad(quadCapacity)
        };
        combined = new Mesh[] { new Mesh(), new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh()};
        dictionary = new Dictionary<Material, Quad>()
        {
            { grass1, quad[0]},
            { grass2, quad[1]},
            { rumble1, quad[2]},
            { rumble2, quad[3]},
            { road1, quad[4]},
            { road2, quad[5]},
            { dashline, quad[6]},
        };
        materials = new Material[] { grass1, grass2, rumble1, rumble2, road1, road2, dashline };
    }
    public void drawSprite(ref Line line)
    {
        if (line.Y < -screenHeight2) { return; }

        Sprite s = line.sprite;
        Sprite s2 = line.spriteCar;

        if (s != null) RenderSprite(s, line, line.spriteX, true);
        if (s2 != null) RenderSprite(s2, line, line.spriteXCar, false);
        
    }

    void RenderSprite (Sprite s, Line line, float spriteX, bool flipable) {
        var w = s.rect.width;
        var h = s.rect.height;

        float destX = line.X + line.W * spriteX + screenWidth2;
        float destY = -line.Y + screenHeight2;
        float destW = w * line.scale * screenWidth2 * SpriteScale;
        float destH = h * line.scale * screenWidth2 * SpriteScale;

        destX += destW * Mathf.Sign(spriteX) / 2; //offsetX
        destY += destH * (-1);    //offsetY

        float clipH = -line.Y + line.clip;
        if (clipH < 0) clipH = 0;

        if (clipH >= destH) return;

        Rect target = new Rect(destX, destY, destW, destH);
        Rect source = new Rect(Vector2Int.zero, new Vector2(1, 1 - clipH / destH));
        Renderer.draw(source, s, target, flipable && line.flipX);
    }
    private void addQuad(Material c, float x1, float y1, float w1, float x2, float y2, float w2, float z)
    {
        dictionary[c].SetQuad(x1 / PPU, y1 / PPU, w1 / PPU, x2 / PPU, y2 / PPU, w2 / PPU, z);
    }

    private void DrawObjects()
    {
        ////////draw objects////////
        if (drawSprites)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            for (int n = startPos + DravingDistance; n > startPos; n--)
            {
                drawSprite(ref track.lines[n % track.Length]);
            }
            Graphics.CopyTexture(_renderTexture, FG.sprite.texture);
            //Revert the matrix and active render texture.
            GL.PopMatrix();
            RenderTexture.active = currentActiveRT;
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
    private void DrawRoad()
    {
        if (drawRoad)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            Graphics.SetRenderTarget(_renderTexture);
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0, 0));
            GL.PushMatrix();
            float refH = targetCamera.orthographicSize * PPU * 2;
            float refHScale = refH / screenHeightRef;
            float HScale = ((float)screenHeightRef) / targetCamera.pixelHeight;
            float unscaledAspectRation = (HScale * targetCamera.pixelWidth) / screenWidthRef;

            var m = Matrix4x4.Scale(new Vector3(unscaledAspectRation * refHScale, refHScale, 1));

            int i = 0;
            foreach (var material in materials)
            {
                Renderer.draw(dictionary[material].ToMesh(combined[i++]), material, m);
            }
            Graphics.CopyTexture(_renderTexture, Plane.sprite.texture);
            GL.PopMatrix();
            Graphics.SetRenderTarget(currentActiveRT);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }

    private void DrawBackground()
    {
        

        foreach (var p in paralaxes) {
            //Good enough
            _renderTexture = RenderTexture.GetTemporary(p.spriteRenderer.sprite.texture.width, p.spriteRenderer.sprite.texture.height, 0, p.spriteRenderer.sprite.texture.graphicsFormat);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);
            p.bgOffset += new Vector2(p.speed/PPU * speed * Time.deltaTime * track.lines[playerPos].curve, 0);

            Graphics.Blit(p.background.texture, _renderTexture, Vector2.one, p.bgOffset, 0, 0);

            Graphics.CopyTexture(_renderTexture, p.spriteRenderer.sprite.texture);

            GL.PopMatrix();
            Graphics.SetRenderTarget(currentActiveRT);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }

    private void PostRender(Camera cam)
    {
        DrawRoad();
    }

    private void FixedUpdate()
    {
        CalculateProjection();
    }
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            CalculateProjection();
        }
#endif
        DrawBackground();
        DrawObjects();
    }

    void CalculateProjection()
    {
        speed = trip - prevTrip;
        prevTrip = trip;
        startPos = trip / track.segmentLength;
        playerZ = trip + cameraHeight * cameraDepth; // car is in front of cammera
        playerPos = (int)(playerZ / track.segmentLength) % track.lines.Length;
        playerY = track.lines[playerPos].y;
        int camH = (int)(playerY + cameraHeight);
        playerX = cameraOffset;
        screenWidth2 = screenWidthRef / 2;
        screenHeight2 = screenHeightRef / 2;

        float maxy = -screenHeight2;
        int counter = 0;
        float x = 0, dx = 0;
        float res = 1f / PPU;

        foreach (var q in quad) { q.Clear(); }
        foreach (var m in combined) { m.Clear(); }
        ///////draw road////////
        for (int n = startPos + 1; n < startPos + DravingDistance; n++)
        {
            ref Line l = ref track.lines[n % track.Length];
            l.project(
                (int)(playerX * track.roadWidth - x),
                camH,
                startPos * track.segmentLength - (n >= track.Length ? track.Length * track.segmentLength : 0),
                screenWidth2,
                screenHeight2,
                cameraDepth);
            x += dx;
            dx += l.curve;

            l.clip = maxy;
            if (l.Y <= maxy)
            {
                continue;
            }
            maxy = l.Y;

            Material grass = (n / 3 / 3) % 2 == 0 ? grass1 : grass2;
            Material rumble = (n / 3) % 2 == 0 ? rumble1 : rumble2;
            Material road = (n / 3 / 2) % 2 == 0 ? road1 : road2;

            ref Line p = ref track.lines[(n - 1) % track.Length]; //previous line

            if (Mathf.Abs(l.Y - p.Y) < res)
            {
                continue;
            }

            var z = (float)(n - startPos) / DravingDistance;

            addQuad(grass, 0, p.Y, screenWidth2, 0, l.Y, screenWidth2, z);
            addQuad(rumble, p.X, p.Y, p.W + p.scale * rumbleWidth * screenWidth2, l.X, l.Y, l.W + l.scale * rumbleWidth * screenWidth2, z);
            addQuad(road, p.X, p.Y, p.W, l.X, l.Y, l.W, z);
            
            float offset = track.roadWidth / 300f;
            var p_rumbleWidth = p.W + p.scale * rumbleWidth * screenWidth2 * - offset;
            var l_rumbleWidth = l.W + l.scale * rumbleWidth * screenWidth2 * - offset;
            //addQuad(grass, p.X - screenWidth2 - p_rumbleWidth, p.Y, screenWidth2, l.X - screenWidth2 - l_rumbleWidth, l.Y, screenWidth2, z);
            //addQuad(grass, p.X + screenWidth2 + p_rumbleWidth, p.Y, screenWidth2, l.X + screenWidth2 + l_rumbleWidth, l.Y, screenWidth2, z);
            //addQuad(rumble, p.X - p_rumbleWidth, p.Y, p.scale* rumbleWidth* screenWidth2, l.X - l_rumbleWidth, l.Y, l.scale*screenWidth2 * rumbleWidth, z);
            //addQuad(rumble, p.X + p_rumbleWidth, p.Y, p.scale* rumbleWidth* screenWidth2, l.X + l_rumbleWidth, l.Y, l.scale*screenWidth2 * rumbleWidth, z);

            addQuad(road, p.X, p.Y, p.W, l.X, l.Y, l.W, z);
            if ((n / 3) % 2 == 0)
            {
                //addQuad(dashline, p.X, p.Y * 1.1f, p.W * 0.05f, l.X, l.Y * 1.1f, l.W * 0.05f, z);                

                addQuad(dashline, p.X + p_rumbleWidth, p.Y * 1.1f, p.W * 0.05f, l.X + l_rumbleWidth, l.Y * 1.1f, l.W * 0.05f, z);
                addQuad(dashline, p.X - p_rumbleWidth, p.Y * 1.1f, p.W * 0.05f, l.X - l_rumbleWidth, l.Y * 1.1f, l.W * 0.05f, z);
            }

            counter++;
        }
    }
}
[System.Serializable]
public class Paralax {
    public Sprite background;
    public SpriteRenderer spriteRenderer;
    public float speed;
    [HideInInspector]
    public Vector2 bgOffset;

}
