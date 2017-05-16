using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineControl.Camera.Interface
{
    public interface Cam<Tbuff,Tout>
    {
        Dictionary<string , int> CamNum { get; set; }
        Action Connect( string camNum , dynamic mode );
        Action Disconnect();
        Action Grab();
        Action Freeze();
        Action BuffClear();
        Func<Tbuff, Tout> BuffGetAll();
        Func<Tbuff, Tout> BuffGetLine();
        Action<double> Exposure();
        Action<double> LineRate();
    }
}
