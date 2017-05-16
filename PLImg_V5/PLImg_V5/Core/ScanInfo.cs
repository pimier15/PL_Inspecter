using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLImg_V2
{
    public class ScanInfo
    {
        public double PsXStart;
        public double PsYStart;
        public double PsYEnd  ;

        public int BuffW = 12000;
        public int BuffH = 1000;
        //public int BuffW = 12288;
        //public int BuffH = 1024;

        public readonly int OneUnitBuffNum = 12;
        public readonly int OneLineBuffNum = 48;
        public double XStep = 28.3; // Unit um

        public int BuffLimit = 11;
        public int UnitLimit = 3;
        public int LineLimit = 3;

        public int ScanSpeed = 10;

        public void SetPos(int xstart,int ystart,int yend,double xStep = 0 ) {
            PsXStart = xstart;
            PsYStart = ystart;
            PsYEnd   = yend;
            XStep    = xStep;
        }
        public void SetBufInfo(int bufW,int bufH) {
            if( bufH > 0 ) BuffH = bufH;
            if( bufW > 0 ) BuffW = bufW;
        }

        public void SetLimit(int buf,int unit, int line = 0) {
            if( buf  >= 0 )  this.BuffLimit = buf; 
            if( unit >= 0 )  this.UnitLimit = unit;
            if( line >= 0 )  this.LineLimit = line;
        }
        public void SetScanSpeed( int sped ) {
            if ( sped > 0 ) ScanSpeed = sped;
        }
    }
}
