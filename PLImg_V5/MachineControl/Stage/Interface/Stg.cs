using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.VisaNS;

namespace MachineControl.Stage.Interface
{
    public enum ConnectMode
    {
        IP, Com
    }
    public interface Stg
    {
        Dictionary<string , int> Axis { get; set; }
        Action Connect( string path , ConnectMode mode );
        Action Disconnect();
        Action Enable( string axis );
        Action Disable( string axis );
        Action Home( string axis );
        Action<double> Moveabs( string axis );
        Action<double> Moverel( string axis );
        Action<double,double> WaitEps( string axis );
        Action<double> SetSpeed( string axis );
        Func<double> GetPos( string axis );
    }
}
