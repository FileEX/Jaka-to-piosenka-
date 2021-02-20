using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Animation;
using Android.Views;
using Android.Widget;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Android.Content.Res;
using Android.Animation;
using static Android.Views.Animations.Animation;
using MikhaelLopez.CircularProgressBarLib;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace Jaka_to_piosenka
{
    public interface ICloseApplication
    {
        void closeApplication();
    }
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = (Activity)Application.Context;
            activity.FinishAffinity();
        }
    }

    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        readonly static List<MusicItem> musicTbl = new MusicData().musicTable;
        ISimpleAudioPlayer player = CrossSimpleAudioPlayer.Current;

        int intProgress;
        bool isTimer;
        bool playSong;
        bool endingGame = false;

        int GoodAnswer;
        Button[] buttons;

        int randomValue;

        List<int> userData;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;

            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            SetContentView(Resource.Layout.game_activity);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            randomValue = GetRandomSong();

            Button buttonFirst = FindViewById<Button>(Resource.Id.button1);
            Button buttonSecond = FindViewById<Button>(Resource.Id.button2);
            Button buttonThree = FindViewById<Button>(Resource.Id.button3);
            Button buttonFour = FindViewById<Button>(Resource.Id.button4);

            TextView textViewQuestion = FindViewById<TextView>(Resource.Id.titleViewText);

            var jsonData = Preferences.Get("userdata", null);

            if (jsonData != null)
                userData = JsonConvert.DeserializeObject<List<int>>(jsonData);

            if (userData == null)
                userData = new List<int>();

            TextView counter = FindViewById<TextView>(Resource.Id.counterText);

            counter.Text = "Utwór " + userData.Count + "/" + musicTbl.Count;

            buttonFirst.Click += Answer_Click;
            buttonSecond.Click += Answer_Click;
            buttonThree.Click += Answer_Click;
            buttonFour.Click += Answer_Click;

            buttonFirst.Alpha = 0f;
            buttonSecond.Alpha = 0f;
            buttonThree.Alpha = 0f;
            buttonFour.Alpha = 0f;
            textViewQuestion.Alpha = 0f;

            if (userData.Count >= musicTbl.Count)
            {
                EndGame();

                return;
            }

            SetTexts(true);
        }

        private void BtnAction(int action)
        {
            if (player.IsPlaying)
                player.Stop();

            playSong = false;

            player.Load(GetStreamFromFile(action == 1 ? "effects/good.wav" : "effects/wrong.wav"));
   
            player.Play();

            if (action == 1)
            {
                userData.Add(randomValue);
                Preferences.Set("userdata", JsonConvert.SerializeObject(userData));

                TextView counter = FindViewById<TextView>(Resource.Id.counterText);

                counter.Text = "Utwór " + userData.Count + "/" + musicTbl.Count;

                if (userData.Count >= musicTbl.Count)
                {
                    EndGame();

                    endingGame = true;

                    return;
                }
            }
        }

        private void EndGame()
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Brawo!");
            alert.SetMessage("Udało Ci się odgadnąć wszystkie piosenki! Czy chcesz zrestartować postęp?");
            alert.SetButton("Nie", (c, ev) =>
            {
                Finish();
                var closer = Xamarin.Forms.DependencyService.Get<ICloseApplication>();
                closer?.closeApplication();
            });
            alert.SetButton2("Tak", (c, ev) =>
            {
                userData.Clear();

                Preferences.Set("userdata", JsonConvert.SerializeObject(userData));

                Finish();
            });

            alert.SetCanceledOnTouchOutside(false);
            alert.Show();
        }

        private int GetRandomSong()
        {
            //Random rnd = new Random(Guid.NewGuid().GetHashCode());
            Random rnd = new Random();
            int randVal = rnd.Next(0, musicTbl.Count);

            if (userData == null)
                userData = new List<int>();

            if (userData.Contains(randVal) && userData.Count != musicTbl.Count)
            {
                GetRandomSong();
            }

            return randVal;
        }

        private void Answer_Click(object sender, EventArgs e)
        {
            Button clicked = sender as Button;

            if (clicked.Alpha != 1f)
                return;

            switch (clicked.Id)
            {
                case Resource.Id.button1:
                    if (GoodAnswer == 1)
                    {
                        BtnAction(1);
                    }
                    else
                    {
                        BtnAction(2);
                    }
                    break;
                case Resource.Id.button2:
                    if (GoodAnswer == 2)
                    {
                        BtnAction(1);
                    }
                    else
                    {
                        BtnAction(2);
                    }
                    break;
                case Resource.Id.button3:
                    if (GoodAnswer == 3)
                    {
                        BtnAction(1);
                    }
                    else
                    {
                        BtnAction(2);
                    }
                    break;
                case Resource.Id.button4:
                    if (GoodAnswer == 4)
                    {
                        BtnAction(1);
                    }
                    else
                    {
                        BtnAction(2);
                    }
                    break;
            }

            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(0.8), () =>
            {
                if (player.IsPlaying)
                    player.Stop();

                if (!endingGame)
                {
                    randomValue = GetRandomSong();

                    SetTexts(false);
                }

                return false;
            });
        }

        private void SetTexts(bool firstSet)
        {
            GoodAnswer = musicTbl[randomValue].GoodAnswerID;

            TextView textViewQuestion = FindViewById<TextView>(Resource.Id.titleViewText);

            var progressBar = FindViewById<CircularProgressBar>(Resource.Id.circularProgressBar1);

            Button buttonFirst = FindViewById<Button>(Resource.Id.button1);

            Button buttonSecond = FindViewById<Button>(Resource.Id.button2);

            Button buttonThree = FindViewById<Button>(Resource.Id.button3);

            Button buttonFour = FindViewById<Button>(Resource.Id.button4);

            player.Load(GetStreamFromFile(musicTbl[randomValue].SongFile));

            buttons = new Button[4] { buttonFirst, buttonSecond, buttonThree, buttonFour };

            isTimer = false;

            if (!firstSet)
            {
                ObjectAnimator animatorFadeOutText = ObjectAnimator.OfFloat(textViewQuestion, "Alpha", 0f);
                animatorFadeOutText.SetDuration(800);
                animatorFadeOutText.Start();

                ObjectAnimator animatorFadeOutBar = ObjectAnimator.OfFloat(progressBar, "Alpha", 0f);
                animatorFadeOutBar.SetDuration(800);
                animatorFadeOutBar.Start();

                ObjectAnimator animatorFadeInText = ObjectAnimator.OfFloat(textViewQuestion, "Alpha", 0f, 1f);
                animatorFadeInText.SetDuration(800);
                animatorFadeInText.StartDelay = 1050;
                animatorFadeInText.Start();

                ObjectAnimator animatorFadeInBar = ObjectAnimator.OfFloat(progressBar, "Alpha", 0f, 1f);
                animatorFadeInBar.SetDuration(800);
                animatorFadeInBar.StartDelay = 1050;
                animatorFadeInBar.Start();
            } else
            {
                ObjectAnimator animatorFadeInText = ObjectAnimator.OfFloat(textViewQuestion, "Alpha", 0f, 1f);
                animatorFadeInText.SetDuration(800);
                animatorFadeInText.Start();

                ObjectAnimator animatorFadeInBar = ObjectAnimator.OfFloat(progressBar, "Alpha", 0f, 1f);
                animatorFadeInBar.SetDuration(800);
                animatorFadeInBar.Start();

                textViewQuestion.Text = musicTbl[randomValue].Question;

                buttonFirst.Text = musicTbl[randomValue].AnswerOne;
                buttonSecond.Text = musicTbl[randomValue].AnswerTwo;
                buttonThree.Text = musicTbl[randomValue].AnswerThree;
                buttonFour.Text = musicTbl[randomValue].AnswerFour;
            }

            foreach (Button button in buttons)
            {
                if (!firstSet)
                {
                    ObjectAnimator animatorFadeOut = ObjectAnimator.OfFloat(button, "Alpha", 0f);
                    animatorFadeOut.SetDuration(800);
                    animatorFadeOut.Start();

                    animatorFadeOut.AnimationEnd += delegate
                    {
                        TextView textViewQuestion = FindViewById<TextView>(Resource.Id.titleViewText);

                        textViewQuestion.Text = musicTbl[randomValue].Question;

                        intProgress = 0;
                        progressBar.Progress = intProgress;

                        buttonFirst.Text = musicTbl[randomValue].AnswerOne;
                        buttonSecond.Text = musicTbl[randomValue].AnswerTwo;
                        buttonThree.Text = musicTbl[randomValue].AnswerThree;
                        buttonFour.Text = musicTbl[randomValue].AnswerFour;
                    };

                    ObjectAnimator animatorFadeIn = ObjectAnimator.OfFloat(button, "Alpha", 0f, 1f);
                    animatorFadeIn.SetDuration(800);
                    animatorFadeIn.StartDelay = 1050;
                    animatorFadeIn.Start();

                    animatorFadeIn.AnimationEnd += delegate
                    {
                        if (!isTimer)
                            InitTimer();

                        playSong = true;
                        player.Play();
                    };
                }
                else
                {
                    ObjectAnimator animatorFadeIn = ObjectAnimator.OfFloat(button, "Alpha", 0f, 1f);
                    animatorFadeIn.SetDuration(800);
                    animatorFadeIn.Start();

                    animatorFadeIn.AnimationEnd += delegate
                    {
                        if (!isTimer)
                            InitTimer();

                        playSong = true;
                        player.Play();
                    };
                }
            }
        }

        void InitTimer()
        {
            var progressBar = FindViewById<CircularProgressBar>(Resource.Id.circularProgressBar1);

            isTimer = true;

            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (player.IsPlaying && playSong && progressBar.Alpha == 1f)
                {
                    intProgress++;

                    progressBar.SetProgressWithAnimation(intProgress, 1000);

                    return true;
                }

                isTimer = false;
                //InitTimer();

                progressBar.SetProgressWithAnimation(10, 1000);

                return false;
            });
        }
        Stream GetStreamFromFile(string filename)
        {
            AssetManager assets = Assets;
            Stream sr = assets.Open(filename);

            return sr;
        }

    }
}