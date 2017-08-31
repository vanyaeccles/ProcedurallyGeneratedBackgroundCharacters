using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// This is the pseudorandom number generator, takes in a seed and generates uniform or gaussian random numbers
// Owned by each agent

// adapted from the C code PRNG library found at http://paulbourke.net/miscellaneous/random/randomlib.c


public class PRNG : MonoBehaviour {


    double[] u = new double[97];
    double c, cd, cm;
    int i97, j97;
    int test = 0;
	


    public void RandomInitialise(int ij, int kl)
    {
        double s, t;
        int ii, i, j, k, l, jj, m;


        // handle initialisation errors
        // ij range must be 0-31328
        // kl range must be 0-30081

        if (ij < 0 || ij > 31328 || kl < 0 || kl > 30081)
        {
            ij = 1802;
            kl = 9373;
        }



        i = (ij / 177) % 177 + 2;
        j = (ij % 177) + 2;
        k = (kl / 169) % 178 + 1;
        l = (kl % 169);

        for (ii = 0; ii < 97; ii++)
        {
            s = 0.0;
            t = 0.5;
            for (jj = 0; jj < 24; jj++)
            {
                m = (((i * j) % 179) * k) % 179;
                i = j;
                j = k;
                k = m;
                l = (53 * l + 1) % 169;
                if (((l * m % 64)) >= 32)
                    s += t;
                t *= 0.5;
            }
            u[ii] = s;
        }

        c = 362436.0 / 16777216.0;
        cd = 7654321.0 / 16777216.0;
        cm = 16777213.0 / 16777216.0;
        i97 = 97;
        j97 = 33;
        test = 1;
    }


    /* 
   This is the random number generator proposed by George Marsaglia in
   Florida State University Report: FSU-SCRI-87-50
   */
    double RandomUniform()
    {
        double uni;

        /* Make sure the initialisation routine has been called */
        if (test == 0)
            RandomInitialise(1802, 9373);

        uni = u[i97 - 1] - u[j97 - 1];
        if (uni <= 0.0)
            uni++;
        u[i97 - 1] = uni;
        i97--;
        if (i97 == 0)
            i97 = 97;
        j97--;
        if (j97 == 0)
            j97 = 97;
        c -= cd;
        if (c < 0.0)
            c += cm;
        uni -= c;
        if (uni < 0.0)
            uni++;

        return (uni);
    }



    /*
  ALGORITHM 712, COLLECTED ALGORITHMS FROM ACM.
  THIS WORK PUBLISHED IN TRANSACTIONS ON MATHEMATICAL SOFTWARE,
  VOL. 18, NO. 4, DECEMBER, 1992, PP. 434-435.
  The function returns a normally distributed pseudo-random number
  with a given mean and standard devaiation.  Calls are made to a
  function subprogram which must return independent random
  numbers uniform in the interval (0,1).
  The algorithm uses the ratio of uniforms method of A.J. Kinderman
  and J.F. Monahan augmented with quadratic bounding curves.
*/

    public double RandomGaussian(double mean, double stddev)
    {
        double q, u, v, x, y;


        /*  
            Generate P = (u,v) uniform in rect. enclosing acceptance region 
          Make sure that any random numbers <= 0 are rejected, since
          gaussian() requires uniforms > 0, but RandomUniform() delivers >= 0.
        */
        do
        {
            u = RandomUniform();
            v = RandomUniform();
            if (u <= 0.0 || v <= 0.0)
            {
                u = 1.0;
                v = 1.0;
            }
            v = 1.7156 * (v - 0.5);

            /*  Evaluate the quadratic form */
            x = u - 0.449871;
            y = Math.Abs(v) + 0.386595;
            q = x * x + y * (0.19600 * y - 0.25472 * x);

            /* Accept P if inside inner ellipse */
            if (q < 0.27597)
                break;

            /*  Reject P if outside outer ellipse, or outside acceptance region */
        } while ((q > 0.27846) || (v * v > -4.0 * Math.Log(u) * u * u));

        /*  Return ratio of P's coordinates as the normal deviate */
        return (mean + stddev * v / u);
    }




    // test the PRNG
    void RunTest()
    {
        int i;
        double r, rmin = 1e32, rmax = -1e32;

        /* Generate 20000 random numbers */
        RandomInitialise(1802, 9373);
        for (i = 0; i < 20000; i++)
        {
            r = RandomUniform();
            if (r < rmin) rmin = r;
            if (r > rmax) rmax = r;
        }

        //Debug.Log(stderr + "Numbers range from %g to %g\n", rmin, rmax);

        /*
           If the random number generator is working properly, 
           the next six random numbers should be:
              6533892.0  14220222.0  7275067.0
              6172232.0  8354498.0   10633180.0
        */
        for (i = 0; i < 6; i++)
            Debug.Log( 4096.0 * 4096.0 * RandomUniform() + " \n");
    }






    

}
