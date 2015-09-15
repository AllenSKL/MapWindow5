﻿// -------------------------------------------------------------------------------------------
// <copyright file="GdalTool.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2015
// </copyright>
// -------------------------------------------------------------------------------------------

using System.Linq;
using MW5.Tools.Model;
using MW5.Tools.Model.Parameters;

namespace MW5.Gdal.Tools
{
    public abstract class GdalTool : GisTool, IGdalTool
    {
        public abstract string AdditionalOptions { get; set; }

        public StringParameter AdditionalOptionsParameter
        {
            get { return Parameters.FirstOrDefault(p => p.Name == "AdditionalOptions") as StringParameter; }
        }

        public abstract string GetOptions(bool mainOnly = false);
    }
}