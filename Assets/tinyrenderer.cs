using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class tinyrenderer : MonoBehaviour
{
    public Vector3 lightDir;
    private Texture2D texture2D = null;
    public RawImage rawImag = null;
    public Mesh mesh = null;
    private int width, height;
    private List<int[]> faces = null;
    public Material material = null;
    float[] zBuffer = null;
    // Start is called before the first frame update
    void Start()
    {
        width = (int)rawImag.rectTransform.rect.width;
        height = (int)rawImag.rectTransform.rect.height;
        texture2D = new Texture2D((int)rawImag.rectTransform.rect.width, (int)rawImag.rectTransform.rect.height, TextureFormat.RGB24, false);
        rawImag.texture = texture2D;
        texture2D.filterMode = FilterMode.Point;
        InitBuffer();
        //  //  DrawMesh(mesh);
        //  Vector2Int[] t0 = new Vector2Int[]
        //  { new Vector2Int(10,70),new Vector2Int(50,160),new Vector2Int(70,80) };
        //  Vector2Int[] t1 = new Vector2Int[]
        //{ new Vector2Int(180,50),new Vector2Int(150,1),new Vector2Int(70,180) };
        //  Vector2Int[] t2  = new Vector2Int[]
        //{ new Vector2Int(180,180),new Vector2Int(120,160),new Vector2Int(130,180) };

        //  //DrawTriangles(t0[0], t0[1], t0[2],Color.red,texture2D);
        //  //DrawTriangles(t1[0], t1[1], t1[2], Color.green, texture2D);
        //  //DrawTriangles(t2[0], t2[1], t2[2], Color.red, texture2D);

        //  //  DrawTriangles(t0,Color.white,texture2D);
          lightDir = new Vector3(0f, 0f, -1f);
        DrawMesh(mesh, lightDir);
     
       // Test();
    }
    void InitBuffer()
    {
        zBuffer = new float[width * height];
        for(int i=0;i<zBuffer.Length;i++)
        {
            zBuffer[i] = float.MinValue;
        }
    }
     void Test()
    {
        int[] yBuffer = new int[width];
        for(int i=0;i< width; i++)
        {
            yBuffer[i] = int.MinValue;

        }
        Rasterize(new Vector2Int(20,34),new Vector2Int(744,400),texture2D, Color.red, yBuffer);
        Rasterize(new Vector2Int(120, 434), new Vector2Int(444, 400), texture2D, Color.green, yBuffer);
        Rasterize(new Vector2Int(330, 463), new Vector2Int(594, 200), texture2D, Color.blue, yBuffer);
    }
    void Rasterize(Vector2Int P0,Vector2Int P1,Texture2D tex, Color color ,int[] yBuffer)
    {
        if(P0.x>P1.x)
        {
           
            Swap(ref P0,ref P1);
        }
        for(int x=P0.x;x<P1.x;x++)
        {
            float t = (float)(x - P0.x) / (P1.x - P0.x);
            int y = (int)((P0.y * (1 - t)) + (P1.y * t));
            if (yBuffer[x]<y)
            {
                yBuffer[x] = y;
                tex.SetPixel(x, 0, color);
            }
            
        }
        tex.Apply();
    //    material.SetTexture("_MainTex", tex);
    }
    void Bresenham01(Vector2Int startPoint, Vector2Int endPoint,Color color,Texture2D tex)
    {
        for(float t=0;t<1;t+=0.01f)
        {
            int x = (int)(startPoint.x + (endPoint.x - startPoint.x) * t);
            int y = (int)(startPoint.y + (endPoint.y - startPoint.y) * t);
            tex.SetPixel(x,y,color);
        }
       
        texture2D.Apply();
    }
    void Swap<T>(ref T a, ref T b) 
    {
        T temp = a;
        a = b;
        b = temp;
    }
    
    void DrawLine(int startX, int startY, int  endX,int endY, Color color, Texture2D tex)
    {
        bool steep = false;
        if(Mathf.Abs(startX-endX)<Mathf.Abs(startY-endY))
        {
            Swap(ref startX, ref startY);
            Swap(ref endX, ref endY);
            steep = true;
        }
        if(startX>endX)
        {
            Swap(ref startX, ref endX);
            Swap(ref startY, ref endY);
        }
        int dx=endX-startX;
        int dy=endY-startY;
        int derror2 = Mathf.Abs(dy) * 2;
        int error2 = 0;
        int y = startY;
        for(int x= startX; x< endX;x++)
        {
           float t= (float)(x - startX) /(endX - startX);
         
            if(steep)
            {
                texture2D.SetPixel(y, x, color);

            }
            else
            {
                texture2D.SetPixel(x, y, color);
            }
            error2 += derror2;
            if(error2>dx)
            {
                y += (endY >startY ? 1 : -1);
                error2 -= dx * 2;
            }
        }
       
     
    }
   
    void DrawTriangles(Vector3[] vertexs, Color color, Texture2D texture2D)
    {
        Vector3 minBox=new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxBox = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for(int i=0;i< vertexs.Length;i++)
        {
            if (minBox.x > vertexs[i].x)
            {
                minBox.x = vertexs[i].x;
            }
            if (minBox.y > vertexs[i].y)
            {
                minBox.y = vertexs[i].y;
            }
            if (maxBox.x <vertexs[i].x)
            {
                maxBox.x = vertexs[i].x;
            }
            if (maxBox.y < vertexs[i].y)
            {
                maxBox.y = vertexs[i].y;
            }
        }
     
   
          for(float x =minBox.x;x<maxBox.x;x++)
            {
                for (float y = minBox.y; y < maxBox.y; y++)
                {
                    Vector3 P = new Vector3(x, y,0);
               Vector3 bc_screen  = Barycentric(P, vertexs[0], vertexs[1], vertexs[2]);
                if (bc_screen.x < 0 || bc_screen.y < 0 || bc_screen.z < 0)
                    continue;
                P.z = 0;
                P.z += vertexs[0].z * bc_screen.x;
                P.z += vertexs[1].z * bc_screen.y;
                P.z += vertexs[2].z * bc_screen.z;
                 int zIndex= ((int)P.x + ((int)P.y * width));

                if (zIndex == 640399)
                {
                    Debug.Log(zIndex);
                }
                if (zBuffer[zIndex] < P.z)
                {
                    zBuffer[zIndex] = P.z;
                    texture2D.SetPixel((int)P.x, (int)P.y, color);
                }
            }
        }
        texture2D.Apply();
    }
    Vector3 Barycentric(Vector3 P, Vector3 P0, Vector3 P1, Vector3 P2)
    {

        Vector3 u = Vector3.Cross(new Vector3(P2.x - P0.x, P1.x - P0.x, P0.x - P.x),
            new Vector3(P2.y - P0.y, P1.y - P0.y, P0.y - P.y));
        if (Mathf.Abs(u.z) > 1e-2)
            return new Vector3(1.0f - (u.x + u.y) / u.z, u.y / u.z, u.x / u.z);

        return new Vector3(-1, -1, -1);


     }
    //    Vector3 Barycentric(Vector3 P, Vector3 P0, Vector3 P1, Vector3 P2)
    //{
    //    Vector2Int p = new Vector2Int((int)P.x, (int)P.y);
    //    Vector2Int p0 = new Vector2Int((int)P0.x, (int)P0.y);
    //    Vector2Int p1 = new Vector2Int((int)P1.x, (int)P1.y);
    //    Vector2Int p2 = new Vector2Int((int)P2.x, (int)P2.y);
    //    Vector3 u = Vector3.Cross(new Vector3(p2.x - p0.x, p1.x - p0.x, p0.x - p.x),
    //        new Vector3(p2.y - p0.y, p1.y - p0.y, p0.y - p.y));
    //    if (Mathf.Abs(u.z) < 1)
    //        return new Vector3(-1, -1, -1);

    //    return new Vector3(1.0f - (u.x + u.y) / u.z, u.y / u.z, u.x / u.z);
    //}
    //void DrawTriangles(Vector2Int v0,Vector2Int v1,Vector2Int v2,Color color,Texture2D texture2D)
    //{

    //    if (v0.y > v1.y) { Swap<Vector2Int>(ref v0,ref v1); }
    //    if (v0.y > v2.y) { Swap<Vector2Int>(ref v0, ref v2); }
    //    if (v1.y > v2.y) { Swap<Vector2Int>(ref v1, ref v2); }
    //    int totalHeight = v2.y - v0.y;
    //    int segment_height = v1.y - v0.y + 1;
    //    for(int y=v0.y;y<v1.y;y++)
    //    {
    //        float alpha = (float)(y - v0.y) / totalHeight;
    //        float beta = (float)(y - v0.y) / segment_height;
    //         Vector2Int t0= (v2 - v0);
    //        Vector2Int t1 = (v1 - v0);
    //        Vector2Int A = v0 + new Vector2Int((int)(t0.x * alpha), (int)(t0.y * alpha));
    //        Vector2Int B = v0 + new Vector2Int((int)(t1.x * beta), (int)(t1.y * beta));

    //        if (A.x > B.x) Swap(ref A,ref  B);
    //        for(int x=A.x;x<B.x;x++)
    //        {
    //            texture2D.SetPixel(x, y, Color.white);


    //        }

    //    }
    //    texture2D.Apply();
    //}
    void DrawMesh(Mesh mesh,Vector3 lightDir)
    {
        int[] triangles = mesh.triangles;
     
        Vector3[] vertices=mesh.vertices;
        int faceLen = triangles.Length / 3;
        faces = new List<int[]>(faceLen);
        for (int i=0;i< faceLen; i++)
        {
            int index0 = triangles[i * 3 + 0];
            int index1 = triangles[i * 3 + 1];
            int index2 = triangles[i * 3 +  2];
            faces.Add(new int[] { index0, index1, index2 });
        }
        Vector3[] vers=  new Vector3[3];
        for (int i=0;i< faceLen; i++)
        {
            int []face= faces[i];
            vers[0]=  new Vector3( (int)((vertices[face[0]].x+1)*width/2.0f+0.5f), (int)((vertices[face[0]].y + 1) * height / 2.0f + 0.5f), vertices[face[0]].z);
            vers[1] = new Vector3((int)((vertices[face[1]].x + 1) * width / 2.0f + 0.5f), (int)((vertices[face[1]].y + 1) * height / 2.0f+0.5f ), vertices[face[1]].z );
            vers[2] = new Vector3((int)((vertices[face[2]].x + 1) * width / 2.0f + 0.5f), (int)((vertices[face[2]].y + 1) * height / 2.0f + 0.5f), vertices[face[2]].z);
           (bool isfront, Color color)= CalculationLightColor(vertices[face[0]], vertices[face[1]], vertices[face[2]], lightDir);
            if(isfront)
            DrawTriangles(vers, color, texture2D);
        
        }
        texture2D.Apply();
        
    }
    (bool,Color) CalculationLightColor(Vector3 P0,Vector3 P1, Vector3 P2 ,Vector3 lightDir)
    {
        Vector3 N= Vector3.Cross( (P2 - P0), (P1 - P0));
        float c = Vector3.Dot(N.normalized, lightDir);
         if (c < 0)
            return (false,default);

    
        return (true, new Color(c,c,c,1));
    }
    void ApplyTexture(Color[][] colors)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
