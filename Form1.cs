using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace Géoloc
{
    public partial class Form1 : Form
    {
        private AsyncServer asyncServer = null;
        private List<Telephone> listeTels;
        private GMapOverlay calqueMarqueurs = new GMapOverlay(); 

        public Form1()
        {
            InitializeComponent();

            // Initialisation de la carte (objet GMapControl mis sur le form)
            mainMap.MapProvider = GMapProviders.OpenStreetMap;
            // Ajout du calque des marqueurs
            mainMap.Overlays.Add(calqueMarqueurs);
            // Ou est centrée la carte
            mainMap.Position = new PointLatLng(47.315758, 5.093609);
            // Là c'est un peu au pif...
            mainMap.MinZoom = 0;
            mainMap.MaxZoom = 24;
            mainMap.Zoom = 9;
            mainMap.CanDragMap = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listeTels = new List<Telephone>();

            asyncServer = new AsyncServer(8888, listeTels, this);
            asyncServer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            asyncServer.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            asyncServer.Stop();
        }

        public void affTel()
        {
            calqueMarqueurs.Clear();
            foreach (Telephone item in listeTels)
            {
                GMapMarker mark = new GMarkerGoogle(new PointLatLng(item.latitude, item.longitude), GMarkerGoogleType.green_small);
                mark.ToolTipMode = MarkerTooltipMode.Always;
                mark.ToolTipText = item.id;
                calqueMarqueurs.Markers.Add(mark);
            }
        }
    }
}
