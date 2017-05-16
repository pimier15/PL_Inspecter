using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.VisaNS;
using DALSA.SaperaLT.SapClassBasic;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace MachineControl.Camera.Dalsa
{
    

    public class DalsaPiranha3_12k : MachineControl.Camera.Interface.Cam<SapBuffer,byte[]>
    {
        #region Global
        public Dictionary<string , int> CamNum { get; set; }

        public Action BuffClear()
        {
            return new Action(()=> Buffers.Clear() );
        }

        public Func<SapBuffer,byte[]> BuffGetAll( )
        {
            return new Func<SapBuffer,byte[]>( ( SapBuffer buff ) =>
            {
                byte[] output = new byte[buff.Width*buff.Height];
                GCHandle outputAddr = GCHandle.Alloc( output, GCHandleType.Pinned); // output 의 주소 만듬
                IntPtr pointer = outputAddr.AddrOfPinnedObject(); // 
                buff.ReadRect( 0 , 0 , buff.Width , buff.Height , pointer );
                Marshal.Copy( pointer , output , 0 , output.Length );
                outputAddr.Free();
                return output;
            } );
        }

        public Func<SapBuffer,byte[]> BuffGetLine()
        {
            return new Func<SapBuffer,byte[]>( ( SapBuffer buff ) =>
            {
                byte[] output = new byte[buff.Width];
                GCHandle outputAddr = GCHandle.Alloc(output, GCHandleType.Pinned); // output 의 GC주소 만듬
                IntPtr pointer = outputAddr.AddrOfPinnedObject();
                int readnum = 0;
                buff.ReadLine( 0 , 0 , buff.Width -1 , 0 , pointer , out readnum );
                Marshal.Copy( pointer , output , 0 , output.Length ); // Pointer에서 가리키는 첫번쨰 메모리 주소에서부터 Length 만큼 카피를 한다.
                outputAddr.Free();
                return output;
            } );
        }

        public Action Connect( string path , dynamic mode )
        {
            return new Action(( )=> {
                Disconnect()();
                //mbSession = ( MessageBasedSession ) ResourceManager.GetLocalManager().Open( path );
                //LoadSetting();
                //SaveSetting();
                LoadConfig( mode );
                CreateCamObj();
            } );
        }

        public Action Disconnect()
        {
            return new Action( ()=> {
                if ( Xfer != null )
                {
                    Xfer.Destroy();
                    Xfer.Dispose();
                }

                if ( AcqDevice != null )
                {
                    AcqDevice.Destroy();
                    AcqDevice.Dispose();
                }

                if ( Acquisition != null )
                {
                    Acquisition.Destroy();
                    Acquisition.Dispose();
                }

                if ( Buffers != null )
                {
                    Buffers.Destroy();
                    Buffers.Dispose();
                }

                if ( View != null )
                {
                    View.Destroy();
                    View.Dispose();
                }
                if ( ServerLocation != null ) ServerLocation.Dispose();
            } );
        }

        public Action<double> Exposure()
        {
            return new Action<double>( (double value)=> {
                //mbSession.Query( "set " + value.ToString() + "\r" );
            } );
        }

        public Action Freeze()
        {
            return new Action( () => { if ( Xfer.Grabbing ) Xfer.Freeze(); } );
        }

        public Action Grab()
        {
            return new Action( () => { if ( !Xfer.Grabbing ) Xfer.Grab(); } );
        }

        public Action<double> LineRate()
        {
            return new Action<double>( ( double value ) => {
                //mbSession.Query( "ssf " + value.ToString() + "\r" );
            } );
        }

        public Action ExposureMode( int value ) {
            return new Action( ( ) => {
               //mbSession.Query( "sem " + value.ToString() + "\r" );
            } );
        }


        #endregion

        #region Local
        MessageBasedSession mbSession       ;
        SapLocation     ServerLocation      ;
        SapAcqDevice    AcqDevice           ;
        SapAcquisition  Acquisition         ;
        SapView         View                ;
        public SapBuffer       Buffers      ;
        public SapAcqToBuf     Xfer         ;

        public string ConfigFileName        ;
        public string ConfigFile            ;
        public int    ResourceIndex         ;
        public string ServerName            ;
        #endregion

        public DalsaPiranha3_12k() {
        }

        public void EvtResist( SapAcqToBuf xfer , SapXferNotifyHandler evtFunc )
        {
            xfer.XferNotify += new SapXferNotifyHandler( evtFunc );
            xfer.XferNotifyContext = View;
            Buffers.Create();
            xfer.Create();
        }

        public Dictionary<string,int> GetBuffWH() {
            Dictionary<string,int> output = new Dictionary<string,int>();
            output.Add("H", Buffers.Height);
            output.Add("W", Buffers.Width) ;
            return output;
        }

        #region Config
        void LoadConfig( ScanConfig mode ) {
            ServerName    = DalsaCam_ccf_Data.ServerName;
            ResourceIndex = DalsaCam_ccf_Data.ResourceIndex;

            // If need custom path, insert config file path to ConfigFileName 
            switch ( mode ) {
                //case ScanConfig.nonTrigger:
                //    ConfigFileName = DalsaCam_ccf_Data.ConfigFile_Non;
                //    ConfigFile = DalsaCam_ccf_Data.ConfigFileNameBase + ConfigFileName;
                //    break;

                case ScanConfig.Trigger_1:
                    ConfigFileName = DalsaCam_ccf_Data.ConfigFile_1;
                    ConfigFile = DalsaCam_ccf_Data.ConfigFileNameBase + ConfigFileName;
                    break;

                case ScanConfig.Trigger_2:
                    ConfigFileName = DalsaCam_ccf_Data.ConfigFile_2;
                    ConfigFile = DalsaCam_ccf_Data.ConfigFileNameBase + ConfigFileName;
                    break;

                case ScanConfig.Trigger_4:
                    ConfigFileName = DalsaCam_ccf_Data.ConfigFile_4;
                    ConfigFile = DalsaCam_ccf_Data.ConfigFileNameBase + ConfigFileName;
                    break;
            }
        }



        void LoadSetting()
        {
            String KeyPath = "Software\\Teledyne DALSA\\Sapera LT\\SapAcquisition";
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(KeyPath);
            if ( RegKey != null )
            {
                ServerName = RegKey.GetValue( "Server" , "" ).ToString();
                ResourceIndex = ( int ) RegKey.GetValue( "Resource" , 0 );
                if ( File.Exists( RegKey.GetValue( "ConfigFile" , "" ).ToString() ) )
                    ConfigFile = RegKey.GetValue( "ConfigFile" , "" ).ToString();
                ConfigFileName = Path.GetFileName( ConfigFile );
            }
        }
        void SaveSetting()
        {
            String KeyPath = "Software\\Teledyne DALSA\\" + Assembly.GetExecutingAssembly().ToString() + "\\SapAcquisition";
            RegistryKey RegKey = Registry.CurrentUser.CreateSubKey(KeyPath);
            RegKey.SetValue( "Server" , ServerName );
            RegKey.SetValue( "ConfigFile" , ConfigFile );
            RegKey.SetValue( "Resource" , ResourceIndex );
        }
        void CreateCamObj() {
            ServerLocation = new SapLocation( ServerName , ResourceIndex );
            Acquisition = new SapAcquisition( ServerLocation , ConfigFile );

            if ( SapBuffer.IsBufferTypeSupported( ServerLocation , SapBuffer.MemoryType.ScatterGather ) )
                Buffers = new SapBufferWithTrash( 2 , Acquisition , SapBuffer.MemoryType.ScatterGather );
            else
                Buffers = new SapBufferWithTrash( 2 , Acquisition , SapBuffer.MemoryType.ScatterGatherPhysical );

            Acquisition.Create();

            Xfer = new SapAcqToBuf( Acquisition , Buffers );
            Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;

            View = new SapView( Buffers );
        }


        #endregion

        #region Config File control
        //public DalsaCam_ccf_Data LoadConfigFile( ) {
        //    OpenFileDialog ofd = new OpenFileDialog();
        //    if ( ofd.ShowDialog() == DialogResult.OK )
        //    {
        //        using ( StreamReader sr = new StreamReader(ofd.FileName , Encoding.UTF8)) {
        //            XmlSerializer xs = new XmlSerializer(typeof(DalsaCam_ccf_Data));
        //            return (DalsaCam_ccf_Data)xs.Deserialize( sr );
        //        }
        //    }
        //    return null;
        //}

        #endregion
    }
}
