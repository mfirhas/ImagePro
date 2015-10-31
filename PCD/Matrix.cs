using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCD
{
    public static class Matrix
    {
        public static double[,] Mean3x3
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, }, 
                  { 1, 1, 1, }, 
                  { 1, 1, 1, }, };
            }
        }

        public static double[,] Mean5x5
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, 1, 1}, 
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1}, };
            }
        }

        public static double[,] Mean7x7
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, 1, 1, 1, 1}, 
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1}, };
            }
        }

        public static double[,] Modus3x3
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, }, 
                  { 1, 1, 1, }, 
                  { 1, 0, 0, }, };
            }
        }

        public static double[,] Modus5x5
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, 1, 1}, 
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1},
                  { 1, 1, 1, 0, 0},
                  { 1, 0, 0, 0, 0}, };
            }
        }

        public static double[,] Modus7x7
        {
            get
            {
                return new double[,]  
                { { 1, 1, 1, 1, 1, 1, 1}, 
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 1, 1},
                  { 1, 1, 1, 1, 1, 0, 0},
                  { 1, 1, 1, 0, 0, 0, 0},
                  { 1, 1, 1, 0, 0, 0, 0}, };
            }
        }

        public static double[,] LaplaceMask
        {
            get
            {
                return new double[,]  
                { { 0, 1, 0, }, 
                  { 1,-4, 1, }, 
                  { 0, 1, 0, }, };
            }
        }

        public static double[,] RobertMask
        {
            get
            {
                return new double[,]  
                { {-1, 0,-1, }, 
                  { 0, 2, 0, }, 
                  { 0, 0, 0, }, };
            }
        }

        public static double[,] PrewitMask
        {
            get
            {
                return new double[,]  
                { {-2,-1, 0, }, 
                  {-1, 0, 1, }, 
                  { 0, 1, 2, }, };
            }
        }

        public static double[,] SobelMask
        {
            get
            {
                return new double[,]  
                { {-2,-2, 0, }, 
                  {-2, 0, 2, }, 
                  { 0, 2, 2, }, };
            }
        }

        public static double[,] FreiChanMask
        {
            get
            {
                return new double[,]  
                { {-2,-(Math.Sqrt(2)), 0, }, 
                  {-(Math.Sqrt(2)), 0, (Math.Sqrt(2)), }, 
                  { 0, (Math.Sqrt(2)), 2, }, };
            }
        }

        public static double[,] aFreiChanMask
        {
            get
            {
                return new double[,]  
                { {-2,-2, 0, }, 
                  {-2, 0, 2, }, 
                  { 0, 2, 2, }, };
            }
        }

        #region 
        

        #endregion
    }
}
