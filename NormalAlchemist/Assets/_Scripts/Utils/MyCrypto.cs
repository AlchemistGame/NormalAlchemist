using BlowfishNET;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
public class MyCrypto
{
    private readonly static string key64 = "*YWR^$FX";
    private readonly static string iv64 = "#)(^*^@*";
    private readonly static string key128 = "*YWR^$FX*YWR^$FX";
    private readonly static string iv128 = "#)(^*^@*#)(^*^@*";
    private readonly static string key224 = "NJ#**ZtKPF70943nfOD8HD#FFGOj";
    private readonly static string iv224 = "ZMwcwfYqSnqI0YzoazJJ1!2*3C9N";
    private readonly static string key448 = "Nr3Hpuue9K5I29g@ylty2kTS0*l@thO!J7td4@r4&OQP2Z%ifzB*mdkf";
    private readonly static string iv448 = "UnAp%@I!h6uRuL$yfEj0ejGUBig3aXCHQfBTmVSh5o2gdiQAPztblo^Q";

    public static string GetMD5(byte[] data)
    {
        DateTime lastTime = DateTime.Now;
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        //string md5Code = System.Text.Encoding.UTF8.GetString(md5.ComputeHash(data));
        StringBuilder md5Code = new StringBuilder();
        foreach (byte b in md5.ComputeHash(data))
        {
            md5Code.Append(b.ToString("X"));
        }
        md5.Clear();
        md5 = null;
        Debug.Log("GetMD5! UseTime: " + (DateTime.Now - lastTime) + " DataLenth: " + data.Length);
        ResourceManager.ReleaseMemory();
        return md5Code.ToString();
    }

    public static byte[] DESEncode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key64);
        byte[] iv = Encoding.UTF8.GetBytes(iv64);
        byte[] enData;

        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            enData = ms.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            cs.Close();
            ms.Close();
        }
        return enData;
    }

    public static byte[] DESDecode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key64);
        byte[] iv = Encoding.UTF8.GetBytes(iv64);
        byte[] deData;

        DESCryptoServiceProvider algorithm = new DESCryptoServiceProvider();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            deData = ms.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            cs.Close();
            ms.Close();
        }

        return deData;
    }

    public static byte[] AESEncode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key128);
        byte[] iv = Encoding.UTF8.GetBytes(iv128);
        byte[] enData = null;

        //AesCryptoServiceProvider algorithm = new AesManaged;
        //MemoryStream ms = new MemoryStream();
        //CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(key, iv), CryptoStreamMode.Write);
        //try
        //{
        //    cs.Write(data, 0, data.Length);
        //    cs.FlushFinalBlock();
        //    enData = ms.ToArray();
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //    return null;
        //}
        //finally
        //{
        //    cs.Close();
        //    ms.Close();
        //}
        return enData;
    }

    public static byte[] AESDecode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key128);
        byte[] iv = Encoding.UTF8.GetBytes(iv128);
        byte[] deData = null;
        //AesManaged.Create()
        //AesCryptoServiceProvider algorithm = new AesCryptoServiceProvider();
        MemoryStream ms = new MemoryStream();
        //CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
        //try
        //{
        //    cs.Write(data, 0, data.Length);
        //    cs.FlushFinalBlock();
        //    deData = ms.ToArray();
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //    return null;
        //}
        //finally
        //{
        //    cs.Close();
        //    ms.Close();
        //}

        return deData;
    }

    public static byte[] RJEncode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key128);
        byte[] iv = Encoding.UTF8.GetBytes(iv128);
        byte[] enData;

        RijndaelManaged algorithm = new RijndaelManaged();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            enData = ms.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            cs.Close();
            ms.Close();
        }
        return enData;
    }

    public static byte[] RJDecode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key128);
        byte[] iv = Encoding.UTF8.GetBytes(iv128);
        byte[] deData;

        RijndaelManaged algorithm = new RijndaelManaged();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            deData = ms.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            cs.Close();
            ms.Close();
        }

        return deData;
    }


    public static MemoryStream BlowfishEncode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);
        //byte[] encodeBuffer = new byte[];
        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();

        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        //finally
        //{
        //    //cs.Close();
        //    //ms.Close();
        //}
    }

    public static byte[] BlowfishEncode_beta(byte[] data)
    {
        //int offset = 1048575;
        if (data == null)
        {
            return null;
        }
        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);
        byte[] encodeBuffer = new byte[encodeSize];
        byte[] rtData = null;
        System.Collections.Generic.List<byte[]> ls_dataP = new System.Collections.Generic.List<byte[]>();
        int totalLenth = 0;
        //MemoryStream ms = new MemoryStream((data.Length / encodeSize + 2) * encodeSize);
        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();
        CryptoStream cs;
        for (int i = 0; i <= data.Length / encodeSize; i++)
        {
            MemoryStream ms = new MemoryStream();
            ms.Seek(0, SeekOrigin.Begin);
            cs = new CryptoStream(ms, algorithm.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            if (i < data.Length / encodeSize)
            {
                Array.Copy(data, i * encodeSize, encodeBuffer, 0, encodeSize);
                try
                {
                    //Debug.Log(cs.Length);
                    cs.Write(encodeBuffer, 0, encodeBuffer.Length);
                    //cs.Length;
                    //cs.FlushFinalBlock();
                    cs.FlushFinalBlock();
                    ms.Flush();
                    //ms.Seek(0, SeekOrigin.Begin);
                    byte[] dataP = ms.ToArray();
                    Debug.Log("En Length ::" + dataP.Length + " Last ::" + dataP[dataP.Length - 1]);
                    totalLenth += dataP.Length;

                    BlowfishDecode(dataP);

                    ls_dataP.Add(dataP);
                    //Debug.Log(cs.);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return null;
                }
            }
            else
            {
                if (data.Length > encodeSize * i)
                {
                    encodeBuffer = null;
                    encodeBuffer = new byte[data.Length - encodeSize * i];
                    Array.Copy(data, i * encodeSize, encodeBuffer, 0, encodeBuffer.Length);
                    try
                    {
                        cs.Write(encodeBuffer, 0, encodeBuffer.Length);
                        //Debug.Log(cs.);
                        cs.FlushFinalBlock();
                        //ms.Seek(0, SeekOrigin.Begin);
                        byte[] dataP = ms.ToArray();
                        ls_dataP.Add(dataP);

                        BlowfishDecode(dataP);
                        Debug.Log("En Length ::" + dataP.Length + " Last ::" + dataP[dataP.Length - 1]);

                        totalLenth += dataP.Length;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                        return null;
                    }
                }

            }
        }

        rtData = new byte[totalLenth];
        int offset = 0;
        foreach (var i in ls_dataP)
        {
            Array.Copy(i, 0, rtData, offset, i.Length);
            offset += i.Length;
        }
        Debug.Log("En  Lenght ::" + rtData.Length);

        return rtData;
        //Debug.Log(" MonoHeapMemory:" + Tools_Client.FormatFileSize(Profiler.GetMonoHeapSizeLong()));
        //Debug.Log(ms.Length);
    }

    /// <summary>
    /// 用时短 内存占用大
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static MemoryStream BlowfishDecode(byte[] data)
    {
        if (data == null)
        {
            return null;
        }
        DateTime lastTime = DateTime.Now;
        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);

        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();
        algorithm.Padding = PaddingMode.PKCS7;
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin);
            Debug.Log("BlowfishDecode byte[]! UseTime: " + (DateTime.Now - lastTime) + " DataLenth: " + data.Length);
            return ms;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static readonly int encodeSize = 1048575;
    public static MemoryStream BlowfishEncode_alpha(byte[] data)
    {
        //int offset = 1048575;
        if (data == null)
        {
            return null;
        }

        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);
        byte[] encodeBuffer = new byte[encodeSize];
        MemoryStream ms = new MemoryStream((data.Length / encodeSize + 2) * encodeSize);
        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();
        CryptoStream cs;
        for (int i = 0; i <= data.Length / encodeSize; i++)
        {
            cs = new CryptoStream(ms, algorithm.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            if (i < data.Length / encodeSize)
            {
                Array.Copy(data, i * encodeSize, encodeBuffer, 0, encodeSize);
                try
                {
                    //Debug.Log(cs.Length);
                    cs.Write(encodeBuffer, 0, encodeBuffer.Length);
                    //cs.Length;
                    //cs.FlushFinalBlock();
                    cs.FlushFinalBlock();
                    //Debug.Log(cs.);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return null;
                }
            }
            else
            {
                if (data.Length > encodeSize * i)
                {
                    encodeBuffer = null;
                    encodeBuffer = new byte[data.Length - encodeSize * i];
                    Array.Copy(data, i * encodeSize, encodeBuffer, 0, encodeBuffer.Length);
                    try
                    {
                        cs.Write(encodeBuffer, 0, encodeBuffer.Length);
                        //Debug.Log(cs.);
                        cs.FlushFinalBlock();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                        return null;
                    }
                }

            }
        }
        ms.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        //Debug.Log(" MonoHeapMemory:" + Tools_Client.FormatFileSize(Profiler.GetMonoHeapSizeLong()));
        //Debug.Log(ms.Length);
        return ms;

    }

    /// <summary>
    /// 用时短 内存占用大
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static MemoryStream BlowfishDecode_alpha(MemoryStream ms, byte[] data, bool isEnd = false)
    {
        if (data == null)
        {
            return null;
        }
        DateTime lastTime = DateTime.Now;
        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);

        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();

        //MemoryStream ms = ms);
        CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
        try
        {
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            //ms.Seek(0, SeekOrigin.Begin);
            Debug.Log("BlowfishDecode byte[]! UseTime: " + (DateTime.Now - lastTime) + " DataLenth: " + data.Length);
            //Debug.Log("BlowfishDecode byte[]! UseTime: " + (DateTime.Now - lastTime) + " DataLenth: " + data.Length);
            if (isEnd)
            {
                ms.Seek(0, SeekOrigin.Begin);
            }
            return ms;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }


    /// <summary>
    /// 节约内存 用时更长
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileMode"></param>
    /// <param name="fileAccess"></param>
    /// <returns></returns>
    public static byte[] BlowfishDecode(FileStream data)
    {
        if (data == null)
        {
            return null;
        }
        DateTime lastTime = DateTime.Now;
        byte[] key = Encoding.UTF8.GetBytes(key448);
        byte[] iv = Encoding.UTF8.GetBytes(iv448);

        BlowfishAlgorithm algorithm = new BlowfishAlgorithm();

        CryptoStream cs = new CryptoStream(data, algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Read);
        byte[] deData = new byte[data.Length];
        try
        {
            cs.Read(deData, 0, deData.Length);
            Debug.Log("BlowfishDecode FileStream! UseTime: " + (DateTime.Now - lastTime) + " DataLenth: " + deData.Length);
            return deData;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            cs.Close();
            data.Close();
            ResourceManager.ReleaseMemory();
        }
    }
}
