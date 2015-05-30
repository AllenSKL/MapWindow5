﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Api;
using MW5.Api.Enums;
using MW5.Api.Interfaces;
using MW5.Api.Legend.Abstract;
using MW5.Plugins.Concrete;
using MW5.Plugins.Interfaces;
using MW5.UI.Menu;

namespace MW5.Menu
{
    internal class MenuUpdater: MenuServiceBase
    {
        private readonly IAppContext _context;
        private readonly IMuteMap _map;
        private readonly IMuteLegend _legend;

        public MenuUpdater(IAppContext context, PluginIdentity identity) : base(context, identity)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
            _map = context.Map;
            _legend = context.Legend;
        }

        public void Update(bool rendered)
        {
            UpdateToolbars(rendered);

            UpdateMenu();
        }

        private void UpdateMenu()
        {
            var layer = _legend.Layers.Current;
            FindMenuItem(MenuKeys.RemoveLayer).Enabled = layer != null;
            FindMenuItem(MenuKeys.LayerClearSelection).Enabled = layer != null && layer.IsVector;
            FindMenuItem(MenuKeys.ClearLayers).Enabled = _map.Layers.Any();
        }

        private void UpdateToolbars(bool rendered)
        {
            // mapControl plays the role of the model here
            FindToolbarItem(MenuKeys.ZoomIn).Checked = _map.MapCursor == MapCursor.ZoomIn;
            FindToolbarItem(MenuKeys.ZoomOut).Checked = _map.MapCursor == MapCursor.ZoomOut;
            FindToolbarItem(MenuKeys.Pan).Checked = _map.MapCursor == MapCursor.Pan;

            FindToolbarItem(MenuKeys.SelectByRectangle).Checked = _map.MapCursor == MapCursor.Selection;
            FindToolbarItem(MenuKeys.SelectByPolygon).Checked = _map.MapCursor == MapCursor.SelectByPolygon;

            bool selectionCursor = _map.MapCursor == MapCursor.Selection ||
                                   _map.MapCursor == MapCursor.SelectByPolygon;
            FindToolbarItem(MenuKeys.SelectDropDown).Checked = selectionCursor;

            bool distance = _map.Measuring.Type == MeasuringType.Distance;
            FindToolbarItem(MenuKeys.MeasureArea).Checked = _map.MapCursor == MapCursor.Measure && !distance;
            FindToolbarItem(MenuKeys.MeasureDistance).Checked = _map.MapCursor == MapCursor.Measure && distance;

            var item = FindToolbarItem(MenuKeys.SetProjection);
            item.Enabled = !_context.Map.Layers.Any();
            if (rendered)
            {
                item.Text = item.Enabled
                    ? "Set coordinate system and projection"
                    : "It's not allowed to change projection when layers are already added to the map.";
            }

            bool hasFeatureSet = false;

            bool hasLayer = _context.Legend.SelectedLayerHandle != -1;
            if (hasLayer)
            {
                var fs = _context.Map.Layers.Current.FeatureSet;
                if (fs != null)
                {
                    FindToolbarItem(MenuKeys.ClearSelection).Enabled = fs.NumSelected > 0;
                    FindToolbarItem(MenuKeys.ZoomToSelected).Enabled = fs.NumSelected > 0;
                    hasFeatureSet = true;
                }
            }

            if (!hasFeatureSet)
            {
                FindToolbarItem(MenuKeys.ClearSelection).Enabled = false;
                FindToolbarItem(MenuKeys.ZoomToSelected).Enabled = false;
            }

            FindToolbarItem(MenuKeys.RemoveLayer).Enabled = hasLayer;

            //toolSearch.Enabled = true;
            //toolSearch.Text = "Find location";
            //if (App.Map.Count > 0 && !App.Map.Measuring.IsUsingEllipsoid)
            //{
            //    toolSearch.Enabled = false;
            //    toolSearch.Text = "Unsupported projection. Search isn't allowed.";
            //}
        }
    }
}
