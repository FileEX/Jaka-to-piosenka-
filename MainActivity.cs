using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using System;
using Android.Content;
using Android.Widget;

namespace Jaka_to_piosenka
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;

            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            Button button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += Button_Click;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GameActivity));
            StartActivity(intent);
        }
    }
}