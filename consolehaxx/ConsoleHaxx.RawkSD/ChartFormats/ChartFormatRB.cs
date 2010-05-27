﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleHaxx.RawkSD
{
	public class ChartFormatRB : IChartFormat
	{
		public const string ChartFile = "chart";
		public const string PanFile = "pan";
		public const string WeightsFile = "weights";
		public const string MiloFile = "milo";

		public static ChartFormatRB Instance;
		public static void Initialise()
		{
			Instance = new ChartFormatRB();
			Platform.AddFormat(Instance);
		}

		public override int ID {
			get { return 0x47; }
		}

		public override string Name {
			get { return "Rock Band MIDI Chart"; }
		}

		public void Create(FormatData data, Stream chart, Stream pan, Stream weights, Stream milo, bool expertplus, bool fixforquickplay)
		{
			data.SetStream(this, ChartFile, chart);
			data.SetStream(this, PanFile, pan);
			data.SetStream(this, WeightsFile, weights);
			data.SetStream(this, MiloFile, milo);

			data.Song.Data.SetValue("RBChartExpertPlus", expertplus);
			data.Song.Data.SetValue("RBChartFixForQuickplay", fixforquickplay);
		}

		public override bool Writable {
			get { return true; }
		}

		public override bool Readable {
			get { return true; }
		}

		public override ChartFormat DecodeChart(FormatData data, ProgressIndicator progress)
		{
			if (!data.HasStream(this, ChartFile))
				throw new FormatException();

			Stream stream = data.GetStream(this, ChartFile);
			ChartFormat chart = ChartFormat.Create(stream);
			data.CloseStream(stream);
			return chart;
		}

		public override void EncodeChart(ChartFormat data, FormatData destination, ProgressIndicator progress)
		{
			Stream stream = destination.AddStream(this, ChartFile);
			data.Save(stream);
			destination.CloseStream(stream);
		}

		public bool NeedsFixing(FormatData data)
		{
			return data.Song.Data.GetValue<bool>("RBChartFixForQuickplay") || data.Song.Data.GetValue<bool>("RBChartExpertPlus");
		}
	}
}
