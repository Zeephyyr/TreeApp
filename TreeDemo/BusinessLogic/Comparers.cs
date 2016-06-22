using System.Collections.Generic;
using DataTransfer;

namespace BusinessLogic
{
    public class AscComparer:IComparer<NodeResponseDto>
    {
        public int Compare(NodeResponseDto x, NodeResponseDto y)
        {
            if (x.DigitValue > y.DigitValue)
                return 1;
            if (x.DigitValue < y.DigitValue)
                return -1;
            return 0;
        }
    }

    public class DescComparer : IComparer<NodeResponseDto>
    {
        public int Compare(NodeResponseDto x, NodeResponseDto y)
        {
            if (x.DigitValue < y.DigitValue)
                return 1;
            if (x.DigitValue > y.DigitValue)
                return -1;
            return 0;
        }
    }
}
