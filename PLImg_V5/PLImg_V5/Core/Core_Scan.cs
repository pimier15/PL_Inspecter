using Accord.Math;
using MachineControl.Camera.Dalsa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLImg_V2
{
    public partial class Core
    {
        public void ReadyNonTrigScan()
        {

        }

        public void ScanStart_Non()
        {

        }



        public void StartTrigScan( ScanConfig config )
        {
            
            Cam.Disconnect();
            CurrentConfig = config;
            TrigLimit = SetTriggerLimit( config );
            TrigCount = 0;
            StopStgBuffer( config );
            StgReadyTrigScan( 0, config );

            System.Threading.Thread.Sleep( 100 );
            ResetCamCofnig( config );
            Console.WriteLine( $"run stage buffer" );
            RunStgBuffer( config );
            System.Threading.Thread.Sleep( 100 );
            Grab();
            Console.WriteLine( $"grap" );
            System.Threading.Thread.Sleep( 100 );
            Console.WriteLine( $"go to end point" );
            ScanMoveXYstg( "Y", TrigScanData.EndYPos[config], TrigScanData.Scan_Stage_Speed );
            
        }

        void StgReadyTrigScan( int triggerNum, ScanConfig config )
        {
            Console.WriteLine( $"Stage Ready Line NUmber = {triggerNum}" );
            var nextYpos = TrigScanData.StartYPos[config];
            var nextXpos = TrigScanData.StartXPos[config] - TrigScanData.XStep_Size * triggerNum;
            MoveXYstg( "Y", nextYpos);
            MoveXYstg( "X", nextXpos );
            Stg.WaitEps( "Y" )( nextYpos, 0.005 );
            Stg.WaitEps( "X" )( nextXpos, 0.005 );
            Stg.SetSpeed( "Y" )( TrigScanData.Scan_Stage_Speed );
        }

        void ResetCamCofnig( ScanConfig config )
        {
            Reconnector[config]();
            Cam.EvtResist( Cam.Xfer, GrabDoneEvt_Trg );
        }

        int SetTriggerLimit( ScanConfig config )
        {
            switch ( config )
            {
                case ScanConfig.Trigger_1:
                    return 1;

                case ScanConfig.Trigger_2:
                    return 2;

                case ScanConfig.Trigger_4:
                    return 4;
                default:
                    return 1;
            }
        }
    }
}
