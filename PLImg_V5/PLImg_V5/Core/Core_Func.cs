using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineControl.Camera.Dalsa;


namespace PLImg_V2
{
    partial class Core
    {
        Func<byte[], int, Image<Gray, byte>> FnBuff2Img( int bufH, int bufW ) {
            Func<byte[],int,Image<Gray , byte>> output = new Func<byte[],int,Image<Gray, byte>>((data,bufcount)=> {
                Image<Gray, byte> buffImgData = new Image<Gray, byte>(bufW,bufH*bufcount);
                buffImgData.Bytes = data;
                return buffImgData;
            } );
            return output;
        }

        Action<ScanConfig> RunStgBuffer; 
        Action<ScanConfig> StopStgBuffer; 
        


        public void SaveImageData( Emgu.CV.UI.ImageBox[,] imgbox, string savepath ) {
            try
            {
                for ( int i = 0; i < imgbox.GetLength( 0 ); i++ )
                {
                    for ( int j = 0; j < imgbox.GetLength( 1 ); j++ )
                    {
                        if ( imgbox[i, j].Image != null )
                        {
                            string temp = i.ToString( "D2" ) + "_"+j.ToString( "D3" );
                            string outpath = System.IO.Path.Combine( savepath, temp );
                            imgbox[i, j].Image.Save( String.Format( outpath + ".bmp" ) );
                        }
                    }
                }

            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.ToString() );

            }
        }

        #region indicator
        double SV( double[] input ) {
            var zscore = Idc.Zscore(input);
            var vari = Idc.Variance(zscore());
            return vari();
        }

        #endregion


    }
}
