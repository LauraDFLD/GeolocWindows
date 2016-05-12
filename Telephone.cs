using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Géoloc
{
    class Telephone
    {
        public string id;
        public double latitude;
        public double longitude;

        public Telephone(string id, int lat, int lon)
        {
            this.id = id;
            this.latitude = lat;
            this.longitude = lon;
        }

        public Telephone(string data)
        {
            // Initialise un téléphone au format id;lat;long
            string[] mots = data.Split(';');
            this.id = mots[0];
            this.latitude = Convert.ToDouble(mots[1]);
            this.longitude = Convert.ToDouble(mots[2]);
        }

        public override string ToString()
        {
            return (this.id + " : " + this.latitude.ToString() + "," + this.longitude.ToString());
        }
    }
}
