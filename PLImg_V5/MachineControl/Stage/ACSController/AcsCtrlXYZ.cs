using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.VisaNS;
using System.Runtime.InteropServices;
using SPIIPLUSCOM660Lib;
using MachineControl.Stage.Interface;
using static LanguageExt.Prelude;

namespace MachineControl.Stage.ACSController
{
    
    public class AcsCtrlXYZ : Stg
    {
        #region global
        public Dictionary<string , int> Axis { get; set; }
        public Action Connect( string path , ConnectMode mode ) {
            return act(()=> {
                switch ( mode ) {
                    case ConnectMode.IP:
                        Ch.CloseComm();
                        Ch.OpenCommEthernetTCP( path , Ch.ACSC_SOCKET_STREAM_PORT );
                        System.Threading.Thread.Sleep( 80 );
                        break;
                    case ConnectMode.Com:
                        Ch.CloseComm();
                        Ch.OpenCommSerial( Convert.ToInt32( path ) , -1 );
                        System.Threading.Thread.Sleep( 80 );
                        break;
                };
            } );
        }
        public Action Disconnect() {
            return act( () => {
                Ch.CloseComm();
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action Enable( string axis ) {
            return act( () => {
                Ch.Enable( Axis[axis] , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action Disable( string axis ) {
            return act( () => {
                Ch.Disable( Axis[axis] , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action Home( string axis ) {
            return act( () => {
                Ch.RunBuffer( Axis[axis] , "" , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action<double> Moveabs( string axis ){
            return act( (double point) => {
                Ch.ToPoint( 0 , Axis[axis] , point , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action<double> Moverel( string axis ){
            return act( ( double point ) => {
                Ch.ToPoint( Ch.ACSC_AMF_RELATIVE , Axis[axis] , point , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Action<double,double> WaitEps( string axis ) {
            return act( (double targetPos , double epsilon ) => {
                System.Threading.Thread.Sleep( 300 );
                while ( true )
                {
                    double error = Math.Abs( targetPos - Ch.GetFPosition( Axis[axis], Ch.ACSC_SYNCHRONOUS, ref pWait ) );
                    if ( error < epsilon ) break;
                }
            } );
        }
        public Action<double> SetSpeed( string axis ) {
            return act( (double speed) =>
            {
                Ch.SetVelocity( Axis[axis] , speed , Ch.ACSC_ASYNCHRONOUS , ref pWait );
                System.Threading.Thread.Sleep( 80 );
            } );
        }
        public Func<double> GetPos( string axis ) {
            return fun( () => {
                var temp =Ch.GetFPosition( Axis.Values.ElementAt( Axis[axis] ) , Ch.ACSC_SYNCHRONOUS , ref pWait );
                return Ch.GetFPosition( Axis.Values.ElementAt( Axis[axis] ) , Ch.ACSC_SYNCHRONOUS , ref pWait );
            } );
        }

        public void StartTrigger(int buffnum) {
            Ch.RunBuffer( buffnum, "", Ch.ACSC_ASYNCHRONOUS, ref pWait );
            System.Threading.Thread.Sleep( 30 );
        }
        public void StopTrigger( int buffnum )
        {
            Ch.StopBuffer( buffnum, Ch.ACSC_ASYNCHRONOUS, ref pWait );
            System.Threading.Thread.Sleep( 30 );
        }

        #endregion

        #region Local
        SPIIPLUSCOM660Lib.AsyncChannel Ch;
        object pWait = 0;

        public AcsCtrlXYZ()
        {
            Ch = new SPIIPLUSCOM660Lib.AsyncChannel();
            Axis = new Dictionary<string , int>();
            Axis.Add( "Y" , 0 );
            Axis.Add( "X" , 1 );
            Axis.Add( "Z" , 2 );
        }
        #endregion
    }
}
