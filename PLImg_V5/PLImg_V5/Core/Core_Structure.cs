using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLImg_V2
{
    public delegate void TferbyteArr( byte[] imgarr );
    public delegate void TferImgArr( Image<Gray , byte> img );
    public delegate void TferTrgImgArr( Image<Gray , byte> img , int lineNum);
    public delegate void TferScanStatus();
    public delegate void TferSplitImgArr( Image<Gray , Byte> img , int lineNum , int unitNum );
    public delegate void TferFeedBackPos( double[] XYZPos );
    public delegate void TferNumber( double num );

    public enum ScanTypes { NonTrig, Trig }
    public enum ScanMode { MultiLine, SingleLine, TrgCustom, Trg2Inch, Trg4Inch };
    public enum ScanState { Start, Pause, Stop, Wait }
}
