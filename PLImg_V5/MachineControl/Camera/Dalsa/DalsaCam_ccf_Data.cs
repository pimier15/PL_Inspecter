using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineControl.Camera.Dalsa
{
    public static class DalsaCam_ccf_Data
    {
        //public static readonly string ServerName     = "Xtium-CL_MX4_1"; 
        //public static readonly int    ResourceIndex  = 0;
        //public static readonly string ConfigFile_Non   =  "p_plimg_plimg_plimg.ccf";
        //public static readonly string ConfigFile_1     =  "E_P3-87-12K40-01-R_8-taps_InternalLineValid_12288.ccf";
        //public static readonly string ConfigFile_2     =  "E_P3-87-12K40-01-R_8-taps_InternalLineValid_24576.ccf";
        //public static readonly string ConfigFile_4     =  "E_P3-87-12K40-01-R_8-taps_InternalLineValid_49152.ccf";
        //public static readonly string ConfigFileNameBase = @"C:\Program Files\Teledyne DALSA\Sapera\CamFiles\User\";

        //TDI Setting
        public static readonly string ServerName     = "Xcelera-HS_PX8_1";
        public static readonly int    ResourceIndex  = 0;
        public static readonly string ConfigFile_Non   =  "T__tdi_non.ccf";
        public static readonly string ConfigFile_1     =  "T__tdi_1inch.ccf";
        public static readonly string ConfigFile_2     =  "T__tdi_2inch.ccf";
        public static readonly string ConfigFile_4     =  "T__tdi_4inch.ccf";
        public static readonly string ConfigFileNameBase = @"C:\Program Files\Teledyne DALSA\Sapera\CamFiles\User\";

    }
}
