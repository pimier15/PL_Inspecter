using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace MachineControl.MathClac
{
    public class Indicator
    {
        public Func<double[]> Zscore(double[] input) {
            return new Func<double[]>( ()=> {
                var mean = input.Sum() / input.GetLength(0);
                var vari = input.Select( x => Math.Pow((x - mean),2) ).Sum()  / input.GetLength(0);
                /* -Parellel-
                double[] output = new double[input.GetLength(0)];
                Parallel.For( 0 , input.GetLength( 0 ) , i => { output[i] = (input[i] - mean)/vari } );
                return output;
                */
                return input.Select( x=> (x - mean)/vari).ToArray();
            } );
        }

        public Func<double> Variance( double[] input ) {
            return new Func<double>( ()=> {
                var mean = input.Sum() / input.GetLength(0);
                return input.Select( x => Math.Pow((x - mean),2) ).Sum()/ input.GetLength(0);
            } );
        }



    }
}
