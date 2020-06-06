namespace TransXChange.Common.Extensions
{
    public static class BoolExtensions
    {
        public static int ToInt(this bool baseBool)
        {
            if (baseBool)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}