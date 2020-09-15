using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EssentialUIKit.AppLayout.Models;
using EssentialUIKit.Models.Api;
using Syncfusion.XForms.Border;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace EssentialUIKit.AppLayout.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuaddieView
    {
        public QuaddieView()
        {
            InitializeComponent();
            BindingContext = AppSettings.Instance;           
            
        }
                
        public void Show(QuaddieGroup quaddie)
        {
            QuaddieLayout.Children.Clear();
            QuaddieLayout.Children.Add(GenerateQuaddieView(quaddie));
            IsVisible = true;
            MainContent.FadeTo(1);            
            MainContent.TranslateTo(MainContent.TranslationX, 0);
            ShadowView.IsVisible = true;            
        }

        public void Hide()
        {
            ShadowView.IsVisible = false;
            var fadeAnimation = new Animation(v => MainContent.Opacity = v, 1, 0);
            var translateAnimation = new Animation(v => MainContent.TranslationY = v, 0, MainContent.Height, null, () => { IsVisible = false; });

            var parentAnimation = new Animation { { 0.5, 1, fadeAnimation }, { 0, 1, translateAnimation } };
            parentAnimation.Commit(this, "HideSettings");
        }
        
        private void Button_OnClicked(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void CloseSettings(object sender, EventArgs e)
        {
            this.Hide();
        }

        public StackLayout GenerateQuaddieView(QuaddieGroup quaddie)
        {
            List<QuaddieViewModel> races = new List<QuaddieViewModel>();
            var selections = quaddie.Selections.OrderBy( sel => sel.Runner.Race.Description );
            List<IGrouping<string, SelectedRunner>> groupedByRace = selections.GroupBy(sel => sel.Runner.Race.Description).ToList();

            var layout = new StackLayout();            
            var style = new AppLayout.Views.Styles();
            var labelStyle = (Style)style["LabelStyle"];
            var headerLabelStyle = (Style)style["HeaderLabelStyle"];
            var closeIconStyle = (Style)style["CloseIconStyle"];

            layout.Margin = new Thickness(10, 10, 10, 10);
            layout.Spacing = 8;
            layout.HorizontalOptions = LayoutOptions.Center;
            layout.VerticalOptions = LayoutOptions.Center;
                        
            StackLayout stackLayout = new StackLayout();
            stackLayout.Spacing = 5;

            Button closeButton = new Button();
            closeButton.Style = closeIconStyle;
            closeButton.Margin = new Thickness(0, -5, 0, 0);
            closeButton.Clicked += Button_OnClicked;
            closeButton.HorizontalOptions = LayoutOptions.End;
            closeButton.WidthRequest = 50;            

            int i = 0;
            foreach (IGrouping<string, SelectedRunner> runners in groupedByRace)
            {                
                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label labelHeader = new Label();
                labelHeader.Text = runners.Key;
                labelHeader.Style = headerLabelStyle;

                headerGrid.Children.Add(labelHeader);
                if (i == 0)
                {
                    headerGrid.Children.Add(closeButton);
                }
                layout.Children.Add(headerGrid);

                BoxView boxView = new BoxView();
                boxView.Color = Color.FromHex("#d54008");
                boxView.HeightRequest = 2;
                boxView.HorizontalOptions = LayoutOptions.Fill;
                layout.Children.Add(boxView);

                foreach (SelectedRunner runner in runners)
                {
                    Grid grid = new Grid();                    
                    grid.RowSpacing = 5;
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    Label labelSelection = new Label();

                    if (runner.Runner.LastPriceTraded.HasValue)
                    {
                        labelSelection.Text = runner.Runner.Name + " ( $" + Math.Round(runner.Runner.LastPriceTraded.Value) + " )";
                    }
                    else
                    {
                        labelSelection.Text = runner.Runner.Name + " ( no price )";
                    }
                    
                    
                    labelSelection.Style = labelStyle;
                    grid.Children.Add(labelSelection);
                    Grid.SetColumn(labelSelection, 0);

                    Label labelSelectionBy = new Label();
                    labelSelectionBy.Text = runner.User.Name;
                    labelSelectionBy.Style = labelStyle;
                    labelSelectionBy.FontAttributes = FontAttributes.Bold;
                    grid.Children.Add(labelSelectionBy);
                    Grid.SetColumn(labelSelectionBy, 1);
                    layout.Children.Add(grid);                    
                }
                
                i++;
            }

            return layout;            
        }
    }
}