/*Title:      mjbWorld
Copyright (c) 1998-2007 Martin John BakerThis program is free software; you can redistribute it and/or
   modify it under the terms of the GNU General Public License
   as published by the Free Software Foundation; either version 2
   of the License, or (at your option) any later version.This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
   GNU General Public License for more details.For information about the GNU General Public License see http://www.gnu.org/To discuss this program http://sourceforge.net/forum/forum.php?forum_id=122133
   also see website http://www.euclideanspace.com/
   */
namespace mjbModel
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Collections;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using AxmjboglLib;/// <summary>
    /// a class to represent a rotation, internally the class may code the rotation    as an
    /// axis angle:
    /// http://www.euclideanspace.com/maths/geometry/rotations/axisAngle/index.htm
    /// or a quaternion:
    /// http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/transforms/index.htm
    /// or as euler angles
    /// http://www.euclideanspace.com/maths/geometry/rotations/euler/index.htm
    /// </summary>
    class sfrotation : property
    {/// <summary>
        /// defines the resolution at which the rotation will be saved to file
        /// </summary>
        public static bool saveAsDouble = false;/// <summary>
        /// x element of axis angle or quaternion
        /// </summary>
        public double x;/// <summary>
        /// y element of axis angle or quaternion
        /// </summary>
        public double y;/// <summary>
        /// z element of axis angle or quaternion
        /// </summary>
        public double z;/// <summary>
        /// angle element of axis angle or w element of quaternion
        /// </summary>
        public double angle;/// <summary>
        /// VRML always uses axis-angle to represent rotations
        /// but quaternions are more efficient for some applications
        /// </summary>
        public int coding = (int)cde.CODING_AXISANGLE;
        /// <summary>
        /// possible values for coding variable
        /// </summary>
        public enum cde
        {
            CODING_AXISANGLE,
            CODING_QUATERNION,
            CODING_EULER,
            CODING_AXISANGLE_SAVEASQUAT,
            CODING_QUATERNION_SAVEASQUAT,
            CODING_EULER_SAVEASQUAT
        };
        /// <summary>
        /// constructor which allows initial value to be suplied as axis angle
        /// </summary>
        /// <param name="x1">x dimention of normalised axis</param>
        /// <param name="y1">y dimention of normalised axis</param>
        /// <param name="z1">z dimention of normalised axis</param>
        /// <param name="a1">angle</param>
        public sfrotation(double x1, double y1, double z1, double a1)
        {
            x = x1;
            y = y1;
            z = z1;
            angle = a1;
        }/// <summary>
        /// constructor which allows initial value to be suplied as axis angle,quaternion
        /// or axis angle as defined by c1 whoes possible values are given by enum cde
        /// </summary>
        /// <param name="x1">if quaternion or axis angle holds x dimention    of normalised axis</param>
        /// <param name="y1">if quaternion or axis angle holds y dimention    of normalised axis</param>
        /// <param name="z1">if quaternion or axis angle holds z dimention    of normalised axis</param>
        /// <param name="a1">if quaternion holds w, if axis angle holds    angle</param>
        /// <param name="c1">possible values are given by enum cde</param>
        public sfrotation(double x1, double y1, double z1, double a1, int c1)
        {
            x = x1;
            y = y1;
            z = z1;
            angle = a1;
            coding = c1;
        }/// <summary>
        /// constructor to create sfrotation from euler angles.
        /// </summary>
        /// <param name="heading">rotation about z axis</param>
        /// <param name="attitude">rotation about y axis</param>
        /// <param name="bank">rotation about x axis</param>
        public sfrotation(double heading, double attitude, double bank)
        {
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            angle = c1c2 * c3 + s1s2 * s3;
            x = c1c2 * s3 - s1s2 * c3;
            y = c1 * s2 * c3 + s1 * c2 * s3;
            z = s1 * c2 * c3 - c1 * s2 * s3;
            coding = (int)cde.CODING_QUATERNION;
            saveAsDouble = false;
        }/// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="in1"></param>
        public sfrotation(sfrotation in1)
        {
            x = (in1 != null) ? in1.x : 0;
            y = (in1 != null) ? in1.y : 0;
            z = (in1 != null) ? in1.z : 1;
            angle = (in1 != null) ? in1.angle : 0;
            coding = (in1 != null) ? in1.coding : (int)cde.CODING_AXISANGLE;
        }/// <summary>
        /// constructor
        /// </summary>
        public sfrotation()
        {
        }/// <summary>
        /// calculates the effect of this rotation on a point
        /// the new point is given by=q * P1 * q'
        /// this version does not alter P1 but returns the result.
        /// 
        /// for theory see:
        /// http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/transforms/index.htm
        /// </summary>
        /// <param name="point">point to be transformed</param>
        /// <returns>translated point</returns>
        public sfvec3f getTransform(sfvec3f p1)
        {
            double wh = angle;
            double xh = x;
            double yh = y;
            double zh = z;
            if (coding == (int)cde.CODING_AXISANGLE)
            {
                double s = Math.Sin(angle / 2);
                xh = x * s;
                yh = y * s;
                zh = z * s;
                wh = Math.Cos(angle / 2);
            }
            sfvec3f p2 = new sfvec3f();
            p2.x = wh * wh * p1.x + 2 * yh * wh * p1.z - 2 * zh * wh * p1.y + xh * xh * p1.x + 2 * yh * xh * p1.y + 2 * zh * xh * p1.z - zh * zh * p1.x - yh * yh * p1.x;
            p2.y = 2 * xh * yh * p1.x + yh * yh * p1.y + 2 * zh * yh * p1.z + 2 * wh * zh * p1.x - zh * zh * p1.y + wh * wh * p1.y - 2 * xh * wh * p1.z - xh * xh * p1.y;
            p2.z = 2 * xh * zh * p1.x + 2 * yh * zh * p1.y + zh * zh * p1.z - 2 * wh * yh * p1.x - yh * yh * p1.z + 2 * wh * xh * p1.y - xh * xh * p1.z + wh * wh * p1.z;
            return p2;
        }/// <summary>
        /// calculates the effect of this rotation on a point
        /// the new point is given by=q * P1 * q'
        /// this version returns the result in p1
        /// 
        /// for theory see:
        /// http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/transforms/index.htm
        /// </summary>
        /// <param name="point">point to be transformed</param>
        public void transform(sfvec3f p1)
        {
            double wh = angle;
            double xh = x;
            double yh = y;
            double zh = z;
            if (coding == (int)cde.CODING_AXISANGLE)
            {
                double s = Math.Sin(angle / 2);
                xh = x * s;
                yh = y * s;
                zh = z * s;
                wh = Math.Cos(angle / 2);
            }
            double resultx = wh * wh * p1.x + 2 * yh * wh * p1.z - 2 * zh * wh * p1.y + xh * xh * p1.x + 2 * yh * xh * p1.y + 2 * zh * xh * p1.z - zh * zh * p1.x - yh * yh * p1.x;
            double resulty = 2 * xh * yh * p1.x + yh * yh * p1.y + 2 * zh * yh * p1.z + 2 * wh * zh * p1.x - zh * zh * p1.y + wh * wh * p1.y - 2 * xh * wh * p1.z - xh * xh * p1.y;
            double resultz = 2 * xh * zh * p1.x + 2 * yh * zh * p1.y + zh * zh * p1.z - 2 * wh * yh * p1.x - yh * yh * p1.z + 2 * wh * xh * p1.y - xh * xh * p1.z + wh * wh * p1.z;
            p1.x = resultx;
            p1.y = resultx;
            p1.z = resultx;
        }/// <summary>
        /// static method to return type of parameter as used in VRML
        /// </summary>
        /// <returns>type of parameter as used in VRML</returns>
        public static string vrmlType_s()
        {
            return "SFRotation";
        }/// <summary>
        /// method to return type of parameter as used in VRML, need non static method    so
        /// that it can be overridden
        /// </summary>
        /// <returns>type of parameter as used in VRML</returns>
        public override string vrmlType()
        {
            return "SFRotation";
        }/// <summary>
        /// get a class that can edit this
        /// </summary>
        /// <returns>class that can edit this</returns>
        public static Type getEditClass()
        {
            return typeof(sfrotationEditor);
        }/// <summary>
        /// override of clone method for this class
        /// </summary>
        /// <returns>clone of this</returns>
        public override property clone()
        {
            //Console.WriteLine("sfparam.clone");
            return new sfrotation(this);
        }/// <summary>
        /// create an array of rotations type with a size given by the parameter
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override property[] createArray(int size)
        {
            return new sfrotation[size];
        }/// <summary>
        /// invert direction of rotation
        /// </summary>
        public void minus()
        {
            if (coding == (int)cde.CODING_AXISANGLE)
            {
                angle = -angle;
                return;
            }
            x = -x;
            y = -y;
            z = -z;
        }/// <summary>
        /// get a clone of the rotation
        /// </summary>
        /// <returns></returns>
        public sfrotation getMinus()
        {
            if (coding == (int)cde.CODING_AXISANGLE) return new sfrotation(x, y, z, -angle, coding);
            return new sfrotation(-x, -y, -z, angle, coding);
        }/// <summary>
        /// set the axis of rotation
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="tz"></param>
        public void set(double tx, double ty, double tz)
        {
            angle = Math.Sqrt(tx * tx + ty * ty + tz * tz);
            if (angle == 0) { x = 1; y = z = 0; return; }
            x = tx / angle;
            y = ty / angle;
            z = tz / angle;
        }/// <summary>
        /// set the values of this rotation
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="tz"></param>
        /// <param name="tangle"></param>
        public void set(double tx, double ty, double tz, double tangle)
        {
            x = tx;
            y = ty;
            z = tz;
            angle = tangle;
        }/// <summary>
        /// returns axis in x dimention
        /// </summary>
        /// <returns>axis in x dimention</returns>
        public double getTx()
        {
            return x * angle;
        }/// <summary>
        /// returns axis in y dimention
        /// </summary>
        /// <returns>returns axis in y dimention</returns>
        public double getTy()
        {
            return y * angle;
        }/// <summary>
        /// returns axis in z dimention
        /// </summary>
        /// <returns>returns axis in z dimention</returns>
        public double getTz()
        {
            return z * angle;
        }/// <summary>
        /// calculate total rotation by taking current rotation and then
        /// apply rotation r
        /// 
        /// if both angles are quaternions then this is a multiplication
        /// </summary>
        /// <param name="r"></param>
        public void combine(sfrotation r)
        {
            toQuaternion();
            if (r == null) return;
            double qax = x;
            double qay = y;
            double qaz = z;
            double qaw = angle;
            double qbx;
            double qby;
            double qbz;
            double qbw;

            if (r.coding == (int)cde.CODING_QUATERNION)
            {
                qbx = r.x;
                qby = r.y;
                qbz = r.z;
                qbw = r.angle;
            }
            else
            {
                double s = Math.Sin(r.angle / 2);
                qbx = r.x * s;
                qby = r.y * s;
                qbz = r.z * s;
                qbw = Math.Cos(r.angle / 2);
            }
            // now multiply the quaternions
            angle = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;
            x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
            y = qaw * qby - qax * qbz + qay * qbw + qaz * qbx;
            z = qaw * qbz + qax * qby - qay * qbx + qaz * qbw;
            coding = (int)cde.CODING_QUATERNION;
        }/// <summary>
        /// combine a rotation expressed as euler angle with current rotation.
        /// first convert both values to quaternoins then combine and convert back to    
        /// axis angle. Theory about these conversions shown here:
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/index.htm
        /// </summary>
        /// <param name="heading">angle about x axis</param>
        /// <param name="attitude">angle about y axis</param>
        /// <param name="bank">angle about z axis</param>
        public void combine(double heading, double attitude, double bank)
        {
            // first calculate quaternion qb from heading, attitude and bank
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            double qbw = c1c2 * c3 + s1s2 * s3;
            double qbx = c1c2 * s3 - s1s2 * c3;
            double qby = c1 * s2 * c3 + s1 * c2 * s3;
            double qbz = s1 * c2 * c3 - c1 * s2 * s3;
            // then convert axis-angle to quaternion if required
            toQuaternion();
            double qax = x;
            double qay = y;
            double qaz = z;
            double qaw = angle;
            // now multiply the quaternions
            angle = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;
            x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
            y = qaw * qby - qax * qbz + qay * qbw + qaz * qbx;
            z = qaw * qbz + qax * qby - qay * qbx + qaz * qbw;
            coding = (int)cde.CODING_QUATERNION;
            //Console.WriteLine("sfrotation.add(h={0} a={1} b={2} angle={3} x={4} y={5}    z={6}",heading,attitude,bank,angle,x,y,z);
        }/// <summary>
        /// if this rotation is not already coded as axis angle then convert it to axis    angle
        /// </summary>
        public void toAxisAngle()
        {
            if (coding == (int)cde.CODING_AXISANGLE) return;
            double s = Math.Sqrt(1 - angle * angle);
            if (Math.Abs(s) < 0.001) s = 1;
            angle = 2 * Math.Acos(angle);
            x = x / s;
            y = y / s;
            z = z / s;
        }/// <summary>
        /// if this rotation is not already coded as quaternion then convert it to quaternion
        /// </summary>
        public void toQuaternion()
        {
            if (coding == (int)cde.CODING_QUATERNION) return;
            double s = Math.Sin(angle / 2);
            x = x * s;
            y = y * s;
            z = z * s;
            angle = Math.Cos(angle / 2);
        }/// used when reading XML
        /// called by sfparam which is called by mfparam which is called by filter_x3d
        ///
        /// expects val to be in following format (1.0 2.0 3.0 0.1)
        public override void setAttribute(string val, string type)
        {
            try
            {
                string[] tokens = val.Split("() \t\n\r\f".ToCharArray());
                int offset = 0;
                while (tokens[offset].Equals("")) offset++;
                /* for (int t=0;t<tokens.Length;t++) {
                char[] c=tokens[t].ToCharArray();
                Console.Write("sfrotation.setAttribute("+val+","+type+")    offset"+offset+" t="+t);
                for (int p=0;p<c.Length;p++){
                int i0 =(int)c[p];
                Console.Write(" "+i0);
                }
                Console.WriteLine("");
                }*/
                if (tokens[offset].Equals(" ")) offset++;
                x = Double.Parse(tokens[offset]);
                y = Double.Parse(tokens[offset + 1]);
                z = Double.Parse(tokens[offset + 2]);
                angle = Double.Parse(tokens[offset + 3]);
            }
            catch (Exception e)
            {
                Console.WriteLine("sfrotation.setAttribute(" + val + "," + type + ")    " + e);
            }
        }/// <summary>
        /// convert x,y,z,angle to string between brackets
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat("(", x.ToString(), ",",
            y.ToString(), ",",
            z.ToString(), ",",
            angle.ToString(), ")");
        }/// <summary>
        /// call openGL mglRotated
        /// </summary>
        /// <param name="axo"></param>
        public void render3d(AxmjboglCtl axo)
        {
            if (coding == (int)cde.CODING_AXISANGLE)
            {
                axo.mglRotated(angle * 180 / Math.PI, x, y, z);
                return;
            }
            double s = Math.Sqrt(1 - angle * angle);
            if (Math.Abs(s) < 0.001) s = 1;
            axo.mglRotated(Math.Acos(angle) * 360 / Math.PI, x / s, y / s, z / s);
        }/// <summary>
        /// output as a string
        /// </summary>
        /// <param name="format">mode values
        /// 0 - output modified values
        /// 1 - output original values
        /// 2 - output attribute
        /// 3 - output attribute in brackets
        /// 4 - output with f prefix</param>
        /// <returns>string representation of this class</returns>
        public override string outstring(int format)
        {
            if (format == 3)
            {
                if (saveAsDouble)
                    return String.Concat("(", x, " ", y, " ", z, "    ", angle, ")");
                else
                    return String.Concat("(", ((float)x).ToString(), " ",
                    ((float)y).ToString(), " ",
                    ((float)z).ToString(), " ",
                    ((float)angle).ToString(), ")");
            }
            else if (format == 4)
            { // output to C
                return String.Concat(((float)angle).ToString(), "f *90/1.57,", //    convert to degrees
                ((float)x).ToString(), "f,",
                ((float)y).ToString(), "f,",
                ((float)z).ToString(), "f");
            }
            else
            {
                if (saveAsDouble)
                    return String.Concat(x, " ", y, " ", z, " ", angle);
                else
                    return String.Concat(((float)x).ToString(), " ",
                    ((float)y).ToString(), " ",
                    ((float)z).ToString(), " ",
                    ((float)angle).ToString());
            }
        }/// <summary>
        /// write to file
        /// </summary>
        /// <param name="f">information about output</param>
        /// <param name="mode">mode values
        /// 0 - output VRML97 modified values
        /// 1 - output VRML97 original values
        /// 2 - output xml (x3d)
        /// 3 - output attribute in brackets
        /// 4 - output with f prefix</param>
        /// <param name="indent"></param>
        public override void write(filter f, int mode, int indent)
        {
            toAxisAngle();
            f.write(outstring(mode));
        }/// <summary>
        /// used by mfparam.vrml2par
        /// </summary>
        /// <param name="f"></param>
        /// <param name="sfp"></param>
        /// <param name="n"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override bool instring(filter f, sfparam sfp, nodeBean n, int mode)
        {
            String s;
            try
            {
                s = f.nextToken();
                if (s != null) if (s.Equals("IS"))
                    {
                        s = f.nextToken();
                        if (sfp != null) sfp.setIs(s);
                        return true;
                    }
                x = Double.Parse(s);
                s = f.nextToken();
                y = Double.Parse(s);
                s = f.nextToken();
                z = Double.Parse(s);
                s = f.nextToken();
                angle = Double.Parse(s);
            }
            catch (Exception e)
            {
                Console.WriteLine("sfrotation.instring {0}", e);
            }
            return true;
        }/// <summary>
        /// parse string which contains rotation
        /// </summary>
        /// <param name="f"></param>
        /// <param name="s1"></param>
        /// <returns></returns>
        public bool instring(filter f, String s1)
        {
            String s;
            try
            {
                x = Double.Parse(s1);
                s = f.nextToken();
                y = Double.Parse(s);
                s = f.nextToken();
                z = Double.Parse(s);
                s = f.nextToken();
                angle = Double.Parse(s);
            }
            catch (Exception e)
            {
                Console.WriteLine("sfrotation.instring {0}", e);
            }
            return true;
        }
    }
} //namespace mjbModel