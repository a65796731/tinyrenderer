using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tinyrenderer : MonoBehaviour
{
    private Texture2D texture2D = null;
    public RawImage rawImag = null;
    public Mesh mesh = null;
    private int width, height;
    private List<int[]> faces = null;
    public Material material = null;
    // Start is called before the first frame update
    void Start()
    {
        width = (int)rawImag.rectTransform.rect.width;
        height= (int)rawImag.rectTransform.rect.height;
        texture2D = new Texture2D((int)rawImag.rectTransform.rect.width, (int)rawImag.rectTransform.rect.height, TextureFormat.RGB24,false);
        rawImag.texture = texture2D;
        texture2D.filterMode = FilterMode.Point;
        //  DrawMesh(mesh);
        Vector2Int[] t0 = new Vector2Int[]
        { new Vector2Int(10,70),new Vector2Int(50,160),new Vector2Int(70,80) };
        Vector2Int[] t1 = new Vector2Int[]
      { new Vector2Int(180,50),new Vector2Int(150,1),new Vector2Int(70,180) };
        Vector2Int[] t2  = new Vector2Int[]
      { new Vector2Int(180,180),new Vector2Int(120,160),new Vector2Int(130,180) };

        DrawTriangles(t0[0], t0[1], t0[2],Color.red,texture2D);
        DrawTriangles(t1[0], t1[1], t1[2], Color.green, texture2D);
        DrawTriangles(t2[0], t2[1], t2[2], Color.red, texture2D);
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
    void DrawTriangles(Vector2Int v0,Vector2Int v1,Vector2Int v2,Color color,Texture2D texture2D)
    {

        if (v0.y > v1.y) { Swap<Vector2Int>(ref v0,ref v1); }
        if (v0.y > v2.y) { Swap<Vector2Int>(ref v0, ref v2); }
        if (v1.y > v2.y) { Swap<Vector2Int>(ref v1, ref v2); }
        int totalHeight = v2.y - v0.y;
        int segment_height = v1.y - v0.y + 1;
        for(int y=v0.y;y<v1.y;y++)
        {
            float alpha = (float)(y - v0.y) / totalHeight;
            float beta = (float)(y - v0.y) / segment_height;
             Vector2Int t0= (v2 - v0);
            Vector2Int t1 = (v1 - v0);
            Vector2Int A = v0 + new Vector2Int((int)(t0.x * alpha), (int)(t0.y * alpha));
            Vector2Int B = v0 + new Vector2Int((int)(t1.x * beta), (int)(t1.y * beta));
         
            if (A.x > B.x) Swap(ref A,ref  B);
            for(int x=A.x;x<B.x;x++)
            {
                texture2D.SetPixel(x, y, Color.white);
             

            }

        }
        texture2D.Apply();
    }
    void DrawMesh(Mesh mesh)
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
        for(int i=0;i< faceLen; i++)
        {
            int []face= faces[i];
            for (int j=0;j<3;j++)
            {
                int index0 = face[j];
                int index1 = face[(j+1)%3];
                Vector3 v0 = vertices[index0];
                Vector3 v1= vertices[index1];
                int x0 = (int)((v0.x + 1.0f) * width / 2.0f);
                int y0 = (int)((v0.y + 1.0f) * height / 2.0f);
                int x1 = (int)((v1.x + 1.0f) * width / 2.0f);
                int y1 = (int)((v1.y + 1.0f) * height / 2.0f);
                Debug.Log(v0);
                DrawLine(x0,y0,x1,y1,Color.white,texture2D);
            }
        }
        texture2D.Apply();
        //material.SetTexture("Texture",texture2D);
    }
    void ApplyTexture(Color[][] colors)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
