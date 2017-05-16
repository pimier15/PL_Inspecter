using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT;
using Accord.Math;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using MachineControl.Camera.Dalsa;

namespace PLImg_V2
{
    enum StageEnableState {
        Enabled,
        Disabled
        
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Core Core = new Core();
        public SeriesCollection seriesbox { get; set; }
        public ChartValues<int> chartV { get; set; }
        ImageBox[,] ImgBoxArr;
        ImageBox[] TrgImgBoxArr;
        Dictionary<ScanConfig, System.Windows.Controls.RadioButton> SampleConfig;
        Dictionary<string,StageEnableState> StgState;
       
        public MainWindow()
        {
            InitializeComponent();
            InitImgBox();
            SetImgBoxStretch();
            InitLocalData();
            DataContext = this;
            ConnectionData cd = new ConnectionData();
            Core.ConnectDevice( cd.CameraPath, cd.ControllerIP, cd.RStage )( ScanConfig.Trigger_1 );
            //Core.ConnectDevice( cd.CameraPath, cd.DctStagePort, cd.RStage );
            InitCore();
            Display_GrabStatus();
        }

        #region Display

        void DisplayTrgImg( Image<Gray , byte> img , int lineNum )
        {
            TrgImgBoxArr[lineNum].Image = img;
        }

        void DisplayAF(double input)
        {
            lblAFV.BeginInvoke(()=> lblAFV.Content = input.ToString("N4") );
        }

        void DisplayRealTime(Image<Gray, byte> img)
        {
            imgboxReal.Image = img;
            Console.WriteLine( "readl time image is comming0" );
        }
    
        void DisplayBuffNumber(int num)
        {
            lblBuffNum.BeginInvoke(() => lblBuffNum.Content = num.ToString());
        }

        void DisplayFullScanImg(Image<Gray, Byte> img, int lineNum, int unitNum)
        {
            if ( lineNum < 4 && unitNum < 4 )
            {
                ImgBoxArr[unitNum , lineNum].Image = img;
            }
        }
        void SetImgBoxStretch()
        {
            foreach (var item in ImgBoxArr)
            {
                item.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        #endregion

        #region Init

        void InitCore( )
        {
            foreach ( var item in GD.YXZ )
            {
                StgState[item] = StageEnableState.Enabled;
            }
            Core.evtRealimg       += new TferImgArr( DisplayRealTime );
            Core.evtTrgImg        += new TferTrgImgArr( DisplayTrgImg );
            Core.evtSV            += new TferNumber( DisplayAF );
            Core.evtFedBckPos     += new TferFeedBackPos( DisplayPos );
            Core.evtScanEnd       += new TferScanStatus( ( ) => { this.BeginInvoke( () => Mouse.OverrideCursor = null ); } );
            Task.Run(()=>Core.GetFeedbackPos());
            imgboxReal.SizeMode = PictureBoxSizeMode.StretchImage;
            InitViewWin();
        }

        void InitImgBox()
        {
            TrgImgBoxArr = new ImageBox[4];
            TrgImgBoxArr[0] = imgboxTrig0;
            TrgImgBoxArr[1] = imgboxTrig1;
            TrgImgBoxArr[2] = imgboxTrig2;
            TrgImgBoxArr[3] = imgboxTrig3;

            foreach ( var item in TrgImgBoxArr )
            {
                item.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            ImgBoxArr = new ImageBox[4,4];
            ImgBoxArr[0, 0] = imgboxScan00;
            ImgBoxArr[0, 1] = imgboxScan01;
            ImgBoxArr[0, 2] = imgboxScan02;
            ImgBoxArr[0, 3] = imgboxScan03;

            ImgBoxArr[1, 0] = imgboxScan10;
            ImgBoxArr[1, 1] = imgboxScan11;
            ImgBoxArr[1, 2] = imgboxScan12;
            ImgBoxArr[1, 3] = imgboxScan13;

            ImgBoxArr[2, 0] = imgboxScan20;
            ImgBoxArr[2, 1] = imgboxScan21;
            ImgBoxArr[2, 2] = imgboxScan22;
            ImgBoxArr[2, 3] = imgboxScan23;

            ImgBoxArr[3, 0] = imgboxScan30;
            ImgBoxArr[3, 1] = imgboxScan31;
            ImgBoxArr[3, 2] = imgboxScan32;
            ImgBoxArr[3, 3] = imgboxScan33;
        }

        void ClearImgBox()
        {
            for (int i = 0; i < ImgBoxArr.GetLength(0); i++)
            {
                for (int j = 0; j < ImgBoxArr.GetLength(1); j++)
                {
                    ImgBoxArr[i, j].Image = null;
                }
            }
        }

        void InitViewWin( )
        {
            nudExtime.Value = 2400;
            nudlinerate.Value = 400;
            nudScanSpeed.Value = 1;
            nudGoXPos.Value = 100;
            nudGoYPos.Value = 50;
            nudGoZPos.Value = 26.190;
        }

        void InitLocalData() {
            StgState = new Dictionary<string , StageEnableState>();
            StgState.Add("Y", new StageEnableState());
            StgState.Add("X", new StageEnableState());
            StgState.Add("Z", new StageEnableState());

            SampleConfig = new Dictionary<ScanConfig , System.Windows.Controls.RadioButton>();
            SampleConfig.Add( ScanConfig.Trigger_1  ,rdb1inch);
            SampleConfig.Add( ScanConfig.Trigger_2  ,rdb2inch);
            SampleConfig.Add( ScanConfig.Trigger_4  ,rdb4inch);
        }

        void DisplayPos(double[] inputPos)
        {
            Task.Run( ( ) => lblXpos.BeginInvoke( (Action)(( ) => lblXpos.Content = inputPos[0].ToString("N4")) ) );
            Task.Run( ( ) => lblYpos.BeginInvoke( (Action)(( ) => lblYpos.Content = inputPos[1].ToString("N4")) ) );
            Task.Run( ( ) => lblZpos.BeginInvoke( (Action)(( ) => lblZpos.Content = inputPos[2].ToString("N4")) ) );
        }
        #endregion

        #region MainWindowEvent
        void ScanStart( ) { Mouse.OverrideCursor = Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;}
        void ScanEnd( ) { Mouse.OverrideCursor = null; }

        #region Camera
        private void btnGrap_Click(object sender, RoutedEventArgs e)
        {
            if ( Core == null ) Console.WriteLine( "null ");
            Core.Grab();
        }
        private void btnFreeze_Click( object sender, RoutedEventArgs e )
        {
            Core.Freeze();
        }
        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            string savePath = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if ( fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                savePath = fbd.SelectedPath;
            }
            Core.SaveImageData( ImgBoxArr, savePath );
        }
        #endregion

        #region Stage
        // common //
        private void btnOrigin_Click( object sender, RoutedEventArgs e ) {
            foreach ( var item in GD.YXZ ) Core.Stg.Home( item )();
        }

        // XYZStage //
        private void btnYMove_Click( object sender, RoutedEventArgs e )
        {
            if ( StgState["Y"] == StageEnableState.Enabled ) Core.MoveXYstg( "Y" , ( double ) nudGoYPos.Value );
        }
        private void btnXMove_Click( object sender, RoutedEventArgs e )
        {
            if ( StgState["X"] == StageEnableState.Enabled ) Core.MoveXYstg( "X" , ( double ) nudGoXPos.Value );
        }
        private void btnZMove_Click( object sender, RoutedEventArgs e )
        {
            if( StgState["Z"] == StageEnableState.Enabled) Core.MoveZstg( ( double ) nudGoZPos.Value );
        }

      

        // R Stage //
        private void btnRMove_Click( object sender, RoutedEventArgs e )
        {
            double pulse = (double)nudGoRPos.Value * 400;
            
        }
        private void btnROrigin_Click( object sender, RoutedEventArgs e )
        {
           
        }
        private void btnRForceStop_Click( object sender, RoutedEventArgs e )
        {
            
        }
        #endregion

        #endregion

        #region Motor Enable / Disable // Done
        private void ckbYDisa_Checked( object sender , RoutedEventArgs e )
        {
            Core.Stg.Disable( "Y" )();
            StgState["Y"] = StageEnableState.Disabled;
        }
        private void ckbXDisa_Checked( object sender, RoutedEventArgs e ) {
            Core.Stg.Disable("X")();
            StgState["X"] = StageEnableState.Disabled;
        }
        private void ckbZDisa_Checked( object sender, RoutedEventArgs e ) {
            Core.Stg.Disable( "Z" )();
            StgState["Z"] = StageEnableState.Disabled;
        }
        private void ckbYDisa_Unchecked( object sender , RoutedEventArgs e )
        {
            Core.Stg.Enable( "Y" )();
            StgState["Y"] = StageEnableState.Enabled;
        }
        private void ckbXDisa_Unchecked( object sender , RoutedEventArgs e )
        {
            Core.Stg.Enable( "X" )();
            StgState["X"] = StageEnableState.Enabled;
        }
        private void ckbZDisa_Unchecked( object sender, RoutedEventArgs e ) {
            Core.Stg.Enable( "Z" )();
            StgState["Z"] = StageEnableState.Enabled;
        }

        
        #endregion

        #region Sscan data Setting 

        private void nudlinerate_KeyUp( object sender, System.Windows.Input.KeyEventArgs e ) {
            if ( e.Key != System.Windows.Input.Key.Enter ) return;
            Core.LineRate( (int)nudlinerate.Value );
        }

        private void nudExtime_KeyUp( object sender , System.Windows.Input.KeyEventArgs e )
        {
            if ( e.Key != System.Windows.Input.Key.Enter ) return;
            Core.Exposure( ( int ) nudExtime.Value );
        }

        #endregion

        #region window Event 
        private void MetroWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e ) {
            foreach ( var item in GD.YXZ )
            {
                Core.Stg.Disable( item )();
                Core.Stg.Disconnect();
            }
            Core.Cam.Freeze();
            Core.Cam.Disconnect();
        }
        #endregion

        #region Image event
        private void imgboxScan00_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan01_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan02_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan03_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan10_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan11_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan12_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan13_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan20_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan21_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan22_Click( object sender , EventArgs e )
        {

        }

        private void imgboxScan23_Click( object sender , EventArgs e )
        {

        }



        #endregion

        #region Tab Select Event
       
        #endregion

        private void btnStartFixAreaScan_Click( object sender , RoutedEventArgs e )
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Core.TrigScanData.Scan_Stage_Speed = (double)nudTrgScanSpeed.Value;
            foreach ( var item in SampleConfig )
            {
                if ( item.Value.IsChecked == true )
                {
                    ResizeTriggerImgBox( item.Key );
                    Core.StartTrigScan( item.Key );
                } 
            }
        }

        void ResizeTriggerImgBox(ScanConfig config)
        {
            switch ( config ) {
                case ScanConfig.Trigger_1:
                    windowTrig0.Width = 560;
                    imgboxTrig0.Width = 560;
                    break;

                case ScanConfig.Trigger_2:
                    windowTrig0.Width = 280;
                    windowTrig1.Width = 280;
                    imgboxTrig0.Width = 280;
                    imgboxTrig1.Width = 280;
                    break;

                case ScanConfig.Trigger_4:
                    windowTrig0.Width = 140;
                    windowTrig1.Width = 140;
                    windowTrig2.Width = 140;
                    windowTrig3.Width = 140;
                    imgboxTrig0.Width = 140;
                    imgboxTrig1.Width = 140;
                    imgboxTrig2.Width = 140;
                    imgboxTrig3.Width = 140;
                    break;
            }

        }

        private void btnSaveImg_Click( object sender, RoutedEventArgs e )
        {
            string savePath = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if ( fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                savePath = fbd.SelectedPath;
                for ( int i = 0; i < TrgImgBoxArr.GetLength(0); i++ )
                {
                    string filename = savePath + "\\" + i.ToString("D3") + ".png";
                    TrgImgBoxArr[i].Image?.Save( filename );
                }
            }
        }

        void Display_GrabStatus() {
            Task.Run( () => {
                while ( true )
                {
                    lblgrabstatus.BeginInvoke(()=> {
                        lblgrabstatus.Content = Core.Cam.Xfer.Grabbing.ToString();
                    } );
                    System.Threading.Thread.Sleep( 200 );
                }
            } );
        }
    }
}
