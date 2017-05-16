using MachineControl.Camera.Dalsa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLImg_V2.Data
{
    public class TriggerScanData
    {
        public Dictionary<ScanConfig,double> StartYPos;
        public Dictionary<ScanConfig,double> StartXPos;
        public Dictionary<ScanConfig,double> EndYPos;
        public double XStep_Size       = 28.35;
        public double Scan_Stage_Speed = 17;
        public double Camera_Exposure  = 800;
        public double Camera_LineRate  = 1200;

        public TriggerScanData() { CreateEndPoint(); }
        public TriggerScanData(
            double startYPos        = 49 ,
            double startXPos        = 49 ,
            double xStep_Size       = 28.2 ,
            double scan_Stage_Speed = 17,
            double camera_Exposure  = 2400,
            double camera_LineRate  = 400
            )
        {
            XStep_Size       = xStep_Size ;
            Scan_Stage_Speed = scan_Stage_Speed;
            Camera_Exposure  = camera_Exposure;
            Camera_LineRate  = camera_LineRate;

            CreateEndPoint();
        }

        void CreateEndPoint() {
            StartYPos = new Dictionary<ScanConfig, double>();
            StartYPos.Add( ScanConfig.Trigger_1, 137 );
            StartYPos.Add( ScanConfig.Trigger_2, 137 );
            StartYPos.Add( ScanConfig.Trigger_4, 161 );

            StartXPos = new Dictionary<ScanConfig, double>();
            StartXPos.Add( ScanConfig.Trigger_1, 60 );
            StartXPos.Add( ScanConfig.Trigger_2, 72  );
            StartXPos.Add( ScanConfig.Trigger_4, 20  );

            EndYPos = new Dictionary<ScanConfig , double>();
            EndYPos.Add( ScanConfig.Trigger_1 , 112 );
            EndYPos.Add( ScanConfig.Trigger_2 , 80  );
            EndYPos.Add( ScanConfig.Trigger_4 , 60  );

        }
    }
}
