using DefineEnum;
using DefineStruct;
namespace DataConfig
{
    public enum templeteXLSXId
    {
        burning = 1,
    }
    public class templeteXLSX
    {
        public int id;
        public string BuffName = "DefaultBuffName";
        public byte BuffLv = 1;
        public string BuffIntro;
        public int Damage = 0;
        public byte Duration;
        public float Bonus = 1;
        public ABILLITY MainAttribute = ABILLITY.NULL;
        public ManaUseLevel? ManaUsage;
    }
}