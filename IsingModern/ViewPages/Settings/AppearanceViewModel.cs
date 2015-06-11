using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using IsingModern.Model;

namespace IsingModern.ViewPages.Settings {
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class AppearanceViewModel
        : NotifyPropertyChanged {
        private const string FontSmall = "small";
        private const string FontLarge = "large";

        private readonly Color[] _accentColors = new Color[]{
                Color.FromRgb(0x60, 0xa9, 0x17),   // green
                Colors.DeepSkyBlue,
                Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
                Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
          };
        private readonly Color[] _accentColorsDarker = new Color[]{
                Color.FromRgb(0x00, 0x6a, 0x00),   // emerald
                Colors.DarkBlue,
                Color.FromRgb(0xaa, 0x00, 0xff),   // violet
                Color.FromRgb(0xe5, 0x14, 0x00),   // red
        };


        private Color _selectedAccentColor;
        private readonly LinkCollection _themes = new LinkCollection();
        private Link _selectedTheme;
        private string _selectedFontSize;

        public AppearanceViewModel() {
            // add the default themes
            this._themes.Add(new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource });
            this._themes.Add(new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource });

            this.SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;
            SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        private void SyncThemeAndColor() {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            this.SelectedTheme = this._themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            this.SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor") {
                SyncThemeAndColor();
            }
        }

        public LinkCollection Themes {
            get { return this._themes; }
        }

        public string[] FontSizes {
            get { return new string[] { FontSmall, FontLarge }; }
        }


        private readonly int[] sizes = new int[] { 50, 100, 200, 400 };
        public int[] MaxSizes {
            get { return sizes; }
        }

        public Color[] AccentColors {
            get { return this._accentColors; }
        }

        public Link SelectedTheme {
            get { return this._selectedTheme; }
            set {
                if(this._selectedTheme != value) {
                    this._selectedTheme = value;
                    OnPropertyChanged("SelectedTheme");


                    //Update Lattice
                    Spin.PointColors.Remove(0);
                    if(value.DisplayName == "dark") {
                        Spin.PointColors.Add(0, Colors.White);
                    } else {
                        Spin.PointColors.Add(0, Colors.Black);
                    }

                    // Update Interface
                    IsingRender.Current.SwitchTheme(value.DisplayName == "dark");
                    // Update Lattice Render
                    IsingRender.RefreshRender();

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                }
            }
        }

        public string SelectedFontSize {
            get { return this._selectedFontSize; }
            set {
                if(this._selectedFontSize != value) {
                    this._selectedFontSize = value;
                    OnPropertyChanged("SelectedFontSize");
                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
                }
            }
        }

        private int _selectedMaxSize = 100;
        public int SelectedMaxSize {
            get { return this._selectedMaxSize; }
            set {
                this._selectedMaxSize = value;
                OnPropertyChanged("SelectedMaxSize");
                IsingRender.Current.SliderMax = Array.IndexOf(sizes, value) + 1;
            }
        }

        public Color SelectedAccentColor {
            get { return this._selectedAccentColor; }
            set {
                int index = Array.IndexOf(this._accentColors, this._selectedAccentColor);
                if(index != -1) {
                    Spin.PointColors.Remove(1);
                    Spin.PointColors.Remove(-1);
                    Spin.PointColors.Add(1, _accentColorsDarker[index]);
                    Spin.PointColors.Add(-1, _selectedAccentColor);
                    IsingRender.RefreshRender();

                    IsingRender.Current.SwitchAccentColor(_selectedAccentColor, _accentColorsDarker[index]);
                }
                if(this._selectedAccentColor != value) {
                    this._selectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");
                    AppearanceManager.Current.AccentColor = value;
                }
            }
        }


    }
}
