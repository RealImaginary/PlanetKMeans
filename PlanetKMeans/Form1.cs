using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CsGL.OpenGL;
using System.Runtime.InteropServices;  // DllImport Beep
using System.Diagnostics;   // Debug.writeln
//using System.Windows.Media;
//using System.Windows.Media.Media3D;


namespace PlanetKMeans
{
    public class Planet
    {
        public float[] pnt, movement;
        public int weight, id;
        public Planet(int id, float[] pnt, int weight) { this.id = id; this.pnt = pnt; this.weight = weight; }
        public Centroid cent;
    };

    public class Centroid
    {
        public float[] pnt, pntAvg;
        public int count, id;
        public Color color;
        public float distMax; // furthest planet
        public Centroid(int id, float[] pnt, Color clr)
        {
            this.id = id; this.pnt = pnt; this.color = clr; count = 0; distMax = 0;
            this.pntAvg = new float[3];
        }
    }

    
    public partial class Form1 : Form
    {
        public static bool busy = false;

        public PlanetKMeans.PlanetView view;

        List<Planet> planets = new List<Planet>();
        List<Planet> snapshot = new List<Planet>();
        List<Centroid> centroids = new List<Centroid>();
       

        public Form1()
        {
            InitializeComponent();
            view = new PlanetKMeans.PlanetView();
            this.splitContainer1.Panel2.Controls.Add(view);
            view.Dock = DockStyle.Fill;
            view.Parent = this.splitContainer1.Panel2;
            view.Focus();
            view.setSceneObjects(planets, centroids);  // unneeded
            busy = false;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (view.finished)
                this.Close();
            //view.setSceneObjects(planets, centroids);  // unneeded
            //view.glDraw();
            //if (!busy)
                view.Invalidate();
            //view.Refresh();
            //Refresh();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            planets.Clear();
        }

        private void buttonRandom_Click(object sender, EventArgs e)
        {
            planets.Clear();
            Random rnd = new Random();
            for (int i = 0; i < 25; i++)
            {
                float[] pnt = new float[3];
                pnt[0] = rnd.Next(20)-10;
                pnt[1] = rnd.Next(20)-10;
                pnt[2] = rnd.Next(20)-10;
                planets.Add(new Planet(1 + i, pnt, 1));
            }
        }

        private void buttonRandomKMeans_Click(object sender, EventArgs e)
        {
            Color[] clrs = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Aqua };

            centroids.Clear();
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                float[] pnt = new float[3];
                pnt[0] = rnd.Next(20) - 10;
                pnt[1] = rnd.Next(20) - 10;
                pnt[2] = rnd.Next(20) - 10;
                centroids.Add(new Centroid(1+1000+i, pnt, clrs[i]));
            }
        }


        float error, errorOld = 0;
        private void buttonStepKMeans_Click(object sender, EventArgs e)
        {
            foreach (Centroid cent in centroids)
            {
                cent.count = 0;
                cent.distMax = 0;
                for (int i = 0; i < cent.pntAvg.Length; i++)
                    cent.pntAvg[i] = 0f;
            }
            // Choose new centroids and sums for average
            foreach (Planet pln in planets)
            {
                float distMin = float.MaxValue; //, distMax=0;
                foreach (Centroid cent in centroids)
                {
                    float sum = 0;
                    for (int i = 0; i < cent.pnt.Length; i++)
                    {
                        float diff = cent.pnt[i] - pln.pnt[i];
                        sum += diff * diff;
                    }
                    float distance = sum; // (float)Math.Sqrt(sum);
                    if (distance < distMin)
                    {
                        distMin = distance;
                        pln.cent = cent;
                    }
                }
                for (int i = 0; i < pln.cent.pntAvg.Length; i++)
                    pln.cent.pntAvg[i] += pln.pnt[i];
                pln.cent.count++;
                if (distMin > pln.cent.distMax)
                    pln.cent.distMax = distMin;
            }
            // Move the centroids to new closest mean of planets subset
            foreach (Centroid cent in centroids)
                if (cent.count > 0)
                    for(int i=0; i<cent.pnt.Length; i++) 
                        cent.pnt[i] = cent.pntAvg[i] / cent.count;
            //RedrawPlanets();
        }
    }

    public class PlanetView : OpenGLControl
    {
        public bool light = true;				// Lighting ON/OFF
        public bool blend = true;				// Blending OFF/ON? ( NEW )
        public bool lp = false;					// L Pressed?
        public bool fp = false;					// F Pressed?
        public bool bp = false;					// B Pressed? ( NEW )

        public float xrot = 0.0f;				// X-axis rotation
        public float yrot = 0.0f;				// Y-axis rotation
        public float zrot = 0.0f;
        public float xspeed = 0.0f;				// X Rotation Speed
        public float yspeed = 0.0f;				// Y Rotation Speed
        public float z = -30.0f;					// Depth Into The Screen
        public float rotDownX, rotDownY;

        // Lighting components for the cube
        public float[] LightAmbient = { 0.5f, 0.5f, 0.5f, 1.0f };
        public float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        public float[] LightPosition = { 10000.0f, 10000.0f, 10000.0f, 1.0f };

        public int filter = 0;					// Which Filter To Use
        public uint[] texture = new uint[3];	// Texture array

        public bool finished;
        Point pntDown;
        int mousePickId;
        uint mode = GL.GL_RENDER;
        float spinCentroid = 0.0f;

        public GLUquadric quadric = GL.gluNewQuadric();
        public float[] ambientLight = { 0.1f, 0.1f, 0.1f, 1.0f };	// ambient light
        public float[] diffuseLight = { 1.0f, 1.0f, 1.0f, 1.0f };	// diffuse light
        public float[] specularLight = { 1.0f, 1.0f, 1.0f, 1.0f };	// specular light
        public float[] spotlightPosition = { 10000.0f, 10000.0f, 10000.0f, 1.0f };	// spotlight position
        public float[] spotlightDirection = { 0.0f, 0.0f, 0.0f };	// point spotlight downwards

        public float[] matAmbient = { 1.0f, 1.0f, 1.0f, 1.0f };	// ambient material
        public float[] matDiff = { 1.0f, 1.0f, 1.0f, 1.0f };		// diffuse material
        public float[] matSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };	// specular material

        protected override void InitGLContext()
        {
            LoadTextures();

            //GL.glEnable(GL.GL_TEXTURE_2D);									// Enable Texture Mapping
            GL.glShadeModel(GL.GL_SMOOTH);									// Enable Smooth Shading
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);						// Black Background
            GL.glClearDepth(1.0f);											// Depth Buffer Setup
            GL.glEnable(GL.GL_DEPTH_TEST);									// Enables Depth Testing
            GL.glDepthFunc(GL.GL_LEQUAL);									// The Type Of Depth Testing To Do
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);		// Really Nice Perspective Calculations

            if (false)
            {
                //GL.glLightfv(GL.GL_LIGHT1, GL.GL_AMBIENT, this.LightAmbient);	// Setup The Ambient Light
                GL.glLightfv(GL.GL_LIGHT1, GL.GL_DIFFUSE, this.LightDiffuse);	// Setup The Diffuse Light
                GL.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, this.LightPosition);	// Position The Light
                GL.glEnable(GL.GL_LIGHT1);										// Enable Light One
            }
            else
            {
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, ambientLight);		// setup the ambient element
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, diffuseLight);		// the diffuse element
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, spotlightPosition);	// place the light in the world
                //GL.glLightf(GL.GL_LIGHT0, GL.GL_SPOT_CUTOFF, 40.0f);
                //GL.glLightf(GL.GL_LIGHT0, GL.GL_SPOT_EXPONENT, 30.0f);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPOT_DIRECTION, spotlightDirection);
                GL.glEnable(GL.GL_LIGHT0);
            }

            if (this.light)													// If lighting, enable it to start
                GL.glEnable(GL.GL_LIGHTING);

            if (this.blend)													// If blending, turn it on and depth testing off
            {
                GL.glEnable(GL.GL_BLEND);
                GL.glDisable(GL.GL_DEPTH_TEST);
            }

            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE);						// Set The Blending Function For Translucency
        }

        protected bool LoadTextures()
        {
            Bitmap image = null;
            string file = @"Data\Glass.bmp";
            try
            {
                // If the file doesn't exist or can't be found, an ArgumentException is thrown instead of
                // just returning null
                image = new Bitmap(file);
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Could not load " + file + ".  Please make sure that Data is a subfolder from where the application is running.", "Error", MessageBoxButtons.OK);
                this.finished = true;
            }
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glGenTextures(3, this.texture);

                // Create Nearest Filtered Texture
                GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[0]);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB, image.Width, image.Height, 0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                // Create Linear Filtered Texture
                GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[1]);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB, image.Width, image.Height, 0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                // Create MipMapped Texture
                GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[2]);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR_MIPMAP_NEAREST);
                GL.gluBuild2DMipmaps(GL.GL_TEXTURE_2D, (int)GL.GL_RGB, image.Width, image.Height, GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                image.UnlockBits(bitmapdata);
                image.Dispose();
                return true;
            }
            return false;
        }

        List<Planet> scenePlanets = new List<Planet>();
        List<Centroid> sceneCentroids = new List<Centroid>();

        public void setSceneObjects(List<Planet> planets, List<Centroid> centroids)
        {
            scenePlanets = planets;
            sceneCentroids = centroids;
        }

        public override void glDraw()
        {
            Trace.WriteLine("glDraw " + DateTime.Now);
            Form1.busy = true;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
            GL.glLoadIdentity();
            GL.glTranslatef(0.0f, 0.0f, this.z);

            GL.glRotatef(this.xrot, 1.0f, 0.0f, 0.0f);
            GL.glRotatef(this.yrot, 0.0f, 1.0f, 0.0f);

            //GL.gluQuadricDrawStyle(quadric, GL.GLU_LINE);
            //GL.glColor4f(0.5f, 0.5f, 0.5f, 0.3f);  // thin alpha shell
            ////GL.glRotatef(spinCentroid, 0.0f, 0.0f, 1.0f);
            //GL.gluSphere(quadric, 5.0f, 20, 15);
            //return;


            if (true)
            {
                GL.glEnable(GL.GL_TEXTURE_2D);									// Enable Texture Mapping
                //GL.glDisable(GL.GL_COLOR_MATERIAL);
                GL.glColor4f(1.0f, 1.0f, 1.0f, 0.5f);							// Full Brightness.  50% Alpha
                GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[filter]);
                //                GL.glDisable(GL.GL_COLOR_MATERIAL);

                GL.glLoadName(3000); // id for mouse pick
                GL.glBegin(GL.GL_QUADS);
                // Front Face
                GL.glNormal3f(0.0f, 0.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                // Back Face
                GL.glNormal3f(0.0f, 0.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                // Top Face
                GL.glNormal3f(0.0f, 1.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                // Bottom Face
                GL.glNormal3f(0.0f, -1.0f, 0.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                // Right face
                GL.glNormal3f(1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                // Left Face
                GL.glNormal3f(-1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glEnd();
            }
            if (true)
            {
                float[] no_mat = { 0.0f, 0.0f, 0.0f, 1.0f };
                float[] mat_ambient_color = { 0.0f, 1.0f, 0.0f, 1.0f };
                float[] mat_diffuse = { 1.0f, 0.0f, 0.0f, 1.0f };
                float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                float no_shininess = 0.0f;
                float low_shininess = 5.0f;
                float high_shininess = 35f; // 100.0f;
                float[] mat_emission = { 0.3f, 0.2f, 0.2f, 0.0f };

                GL.glDisable(GL.GL_TEXTURE_2D);									// Enable Texture Mapping
                GL.glEnable(GL.GL_COLOR_MATERIAL);
                GL.glColorMaterial(GL.GL_FRONT, GL.GL_AMBIENT_AND_DIFFUSE);
                //GL.glDepthMask(0);
                GL.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, mat_ambient_color);
                GL.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, mat_diffuse);
                GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, mat_specular);
                GL.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, high_shininess);
                GL.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, no_mat); //mat_emission); //
                Centroid pickCentroid = null;
                foreach(Planet pln in scenePlanets) 
                {
                    GL.glPushMatrix();
                    GL.glTranslatef(pln.pnt[0], pln.pnt[1], pln.pnt[2]);
                    //GL.glColor4f(1.0f, 1.0f, 1.0f, 0.5f);
                    Color clr = (pln.cent == null) ? Color.White : pln.cent.color;
                    GL.glColor4f(clr.R / 256.0f, clr.G / 256.0f, clr.B / 256.0f, 1.0f);
                    GL.glLoadName((uint)pln.id); // id for mouse pick
                    GL.glutSolidSphere(0.5f, 30, 30);
                    GL.glPopMatrix();
                }
                foreach (Centroid cnt in sceneCentroids)
                {
                    GL.glPushMatrix();
                    GL.glTranslatef(cnt.pnt[0], cnt.pnt[1], cnt.pnt[2]);
                    GL.glColor4f(cnt.color.R/256.0f, cnt.color.G/256.0f, cnt.color.B/256.0f, 1.0f);
                    GL.glLoadName((uint)cnt.id); // id for mouse pick
                    GL.glutSolidSphere(1.5f, 30, 30);
                    if ((mode == GL.GL_RENDER) && (mousePickId == cnt.id))
                    {
                        GL.gluQuadricDrawStyle(quadric, GL.GLU_LINE);
                        GL.glColor4f(cnt.color.R / 256.0f, cnt.color.G / 256.0f, cnt.color.B / 256.0f, 0.3f);  // thin alpha shell
                        GL.glRotatef(spinCentroid, 0.0f, 0.0f, 1.0f);
                        GL.gluSphere(quadric, Math.Sqrt(cnt.distMax), 20, 15);
                    }
                    GL.glLineWidth(2);
                    GL.glPopMatrix();
                    GL.glBegin(GL.GL_LINES);
                    GL.glVertex3f(0.0f, 0.0f, 0.0f); // origin of the line
                    GL.glVertex3f(cnt.pnt[0], cnt.pnt[1], cnt.pnt[2]);
                    GL.glEnd();
                }
            }
            this.xrot += this.xspeed;
            this.yrot += this.yspeed;
            spinCentroid += 1.0f;
            Form1.busy = false;
            Trace.WriteLine("leaving glDraw " + DateTime.Now);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Size s = Size;
            if (s.Height == 0) s.Height = 1;

            GL.glViewport(0, 0, s.Width, s.Height);

            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            GL.gluPerspective(45.0f, (double)s.Width / (double)s.Height, 0.1f, 100.0f);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
        }

        public PlanetView()
            : base()
        {
            this.KeyDown += new KeyEventHandler(LessonView_KeyDown);
            this.KeyUp += new KeyEventHandler(LessonView_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseWheeled);  // From HanziEye
            this.finished = false;
        }

        /*[DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);
        [DllImport("coredll.dll")]
        public static extern int PlaySound(string szSound, IntPtr hModule, int flags); */

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            this.xspeed = this.yspeed = 0;
            pntDown = e.Location;
            rotDownX = this.xrot;
            rotDownY = this.yrot;
            //Beep(1000, 100);
            //PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
            //PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(0x20000 | 0x0));
            ((Control)sender).Capture = true;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (((Control)sender).Capture)
            {
                // rotate
                this.xrot = rotDownX - 180 * (float)(pntDown.Y - e.Y) / this.Height;   // ?!?! transposed X/Y ?
                this.yrot = rotDownY - 180 * (float)(pntDown.X - e.X) / this.Width;
            }
            else
            {
                // Mouse Pick 
                int[] view = new int[4];
                int hits, id;
                uint[] buff = new uint[64];   buff[0]=0;
                GL.glSelectBuffer(64, buff);            // This chooses the buffer where store the values for the selection data
                GL.glGetIntegerv(GL.GL_VIEWPORT, view);    // This retrieves info about the viewport
                GL.glRenderMode(GL.GL_SELECT); mode = GL.GL_SELECT;     // Switching in selecton mode
                GL.glInitNames();                       // Clearing the name's stack. This stack contains all the info about the objects
                GL.glPushName(0);                       // Now fill the stack with one element (or glLoadName will generate an error)
                // Now modify the vieving volume, restricting selection area around the cursor
 	            GL.glMatrixMode(GL.GL_PROJECTION); GL.glPushMatrix(); GL.glLoadIdentity();
                GL.gluPickMatrix(e.X, Size.Height-e.Y, 1.0f, 1.0f, view); // Project/magnify/restrict the draw to an area around the cursor
                GL.gluPerspective(45.0f, (double)Size.Width / (double)Size.Height, 0.1f, 100.0f);  // To apply the pickMatrix
                GL.glMatrixMode(GL.GL_MODELVIEW);
                glDraw();                              // draw only the names in the stack, and fill the array
         		GL.glMatrixMode(GL.GL_PROJECTION); GL.glPopMatrix();
                GL.glMatrixMode(GL.GL_MODELVIEW);
                hits = GL.glRenderMode(GL.GL_RENDER); mode = GL.GL_RENDER;   // get number of objects drawed in that area and return to render mode
             	//list_hits(hits, buff);
                mousePickId = (hits>0) ? (int)buff[3] : -1;
                Debug.WriteLine(hits + " hits " + buff[0] + " " + buff[3]);
            }
            this.Invalidate();
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            ((Control)sender).Capture = false;
            ((Control)sender).Invalidate();
        }

        protected void LessonView_KeyDown(object Sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)				// Finish the application if the escape key was pressed
                this.finished = true;
            else if (e.KeyCode == Keys.L && !this.lp)	// On the L key, flip the lighting mode
            {
                this.lp = true;
                this.light = !this.light;
                if (this.light)
                    GL.glEnable(GL.GL_LIGHTING);
                else
                    GL.glDisable(GL.GL_LIGHTING);
            }
            else if (e.KeyCode == Keys.F && !this.fp)	// On the F key, cycle the texture filter (texture used)
            {
                this.fp = true;
                this.filter = (filter + 1) % 3;
            }
            else if (e.KeyCode == Keys.B && !this.bp)	// Blending code starts here
            {
                this.bp = true;
                this.blend = !this.blend;
                if (this.blend)
                {
                    GL.glEnable(GL.GL_BLEND);			// Turn Blending On
                    GL.glDisable(GL.GL_DEPTH_TEST);		// Turn Depth Testing Off
                }
                else
                {
                    GL.glDisable(GL.GL_BLEND);			// Turn Blending Off
                    GL.glEnable(GL.GL_DEPTH_TEST);		// Turn Depth Testing On
                }
            }											// Blending Code Ends Here
            else if (e.KeyCode == Keys.PageUp)			// On page up, move out
                this.z -= 0.5f;
            else if (e.KeyCode == Keys.PageDown)		// On page down, move in
                this.z += 0.5f;
        }

        private void MouseWheeled(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {   // Contrast Middle
            }
            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {   // Rotation
            }
            else
            {
                this.z += (e.Delta>0) ? -0.5f : 0.5f;
            }
        }


        private void LessonView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.L)					// Release the lighting toggle key lock
                this.lp = false;
            else if (e.KeyCode == Keys.F)				// Release the filter cycle key lock
                this.fp = false;
            else if (e.KeyCode == Keys.B)				// Release the blending toggle key lock
                this.bp = false;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Up)						// Change rotation about the x axis
                this.xspeed -= 0.01f;
            else if (keyData == Keys.Down)
                this.xspeed += 0.01f;
            else if (keyData == Keys.Right)				// Change rotation about the y axis
                this.yspeed += 0.01f;
            else if (keyData == Keys.Left)
                this.yspeed -= 0.01f;

            return base.ProcessDialogKey(keyData);
        }
    }

    
}
