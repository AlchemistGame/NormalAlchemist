using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace ConfigLoad
{
    public interface IConfigData
    {
        void SetConfigData();
    }
    //列信息
    public class ColInfo
    {
        public byte dataType;
        public int poolIdx;
        public string name;
        public ColInfo(byte type, int idx, string name)
        {
            dataType = type;
            poolIdx = idx;
            this.name = name;
        }
        //public int length;
    }
    public interface IReader
    {
        int ReadInt32();
        double ReadDouble();
        float ReadSingle();
        bool ReadBoolean();
        byte ReadByte();
        uint ReadUInt32();
        long ReadInt64();
        string ReadString();
    }

    public class DataReader : IReader
    {
        BinaryReader reader;
        public DataReader(Stream stream)
        {
            reader = new BinaryReader(stream);
        }
        public int ReadInt32()
        {
            return reader.ReadInt32();
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public uint ReadUInt32()
        {
            return reader.ReadUInt32();
        }

        public long ReadInt64()
        {
            return reader.ReadInt64();
        }

        public string ReadString()
        {
            return reader.ReadString();
        }
    }

    public class FileRW
    {
        public static void BinaryRead(string fileName)
        {
            FileStream fs;
            fs = new FileStream(fileName, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fs);
            binaryReader.ReadInt32();
            binaryReader.ReadDouble();
            binaryReader.ReadSingle();
            binaryReader.ReadBoolean();
            binaryReader.ReadByte();
            binaryReader.ReadUInt32();
            binaryReader.ReadInt64();
            binaryReader.ReadString();
        }

        public static void BinaryWrite()
        {
            // cfg version w
            // table length w
            // table 循环 length
            //-- tableWrite
            //-- row count 行数 w
            //-- col count 列数 w
            //---- 单行循环 
            //---- type w
            //---- value w
            //---- 若为 struct type
            //---- length w
            //---- type w
            //---- value w

        }
    }

    public interface ConfigStruct
    {
        void ConfigStruct();
    }
    public class Table<T> where T : IConfig
    {
        public List<int[]> Int32Pool = new List<int[]>();
        public List<bool[]> BoolPool = new List<bool[]>();
        public List<uint[]> UIntPool = new List<uint[]>();
        public List<float[]> FloatPool = new List<float[]>();
        public List<long[]> LongPool = new List<long[]>();
        public List<ulong[]> UlongPool = new List<ulong[]>();
        public List<string[]> StrPool = new List<string[]>();
        public List<byte[]> BytePool = new List<byte[]>();

        public List<List<int[]>> Int32ArrayPool = new List<List<int[]>>();
        public List<List<bool[]>> BoolArrayPool = new List<List<bool[]>>();
        public List<List<uint[]>> UIntArrayPool = new List<List<uint[]>>();
        public List<List<float[]>> FloatArrayPool = new List<List<float[]>>();
        public List<List<long[]>> LongArrayPool = new List<List<long[]>>();
        public List<List<ulong[]>> UlongArrayPool = new List<List<ulong[]>>();
        public List<List<string[]>> StrArrayPool = new List<List<string[]>>();
        public List<List<byte[]>> ByteArrayPool = new List<List<byte[]>>();

        public int tableVersion = 0;
        public int tableRowLength = 0;
        public int tableColLength = 0;
        private IReader reader;

        public Dictionary<string, Dictionary<string, ColInfo>> SpecialTypeColInfo = new Dictionary<string, Dictionary<string, ColInfo>>();
        public Dictionary<string, ColInfo> ColNodeDict = new Dictionary<string, ColInfo>();

        public Dictionary<uint, T> configCache = new Dictionary<uint, T>();

        IdIndexMap[] IdRanges;
        public enum DataType
        {
            Int32 = 0,
            Bool = 1,
            UInt32 = 2,
            Float = 3,
            Byte = 4,
            Enum = 5,
            String = 6,
            Struct = 7,

            Int32Array = 8,
            BoolArray = 9,
            UInt32Array = 10,
            FloatArray = 11,
            ByteArray = 12,
            EnumArray = 13,
            StringArray = 14,
            StructArray = 15,
        }


        public class IdIndexMap
        {
            public uint startId;
            public int startIdx;
            public int count;

            public bool Contains(uint id)
            {
                return id < startId + count && id >= startId;
            }

            public int GetIndex(uint id)
            {
                return startIdx + (int)(id - startId);
            }
        }

        private void AddColInfo(string colName, byte dataType, int idx)
        {
            ColInfo colInfo = new ColInfo(dataType, idx, colName);
            if (ColNodeDict.ContainsKey(colName))
            {
                ColNodeDict.Add(colName, colInfo);
            }
            else
            {
                throw new Exception($"ColName {colName} is AllReady Have");
            }
        }

        private void ClearData()
        {
            configCache.Clear();
            ColNodeDict.Clear();
            SpecialTypeColInfo.Clear();
            IdRanges = null;

            Int32Pool = new List<int[]>();
            BoolPool = new List<bool[]>();
            UIntPool = new List<uint[]>();
            FloatPool = new List<float[]>();
            LongPool = new List<long[]>();
            UlongPool = new List<ulong[]>();
            StrPool = new List<string[]>();

            Int32ArrayPool = new List<List<int[]>>();
            BoolArrayPool = new List<List<bool[]>>();
            UIntArrayPool = new List<List<uint[]>>();
            FloatArrayPool = new List<List<float[]>>();
            LongArrayPool = new List<List<long[]>>();
            UlongArrayPool = new List<List<ulong[]>>();
            StrArrayPool = new List<List<string[]>>();
        }


        public void InitTable(IReader reader)
        {
            ClearData();

            tableVersion = reader.ReadInt32();
            tableColLength = reader.ReadInt32();
            tableRowLength = reader.ReadInt32();
            this.reader = reader;

            for (int i = 0; i < tableColLength; i++)
            {
                byte dataType = reader.ReadByte();
                string dataName = reader.ReadString();
                int dataIdx = -1;

                if (dataName == "Id")
                {
                    List<IdIndexMap> idIndexMaps = new List<IdIndexMap>();
                    UInt32[] data = new UInt32[tableRowLength];
                    UInt32 tempId = 0;
                    IdIndexMap idIndexMap = new IdIndexMap();
                    int count = 1;
                    for (int j = 0; j < tableRowLength; j++)
                    {

                        data[j] = reader.ReadUInt32();
                        if (j == 0)
                        {
                            idIndexMap.startId = data[j];
                            idIndexMap.startIdx = j;
                            count = 1;
                        }
                        if (tempId + 1 != data[j])
                        {
                            idIndexMap.count = count;
                            idIndexMaps.Add(idIndexMap);
                            idIndexMap = new IdIndexMap();
                            idIndexMap.startId = data[j];
                            idIndexMap.startIdx = j;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        tempId = data[j];
                    }

                    if (idIndexMaps.Count == 0 && tableRowLength > 0)
                    {
                        idIndexMaps.Add(idIndexMap);
                    }
                    this.IdRanges = idIndexMaps.ToArray();
                }
                else
                {
                    switch (dataType)
                    {
                        case (byte)DataType.Bool:
                            dataIdx = AddColDataToPool(BoolPool, (IReader read) => read.ReadBoolean());
                            break;
                        case (byte)DataType.Byte:
                            dataIdx = AddColDataToPool(BytePool, (IReader read) => read.ReadByte());
                            break;
                        case (byte)DataType.Enum:
                            dataIdx = AddColDataToPool(BytePool, (IReader read) => read.ReadByte());
                            break;
                        case (byte)DataType.Float:
                            dataIdx = AddColDataToPool(FloatPool, (IReader read) => read.ReadSingle());
                            break;
                        case (byte)DataType.Int32:
                            dataIdx = AddColDataToPool(Int32Pool, (IReader read) => read.ReadInt32());
                            break;
                        case (byte)DataType.UInt32:
                            dataIdx = AddColDataToPool(UIntPool, (IReader read) => read.ReadUInt32());
                            break;
                        case (byte)DataType.String:
                            dataIdx = AddColDataToPool(StrPool, (IReader read) => read.ReadString());
                            break;
                    }
                }

                AddColInfo(dataName, dataType, dataIdx);
            }
        }

        private int AddColDataToPool<T0>(List<T0[]> listT, Func<IReader, T0> read)
        {
            T0[] data = new T0[tableRowLength];

            for (int j = 0; j < tableRowLength; j++)
            {
                data[j] = read(this.reader);
            }
            listT.Add(data);
            return listT.Count;
        }

        private T1 GetColDataFromPool<T1>(List<T1[]> pool, int configIdx, int poolIdx)
        {
            if (pool[poolIdx] == null)
            {
                return default(T1);
            }
            return pool[poolIdx][configIdx];
        }

        public object TryGetColDataFromPool(string dataName, int configIdx)
        {
            ColInfo info;
            if (!ColNodeDict.ContainsKey(dataName))
            {
                return null;
            }
            info = ColNodeDict[dataName];
            byte dataType = info.dataType;
            int poolIdx = info.poolIdx;
            switch (dataType)
            {
                case (byte)DataType.Bool:
                    return GetColDataFromPool(BoolPool, configIdx, poolIdx);
                case (byte)DataType.Byte:
                    return GetColDataFromPool(BytePool, configIdx, poolIdx);
                case (byte)DataType.Enum:
                    return GetColDataFromPool(BytePool, configIdx, poolIdx);
                case (byte)DataType.Float:
                    return GetColDataFromPool(FloatPool, configIdx, poolIdx);
                case (byte)DataType.Int32:
                    return GetColDataFromPool(Int32Pool, configIdx, poolIdx);
                case (byte)DataType.UInt32:
                    return GetColDataFromPool(UIntPool, configIdx, poolIdx);
                case (byte)DataType.String:
                    return GetColDataFromPool(StrPool, configIdx, poolIdx);
            }
            return null;
        }

        public int IdToIndex(uint id)
        {
            int index = SerchId(this.IdRanges, id);
            return index;
        }

        // id 转 index
        public int SerchId(IdIndexMap[] idRanges, uint id)
        {
            IdIndexMap range = null;
            for (int i = 0; i < idRanges.Length; i++)
            {
                if (idRanges[i].startId >= id && id < idRanges[i].count + idRanges[i].startId)
                {
                    range = idRanges[i];
                    break;
                }
            }
            if (range == null)
            {
                throw new Exception($"Id {id} Not In Config");
            }
            int low = 0, high = idRanges.Length, mid;
            while (low <= high)
            {
                mid = (low + high) / 2;
                if (idRanges[mid].Contains(id))
                    return idRanges[mid].GetIndex(id); //查找成功，返回mid 
                if (id > idRanges[mid].startId + idRanges[mid].count)
                    low = mid + 1;  //在后半序列中查找 
                else
                    high = mid - 1; //在前半序列中查找 
            }
            return -1;//查找失败，返回-1 
        }

        public ConfigStruct GetConfigStruct(string structName)
        {
            Type t;
            try
            {
                t = Type.GetType(structName);

            }
            catch (Exception)
            {

                throw;
            }

            object obj = Activator.CreateInstance(t, new string[] { "seted new name" });

            return (ConfigStruct)obj;
        }

    }

}
