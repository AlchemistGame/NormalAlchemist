public class NewTestUIViewModel : BaseModel
{

    private dynamic _teststring_one;
    public dynamic TestString_One
    {
        get
        {
            return _teststring_one;
        }
        set
        {
            _teststring_one = value;
            TryCallHandle("TestString_One", "CallBindStringToText", value);
        }
    }
}