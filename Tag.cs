using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace DaTag
{
    [ComVisible(true), Guid("B9836559-B663-46c4-BC0B-E343EADA7B60")]
    public class Tag
    {
        public enum STATUS : int { EMPTY, RESERVED, PREPAID, AGREED };

        public int xStart, yStart, xEnd, yEnd;
        public string text;
        public STATUS status;

        public Tag()
        {
            xStart = yStart = xEnd = yEnd = 0;
            text = "";

            status = STATUS.EMPTY;
        }
    }
}
