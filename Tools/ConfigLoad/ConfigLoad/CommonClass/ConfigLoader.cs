using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
namespace ConfigLoad
{
    public interface IConfig
    {
        //void LoadData(ConfigLoad loader);
        void LoadTable(IReader reader);
        IConfig GetConfig(uint id);

        void BuildConfig(uint id);
    }

    public struct SVector2
    {
        float x;
        float y;
        public SVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class EnemyConfig : IConfig
    {
        public static Table<EnemyConfig> ConfigTable = new Table<EnemyConfig>();
        public uint id;
        public int atk;
        public SVector2 vector2;
        public void BuildConfig(uint id)
        {
            EnemyConfig cfg = new EnemyConfig();
            int idx = ConfigTable.IdToIndex(id);
            if (idx == -1)
            {
                ConfigTable.configCache[id] = null;
                return;
            }
            object temp;
            cfg.id = (temp = ConfigTable.TryGetColDataFromPool("id", idx)) == null ? 0 : (uint)temp;
            cfg.atk = (temp = ConfigTable.TryGetColDataFromPool("atk", idx)) == null ? 0 : (int)temp;
            cfg.vector2 = new SVector2(
                 (temp = ConfigTable.TryGetColDataFromPool("vector2_x", idx)) == null ? 1.1f : (uint)temp,
                  (temp = ConfigTable.TryGetColDataFromPool("vector2_y", idx)) == null ? 1.2f : (uint)temp
                ); 
            ConfigTable.configCache[id] = cfg;
        }

        public IConfig GetConfig(uint id)
        {
            if (!ConfigTable.configCache.ContainsKey(id))
            {
                BuildConfig(id);
            }
            return ConfigTable.configCache[id];
        }

        public void LoadTable(IReader reader)
        {
            ConfigTable.InitTable(reader);
        }
    }

    public class ConfigWriter
    {
        private ConfigWriter instance;
        public ConfigWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigWriter();
                }
                return instance;
            }
        }
        private int configVersion;
        private int tableNum;
        public void WriteTable(BinaryWriter writer)
        {
            writer.Write(configVersion);
            writer.Write(tableNum);
        }
    }

    public class ConfigLoader
    {
        private ConfigLoader instance;
        public ConfigLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigLoader();
                }
                return instance;
            }
        }
        public int ConfigVersion => configVersion;
        private int configVersion;
        private int tableNum;

        public void InitTable(IReader reader)
        {
            configVersion = reader.ReadInt32();
            tableNum = reader.ReadInt32();
            for (int i = 0; i < tableNum; i++)
            {
                string tableName = reader.ReadString();
                Type t = Type.GetType(tableName);
                object model = Activator.CreateInstance(t);
                (model as IConfig).LoadTable(reader);
                //Type.GetType(tableName);
                //Type t1 = Type.GetType(tableName);
                //MethodInfo mi = this.GetType().GetMethod("CreateTable").MakeGenericMethod(new Type[] { t1 });
                //mi.Invoke(this, new object[] { reader, tableName });

                //Type t = Type.GetType(tableName);
                //MethodInfo mi = t.GetMethod("Method").MakeGenericMethod(typeof(Person));
                //mi.Invoke(obj, new object[] { 5 });
                //Table<>
            }
        }
    }
}
