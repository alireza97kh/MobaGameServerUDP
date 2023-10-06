namespace Dobeil
{
    [System.Serializable]
    public class NumberRange
    {
        public int Min;
        public int Max;

        public NumberRange()
        {
            Min = 0;
            Max = 0;
        }

        public NumberRange(int _min, int _max)
        {
            Min = _min;
            Max = _max;
        }
    }
}