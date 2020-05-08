using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace shock
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			axShockwaveFlash1.Movie = @"C:\worktemp\uppod115.swf";
			axShockwaveFlash1.Play();
		}
	}
}
