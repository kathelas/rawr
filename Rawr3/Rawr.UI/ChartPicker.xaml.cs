﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Rawr.UI
{
    public partial class ChartPicker : UserControl
    {
        public GraphDisplay GraphDisplay { get; set; }

        public List<ChartPickerItem> Items { get; set; }

        public static readonly DependencyProperty PrimaryItemProperty =
            DependencyProperty.Register("PrimaryItem", typeof(ChartPickerItem), typeof(ChartPicker), new PropertyMetadata(null));
        public ChartPickerItem PrimaryItem
        {
            get { return (ChartPickerItem)GetValue(PrimaryItemProperty); }
            set { SetValue(PrimaryItemProperty, value); }
        }


        public ChartPicker()
        {
            InitializeComponent();
            this.Items = new List<ChartPickerItem>(new ChartPickerItem[]
            {
                new ChartPickerItem("Gear", "Head", "Neck", "Shoulders", "Back", "Chest", "Wrists", "Hands", "Waist", "Legs", "Feet", "Finger 1", "Finger 2", "Trinket 1", "Trinket 2", "Main Hand", "Off Hand", "Ranged", "Projectile", "Projectile Bag"),
                new ChartPickerItem("Enchants", "Head", "Shoulders", "Back", "Chest", "Wrists", "Hands", "Legs", "Feet", "Finger 1", "Finger 2", "Main Hand", "Off Hand", "Ranged"),
                new ChartPickerItem("Gems", "All Normal", "Red", "Blue", "Yellow", "Meta"),
                new ChartPickerItem("Buffs", "All", "Food", "Flasks and Elixirs", "Scrolls", "Potions", "Raid Buffs", "Set Bonuses"),
                new ChartPickerItem("Talents and Glyphs", "Individual Talents", "Talent Specs", "Glyphs"),
                new ChartPickerItem("Equipped", "Gear", "Enchants", "Buff"),
                new ChartPickerItem("Available", "Gear", "Enchants"),
                new ChartPickerItem("Direct Upgrades", "Gear", "Enchants"),
                new ChartPickerItem("Stat Values", "Relative Stat Values"),
                //new ChartPickerItem("{0} Specific")
            });
            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);

            this.DataContext = this;
        }

        void Calculations_ModelChanged(object sender, EventArgs e)
        {
            bool specificWasSelected = false;
            ChartPickerItem last = Items.Last();
            if (last.Header.EndsWith(" Specific"))
            {
                specificWasSelected = PrimaryItem == last;
                Items.Remove(last);
            }
            last = new ChartPickerItem(Calculations.Instance.Name + " Specific", Calculations.CustomChartNames);
            Items.Add(last);
            if (specificWasSelected)
            {
                PrimaryItem = last;
                SetCurrentGraph();
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChartPickerItem mousedOverItem = ((FrameworkElement)sender).DataContext as ChartPickerItem;
            PrimaryItem = mousedOverItem;
            ListBoxSecondary.SelectedItem = PrimaryItem.SelectedItem;
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChartPickerItem clickedItem = ((FrameworkElement)sender).DataContext as ChartPickerItem;
            if (ListBoxSecondary.Items.Contains(clickedItem))
                PrimaryItem.SelectedItem = clickedItem;
            SetCurrentGraph();
            Close();
        }

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void SetCurrentGraph()
        {
            TextBlockChartButtonPrimary.Text = PrimaryItem.Header;
            TextBlockChartButtonSecondary.Text = PrimaryItem.SelectedItem.Header;
            GraphDisplay.CurrentGraph = string.Format("{0}|{1}", PrimaryItem, PrimaryItem.SelectedItem);
        }

        private void Close()
        {
            PrimaryItem = Items.FirstOrDefault(chartPickerItem => chartPickerItem.Header == GraphDisplay.CurrentGraph.Split('|')[0]);
            PopupChartPicker.IsOpen = false;
        }

        public class ChartPickerItem : DependencyObject
        {
            public static readonly DependencyProperty ItemsProperty =
                DependencyProperty.Register("Items", typeof(ChartPickerItem[]), typeof(ChartPickerItem), new PropertyMetadata(null));
            public ChartPickerItem[] Items
            {
                get { return (ChartPickerItem[])GetValue(ItemsProperty); }
                set { SetValue(ItemsProperty, value); }
            }

            public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register("Header", typeof(string), typeof(ChartPickerItem), new PropertyMetadata(null));
            public string Header
            {
                get { return (string)GetValue(HeaderProperty); }
                set { SetValue(HeaderProperty, value); }
            }

            public static readonly DependencyProperty SelectedItemProperty =
                DependencyProperty.Register("SelectedItem", typeof(ChartPickerItem), typeof(ChartPickerItem), new PropertyMetadata(null));
            public ChartPickerItem SelectedItem
            {
                get { return (ChartPickerItem)GetValue(SelectedItemProperty); }
                set { SetValue(SelectedItemProperty, value); }
            }

            public ChartPickerItem() { }
            public ChartPickerItem(string header, params string[] children)
            {
                Header = header;
                Items = children.Select(child => new ChartPickerItem(child)).ToArray();
                SelectedItem = Items.FirstOrDefault();
            }

            public override string ToString()
            {
                return Header;
            }
        }
    }
}