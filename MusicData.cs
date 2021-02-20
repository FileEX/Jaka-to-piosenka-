using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaka_to_piosenka
{
    public class MusicItem
    {
        public string Question { get; set; }
        public int GoodAnswerID { get; set; }
        public string SongFile { get; set; }
        public string AnswerOne { get; set; }
        public string AnswerTwo { get; set; }
        public string AnswerThree { get; set; }
        public string AnswerFour { get; set; }
    }
    public class MusicData
    {
        public List<MusicItem> musicTable = new List<MusicItem>
        {
          new MusicItem{ Question="Jaki jest tytuł tej piosenki?", GoodAnswerID = 2, AnswerOne="Autobiografia", AnswerTwo="Całkiem inny kraj", AnswerThree="Rzeczy do zrobienia", AnswerFour="17 lat", SongFile="201.mp3" },
          new MusicItem{ Question="Jaki jest tytuł tej piosenki?", GoodAnswerID = 1, AnswerOne="Biała flaga", AnswerTwo="Gdzie oni są?", AnswerThree="Gdzie są moi przyjaciele?", AnswerFour="Mądre przekonania", SongFile="202.mp3" },
          new MusicItem{ Question="Kto jest wykonawcą tej piosenki?", GoodAnswerID = 3, AnswerOne="Lady Pank", AnswerTwo="Perfect", AnswerThree="Video", AnswerFour="Wilki", SongFile="203.mp3"},
        };
    }
}