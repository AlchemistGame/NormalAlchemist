using System.IO;
using System.Windows.Forms;
using LitJson;

namespace ConfigLoad
{
    public class SaveJson
    {
        public static string path = Application.StartupPath + "\\cfg\\";
        public static string mmPath = Application.StartupPath + "\\MonsterMap\\";
        public static void Save(JsonData json, System.Collections.Generic.Dictionary<string, string> filePath)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            if (Directory.Exists(mmPath))
            {
                Directory.Delete(mmPath, true);
            }
            //创建目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(mmPath))
            {
                Directory.CreateDirectory(mmPath);
            }

            foreach (var data in filePath)
            {
                if (data.Value != "\\Config\\" && data.Value != "\\MonsterMap\\")
                {
                    if (!Directory.Exists(path + "\\" + data.Value + "\\"))
                    {
                        Directory.CreateDirectory(path + "\\" + data.Value + "\\");
                    }
                }
            }

            foreach (var name in json.Keys)
            {
                //JsonData js = JsonMapper.ToObject(json.ToJson());
                //var it = js.Keys.GetEnumerator();
                //while (it.MoveNext())
                //{
                //    string dir = "";
                //    filePath.TryGetValue(it.Current, out dir);
                //    FileStream fs;
                //    if (dir == "\\Config\\")
                //    {
                //        dir = "";
                //        fs = new FileStream(path + dir + it.Current + ".json", FileMode.Create);
                //        WriteStr(fs, js[it.Current].ToJson());
                //    }
                //    else if (dir == "\\MonsterMap\\")
                //    {
                //        fs = new FileStream(mmPath + it.Current + ".json", FileMode.Create);
                //        WriteStr(fs, js[it.Current].ToJson());
                //    }
                //}
                int i = 0;
                try
                {
                    string dir = "";
                    filePath.TryGetValue(name, out dir);
                    FileStream fs;
                    if (dir == "\\Config\\")
                    {
                        dir = "";
                        fs = new FileStream(path + dir + name + ".json", FileMode.Create);
                    }
                    else if (dir == "\\MonsterMap\\")
                    {
                        fs = new FileStream(mmPath + name + ".json", FileMode.Create);
                    }
                    else
                    {
                        continue;
                    }

                    string strJson = "{";
                    for (i = 0; i < json[name].Count; ++i)
                    {
                        try
                        {
                            JsonData js = JsonMapper.ToObject(json[name][i].ToJson());
                            strJson += "\"" + json[name][i]["id"].ToJson() + "\"" + ":" + js.ToJson();
                            if (i != json[name].Count - 1)
                            {
                                strJson += ",\n";
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString() + "\nID: " + json[name][i]["id"].ToString(), name);
                        }

                    }
                    strJson += "}";
                    WriteStr(fs, strJson);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), name);
                    continue;
                }


                //try
                //{
                //    JsonData js = JsonMapper.ToObject(json[name].ToJson());
                //    string dir = "";
                //    filePath.TryGetValue(name, out dir);
                //    FileStream fs;
                //    if (dir == "\\Config\\")
                //    {
                //        dir = "";
                //        fs = new FileStream(path + dir + name + ".json", FileMode.Create);
                //        WriteStr(fs, js.ToJson());
                //    }
                //    else if (dir == "\\MonsterMap\\")
                //    {
                //        fs = new FileStream(mmPath + name + ".json", FileMode.Create);
                //        WriteStr(fs, js.ToJson());
                //    }
                //}
                //catch (System.Exception ex)
                //{
                //    MessageBox.Show(ex.Message.ToString(), name);
                //}
            }
        }

        public static void WriteBinary(FileStream fs, byte[] buffs)
        {
            BinaryWriter bw = new BinaryWriter(fs);

            try
            {
                for (int i = 0; i < buffs.Length; ++i)
                    bw.Write((int)buffs[i]);
                bw.Flush();
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }

        public static void WriteStr(FileStream fs, string json)
        {
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.Write(json);
                sw.Flush();
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
        }

        public static byte[] getBuffs(string json)
        {
            byte[] buffs = new byte[json.Length];
            for (int i = 0; i < json.Length; ++i)
            {
                buffs[i] = (byte)json[i];
            }
            return buffs;
        }
    }
}
