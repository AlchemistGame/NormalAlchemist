using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MyFile
{
    public static byte[] ReadFile(string path, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read)
    {
        DateTime lastTime = DateTime.Now;
        byte[] data;
        FileStream fs = null;
        try
        {
            fs = File.Open(path, fileMode, fileAccess, FileShare.Read);
            data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            Debug.Log("Read File Byte[] Sucess! UseTime:" + (DateTime.Now - lastTime) + " path:" + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
        return data;
    }
    
    public static FileStream GetFileStream(string path, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read)
    {
        FileStream fs = null;
        try
        {
            fs = File.Open(path, fileMode, fileAccess, FileShare.Read);
            Debug.Log("Read File Stream Sucess! path:" + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        return fs;
    }

    public static bool WriteFile(string path, byte[] data, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write)
    {
        DateTime lastTime = DateTime.Now;
        FileStream fs = null;
        try
        {
            fs = File.Open(path, fileMode, fileAccess);
            fs.Position = 0;
            fs.SetLength(data.Length);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            Debug.Log("Write File Sucess! UseTime:" + (DateTime.Now - lastTime) + " path:" + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }
        return true;
    }

    //public static void AsyncReadFile(string path, out AsyncFileState state)
    //{
    //    state = new AsyncFileState();
    //    try
    //    {
    //        state.fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024000, true);
    //        state.data = new byte[state.fs.Length];
    //        state.fs.BeginRead(state.data, 0, 1024000, OnAsyncRead, state);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        if (state.fs != null)
    //            state.fs.Close();
    //    }
    //}

    //private static void OnAsyncRead(IAsyncResult ar)
    //{
    //    AsyncFileState state = ((AsyncFileState)ar.AsyncState);
    //    try
    //    {
    //        int offset = state.fs.EndRead(ar);
    //        Debug.Log(offset);
    //        if (offset > 0)
    //        {
    //            state.fs.BeginRead(state.data, 0, 1024000, OnAsyncRead, state);
    //        }
    //        else
    //        {
    //            state.fs.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        if (state.fs != null)
    //            state.fs.Close();
    //    }
    //}

    //public struct AsyncFileState
    //{
    //    public FileStream fs;
    //    public byte[] data;
    //}
}
