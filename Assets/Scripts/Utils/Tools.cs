using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public static Sprite CaptureCamera(RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D texture2DTemp = new Texture2D(rt.width, rt.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        texture2DTemp.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2DTemp.Apply();
        RenderTexture.active = null;
        //byte[] bytes = screenShot.EncodeToPNG();

        Sprite spriteTemp = Sprite.Create(texture2DTemp, new Rect(0, 0, texture2DTemp.width, texture2DTemp.height), new Vector2(0.5f, 0.5f));

        return spriteTemp;
    }

    public static Vector2 SetSpriteRectSize(Vector2 vecTarget, Sprite sprite)
    {
        //获得最大值,然后 width=height
        if (vecTarget.x > vecTarget.y)
        {
            vecTarget.y = vecTarget.x;
        }
        else
        {
            vecTarget.x = vecTarget.y;

        }

        //计算图片长宽比
        if (sprite.rect.height > sprite.rect.width)
        {
            vecTarget.x = vecTarget.x * (sprite.rect.width / sprite.rect.height);
            return vecTarget;
        }
        else
        {
            vecTarget.y = vecTarget.y * (sprite.rect.height / sprite.rect.width);
        }
        return vecTarget;
    }

    static string strFileHead = @"\}{:>?<)(~`12345./,?><:[*&^%$#$@!D";
    public static byte[] GetFileByte(string strFile)
    {
        string str = strFileHead + strFile;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
        byte[] buffer_0 = new byte[buffer.Length / 2];
        byte[] buffer_1 = new byte[buffer.Length / 2];
        byte[] buffer_2 = new byte[buffer.Length / 2];
        byte[] buffer_3 = new byte[buffer.Length / 2];
        int int_0 = 0;
        int int_1 = 0;
        int int_2 = 0;
        int int_3 = 0;
        int temp = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            temp = i % 4;
            if (temp == 0)
            {
                buffer_0[int_0++] = buffer[i];
            }
            else if (temp == 1)
            {
                buffer_1[int_1++] = buffer[i];
            }
            else if (temp == 2)
            {
                buffer_2[int_2++] = buffer[i];
            }
            else if (temp == 3)
            {
                buffer_3[int_3++] = buffer[i];
            }
        }

        byte[] head = System.BitConverter.GetBytes(buffer.Length);
        int intIndex_0 = 0;
        int intIndex_1 = 0;
        int intIndex_2 = 0;
        int intIndex_3 = 0;
        int intCount = 4 + buffer_0.Length + buffer_1.Length + buffer_2.Length + buffer_3.Length;
        byte[] byteData = new byte[intCount];
        for (int i = 0; i < intCount; i++)
        {
            temp = i % 4;
            if (i < 4)
            {
                byteData[i] = head[i];
            }
            else if (i < 4 + buffer_0.Length)
            {
                byteData[i] = buffer_2[intIndex_0++];
            }
            else if (i < 4 + buffer_0.Length + buffer_1.Length)
            {
                byteData[i] = buffer_1[intIndex_1++];
            }
            else if (i < 4 + buffer_0.Length + buffer_1.Length + buffer_2.Length)
            {
                byteData[i] = buffer_0[intIndex_2++];
            }
            else if (i < 4 + buffer_0.Length + buffer_1.Length + buffer_2.Length + buffer_3.Length)
            {
                byteData[i] = buffer_3[intIndex_3++];
            }
        }
        return byteData;
    }
    public static string GetFileString(byte[] byteFile)
    {
        byte[] byteCount = new byte[] { byteFile[0], byteFile[1], byteFile[2], byteFile[3] };
        int intCount = System.BitConverter.ToInt32(byteCount);
        byte[] buffer_0 = new byte[intCount / 2];
        byte[] buffer_1 = new byte[intCount / 2];
        byte[] buffer_2 = new byte[intCount / 2];
        byte[] buffer_3 = new byte[intCount / 2];
        int int_0 = 0;
        int int_1 = 0;
        int int_2 = 0;
        int int_3 = 0;
        for (int i = 0; i < buffer_0.Length * 4; i++)
        {
            if (i < buffer_0.Length)
            {
                buffer_0[int_0++] = byteFile[i + 4];
            }
            else if (i < buffer_0.Length * 2)
            {
                buffer_1[int_1++] = byteFile[i + 4];
            }
            else if (i < buffer_0.Length * 3)
            {
                buffer_2[int_2++] = byteFile[i + 4];
            }
            else if (i < buffer_3.Length * 4)
            {
                buffer_3[int_3++] = byteFile[i + 4];
            }
        }
        byte[] buffer = new byte[intCount];
        int intIndex_0 = 0;
        int intIndex_1 = 0;
        int intIndex_2 = 0;
        int intIndex_3 = 0;
        int temp = 0;
        for (int i = 0; i < intCount; i++)
        {
            temp = i % 4;
            if (temp == 0)
            {
                buffer[i] = buffer_2[intIndex_0++];
            }
            else if (temp == 1)
            {
                buffer[i] = buffer_1[intIndex_1++];
            }
            else if (temp == 2)
            {
                buffer[i] = buffer_0[intIndex_2++];
            }
            else if (temp == 3)
            {
                buffer[i] = buffer_3[intIndex_3++];
            }
        }
        string str = System.Text.Encoding.UTF8.GetString(buffer);
        str = str.Replace(strFileHead, "");
        return str;
    }
}
