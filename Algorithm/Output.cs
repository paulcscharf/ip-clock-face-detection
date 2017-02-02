using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

// This file was created for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    /// <summary>
    /// This class contains a list of OutputParts, and is returned after executing an algorithm
    /// </summary>
    class Output
    {
        private List<OutputPart> parts = new List<OutputPart>();

        public Output()
        {

        }

        /// <summary>
        /// Adds a bitmap to the list of OutputPars, also requires a name to identify
        /// </summary>
        public void Add(Bitmap bitmap, string name)
        {
            parts.Add(new OutputPart(bitmap, name));
        }

        /// <summary>
        /// Provides read-only access to the internal list of OutputParts
        /// </summary>
        public OutputPart this[int i]
        {
            get { return parts[i]; }
        }

        /// <summary>
        /// Number of included OutputParts
        /// </summary>
        public int Count
        {
            get { return parts.Count; }
        }

        /// <summary>
        /// This method has the Output add all its images to a given combo box
        /// </summary>
        public void AddToComboBox(ComboBox cBox)
        {
            cBox.Items.AddRange(parts.ToArray());
            cBox.SelectedItem = parts.Last();
        }
    }

    /// <summary>
    /// This class includes a named bitmap and should only be used by the class Output
    /// </summary>
    class OutputPart
    {
        /// <summary>
        /// The bitmap in this OutputPart
        /// </summary>
        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// The name of this OutputPart
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructor of OutputPart, takes a bitmap and a name
        /// </summary>
        public OutputPart(Bitmap bitmap, string name)
        {
            Bitmap = bitmap;
            Name = name;
        }

        /// <summary>
        /// Returns the name of the OutputPart
        /// </summary>
        public override string ToString()
        {
            return "  " + Name;
        }
    }
}
