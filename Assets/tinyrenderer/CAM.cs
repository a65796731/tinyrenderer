using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAM
{
    // Start is called before the first frame update
    Matrix4x4 ModelView;
    float depth = 1;
    public   void lockAt(Vector3 eye, Vector3 target,Vector3 up)
    {
         Vector3 z = (eye - target).normalized;
         Vector3 x= Vector3.Cross(up, z).normalized;
          Vector3 y = Vector3.Cross(z,x).normalized;
        Matrix4x4 minv = Matrix4x4.identity;
        Matrix4x4 Tr = Matrix4x4.identity;//Î»ÒÆ
        minv.SetRow(0, x);
        minv.SetRow(1, y);
        minv.SetRow(2, z);
        Tr.SetColumn(3, -eye);
        ModelView= minv* Tr;
    }
    public Matrix4x4 viewPort(int x,int y,int w,int h)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.SetRow(0,new Vector4(w/2f,0,0,x+w/2f));
        m.SetRow(1, new Vector4(0, h/2, 0, x + w / 2f));
        m.SetRow(2, new Vector4(0,0, depth / 2f, depth / 2f));
        m.SetRow(3, new Vector4(0, 0, 0, 1f));
        return m;
    }
}
