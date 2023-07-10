using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuaternion
{
    public float x, y, z, w;

    public static readonly MyQuaternion identityQ = new MyQuaternion();

    public MyQuaternion()
    {
        x = y = z = 0;
        w = 1;
    }

    public MyQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public void Set(float newX, float newY, float newZ, float newW)
    {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
    }

    public static MyQuaternion operator *(MyQuaternion q1, MyQuaternion q2)
    {
        return new MyQuaternion(q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
                                q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
                                q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x,
                                q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z);
    }

    public static Vector3 operator *(MyQuaternion q, Vector3 v)
    {
        float num = q.x * 2f;
        float num2 = q.y * 2f;
        float num3 = q.z * 2f;
        float num4 = q.x * num;
        float num5 = q.y * num2;
        float num6 = q.z * num3;
        float num7 = q.x * num2;
        float num8 = q.x * num3;
        float num9 = q.y * num3;
        float num10 = q.w * num;
        float num11 = q.w * num2;
        float num12 = q.w * num3;

        Vector3 result;
        result.x = (1f - (num5 + num6)) * v.x + (num7 - num12) * v.y + (num8 + num11) * v.z;
        result.y = (num7 + num12) * v.x + (1f - (num4 + num6)) * v.y + (num9 - num10) * v.z;
        result.z = (num8 - num11) * v.x + (num9 + num10) * v.y + (1f - (num4 + num5)) * v.z;

        return result;
    }

    public static MyQuaternion Conjugate(MyQuaternion q)
    {
        return new MyQuaternion(-q.x, -q.y, -q.z, q.w);
    }

    public static float Norm(MyQuaternion q)
    {
        return Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
    }

    public static float DotProduct(MyQuaternion q1, MyQuaternion q2)
    {
        return q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;
    }

    public static MyQuaternion Normalize(MyQuaternion q)
    {
        float mag = Mathf.Sqrt(DotProduct(q, q));

        if (mag < Mathf.Epsilon)
            return identityQ;

        return new MyQuaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }

    public static MyQuaternion Inverse(MyQuaternion q)
    {
        Conjugate(q);
        float num = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
        return new MyQuaternion(q.x /= num, q.y /= num, q.z /= num, q.w /= num);
    }

    public static Quaternion Euler(float x, float y, float z)
    {
        x *= Mathf.Deg2Rad;
        y *= Mathf.Deg2Rad;
        z *= Mathf.Deg2Rad;

        float zOver2 = z * 0.5f;
        float sinzOver2 = (float)System.Math.Sin((float)zOver2);
        float coszOver2 = (float)System.Math.Cos((float)zOver2);

        float yOver2 = y * 0.5f;
        float sinyOver2 = (float)System.Math.Sin((float)yOver2);
        float cosyOver2 = (float)System.Math.Cos((float)yOver2);

        float xOver2 = x * 0.5f;
        float sinxOver2 = (float)System.Math.Sin((float)xOver2);
        float cosxOver2 = (float)System.Math.Cos((float)xOver2);

        MyQuaternion qx = new MyQuaternion(sinxOver2, 0, 0, cosxOver2);
        MyQuaternion qy = new MyQuaternion(0, sinyOver2, 0, cosyOver2);
        MyQuaternion qz = new MyQuaternion(0, 0, sinzOver2, coszOver2);

        MyQuaternion q;

        q = qx * qy * qz;
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    public static Quaternion matrixRotation(float x, float y, float z, Quaternion q)
    {
        Quaternion rotation = Quaternion.Euler(x, y, z);
        Matrix4x4 rot = Matrix4x4.Rotate(q);
        Matrix4x4 newrot = Matrix4x4.Rotate(rotation);

        Matrix4x4 m = newrot * rot;

        Quaternion qu = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return qu;
    }
      
    override public string ToString()
    {
        return string.Format("(x:{0}%.2f, y:{1}, z:{2}, w:{3})", x, y, z, w);
    }

}
