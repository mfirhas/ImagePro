using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCD
{
    public class ConvMask
    {
        public ConvMask() { }

        public int atasKiri = 0, atasTengah = 0, atasKanan = 0;
        public int tengahKiri = 0, tengah = 1, tengahKanan = 0;
        public int bawahKiri = 0, bawahTengah = 0, bawahKanan = 0;
        public int factor = 1;
        public int offset = 0;
        public void setAll(int nVal)
        {
            atasKiri = atasTengah = atasKanan = tengahKiri = tengah = tengahKanan = bawahKiri = bawahTengah = bawahKanan = nVal;
        }

        
    }
}
